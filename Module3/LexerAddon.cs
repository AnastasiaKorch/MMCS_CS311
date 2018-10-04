using System;
using System.IO;
using SimpleScanner;
using ScannerHelper;
using System.Collections.Generic;

namespace  GeneratedLexer
{
    
    public class LexerAddon
    {
        public Scanner myScanner;
        private byte[] inputText = new byte[255];

        public int idCount = 0;
        public int minIdLength = Int32.MaxValue;
        public double avgIdLength = 0;
        public int maxIdLength = 0;
        public int sumInt = 0;
        public double sumDouble = 0.0;
        public List<string> idsInComment = new List<string>();
        public List<int> idLength = new List<int>();


        public LexerAddon(string programText)
        {
            
            using (StreamWriter writer = new StreamWriter(new MemoryStream(inputText)))
            {
                writer.Write(programText);
                writer.Flush();
            }
            
            MemoryStream inputStream = new MemoryStream(inputText);
            
            myScanner = new Scanner(inputStream);
        }

        public void Lex()
        {
            // Чтобы вещественные числа распознавались и отображались в формате 3.14 (а не 3,14 как в русской Culture)
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            int tok = 0;
            do {
                tok = myScanner.yylex();

                //if (tok == (int)Tok.ID || tok == (int)Tok.ID2)
                if (tok == (int)Tok.ID)
                    idLength.Add(myScanner.yytext.Length);

                if (tok == (int)Tok.ID2)
                    idsInComment.Add(myScanner.yytext);

                    if (tok == (int)Tok.INUM)
                    sumInt += myScanner.LexValueInt;

                if (tok == (int)Tok.RNUM)
                    sumDouble += myScanner.LexValueDouble;

                if (tok == (int)Tok.EOF)
                {
                    double totalLength = 0;
                    idCount = idLength.Count;
                    foreach (var item in idLength)
                    {
                        totalLength += item;
                        if (item > maxIdLength)
                            maxIdLength = item;
                        if (item < minIdLength)
                            minIdLength = item;
                    }
                    avgIdLength = totalLength / idCount;

                    break;
                }
            } while (true);
        }
    }
}

