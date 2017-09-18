using System;
using System.Collections.Generic;
using System.IO;

public class LexerException : System.Exception
{
    public LexerException(string msg)
        : base(msg)
    {
    }

}

public enum Tok
{
    EOF,
    ID,
    INUM,
    COLON,
    SEMICOLON,
    ASSIGN,
    BEGIN,
    END,
    CYCLE
}

public class Lexer
{

    private string inputText;
    private int position;
    private char currentCh;                      // ������� ������
    private int LexRow, LexCol;                  // ������-������� ������ �������. ����� ������� = LexCol+LexText.Length
    private int row, col;                        // ������� ������ � ������� � �����
    private StringReader inputReader;
    private Dictionary<string, Tok> keywordsMap; // �������, �������������� �������� ������ ��������� ���� TLex. ���������������� ���������� InitKeywords 
    private Tok LexKind;                         // ��� �������
    private string LexText;                      // ����� �������
    private int LexValue;                        // ����� ��������, ��������� � �������� LexNum

    public Lexer(string input)
    {
        inputText = input;
        inputReader = new StringReader(input);
        keywordsMap = new Dictionary<string, Tok>();
        InitKeywords();
        row = 1; col = 1;
        NextCh();       // ������� ������ ������ � ch
        NextLexem();    // ������� ������ �������, �������� LexText, LexKind �, ��������, LexValue
    }

    private void PassSpaces()
    {
        while (char.IsWhiteSpace(currentCh))
        {
            NextCh();
        }
    }

    private void InitKeywords()
    {
        keywordsMap["begin"] = Tok.BEGIN;
        keywordsMap["end"] = Tok.END;
        keywordsMap["cycle"] = Tok.CYCLE;
    }

    private void LexError(string message)
    {
        var reader = new StringReader(inputText);
        for (int i = 0; i < row - 1; i++)
        {
            reader.ReadLine();
        }
        var errorString = reader.ReadLine();
        System.Text.StringBuilder errorDescription = new System.Text.StringBuilder();
        errorDescription.AppendFormat("Lexical error in line {0}:", row);
        errorDescription.Append("\n");
        errorDescription.Append(errorString);
        errorDescription.Append("\n");
        errorDescription.Append(new String(' ', col - 1) + '^'); 
        errorDescription.Append('\n');
        if (message != "")
        {
            errorDescription.Append(message);
        }
        throw new LexerException(errorDescription.ToString());
    }

    private void NextCh()
    {
        // � LexText ������������� ���������� ������ � ����������� ��������� ������
        LexText += currentCh;
        var nextChar = inputReader.Read();
        if (nextChar != -1)
        {
            currentCh = (char)nextChar;
            if (currentCh != '\n')
            {
                col += 1;
            }
            else
            {
                row += 1;
                col = 1;
            }
        }
        else
        {
            currentCh = (char)0; // ���� ��������� ����� �����, �� ������������ #0
        }
    }

    private void NextLexem()
    {
        PassSpaces();
        // R � ����� ������� ������ ������ ������� ������ � ch
        LexText = "";
        LexRow = row;
        LexCol = col;
        // ��� ������� ������������ �� �� ������� �������
        // ��� ������ ������� �������� �������������� ���������
        if (currentCh == ';') {
            NextCh();
            LexKind = Tok.SEMICOLON;
        } else if (currentCh == ':') {
            NextCh();
            if (currentCh != '=')
            {
                LexError("= was expected");
            }
            NextCh();
            LexKind = Tok.ASSIGN;
        } else if (char.IsLetter(currentCh)) {
            while (char.IsLetterOrDigit(currentCh))
            {
                NextCh();
            }
            if (keywordsMap.ContainsKey(LexText))
            {
                LexKind = keywordsMap[LexText];
            }
            else
            {
                LexKind = Tok.ID;
            }
        }
        else if (char.IsDigit(currentCh))
        {
            while (char.IsDigit(currentCh))
            {
                NextCh();
            }
            LexValue = Int32.Parse(LexText);
            LexKind = Tok.INUM;
        } else if ((int)currentCh == 0) {
            LexKind = Tok.EOF;
        } else {
            LexError("Incorrect symbol " + currentCh);
        }
    }

    public virtual void Parse()
    {
        do
        {
            Console.WriteLine(TokToString(LexKind));
            NextLexem();
        } while (LexKind != Tok.EOF);
    }

    private string TokToString(Tok t)
    {
        var result = t.ToString();
        switch (t)
        {
            case Tok.ID: result += ' ' + LexText;
                break;
            case Tok.INUM: result += ' ' + LexValue.ToString();
                break;
        }
        return result;
    }
}


public class Program
{
    public static void Main()
    {
        string fileContents = @"begin 
id23 := 24;  
cycle; 2 id258 id29 ; 
end";
        Lexer l = new Lexer(fileContents);
        try
        {
            l.Parse();
        }
        catch (LexerException e)
        {
            Console.WriteLine("lexer error: " + e.Message);
        }
    }
}