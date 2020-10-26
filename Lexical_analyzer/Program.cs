using System;
using System.Collections.Generic;
using System.IO;

namespace Lexical_analyzer
{
    class Program
    {
        enum States { S, INT, ID, ERROR, COLOR, STRING };
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(@"C:\Users\skoro\source\repos\Lexical_analyzer\Lexical_analyzer\input.txt");
            StreamWriter sw = new StreamWriter(@"C:\Users\skoro\source\repos\Lexical_analyzer\Lexical_analyzer\output.txt");

            string text = sr.ReadToEnd();

            States state = States.S;

            List<string> KeyWords = new List<string>() { "html", "head", "title", "meta", "link", "base", "basefont", "body", "img", "br", "p", "h1", "h2", "h3" };
            List<char> Separator = new List<char>() { '<', '>', '=', '/' };

            List<string> listID = new List<string>();
            List<int> listINT = new List<int>();

            int countLine = 1;

            string buf = "";

            int dt = 0;
            int negative = 0;

            foreach (char c in text)
            {
                switch (state)
                {
                    case States.S:
                        if (c == ' ' || c == '\t' || c == '\0' || c == '\r')
                        {
                            continue;
                        }
                        else if (c == '\n')
                        {
                            countLine++;
                            continue;
                        }
                        else if (char.IsLetter(c))
                        {
                            buf += c;
                            state = States.ID;
                        }
                        else if (char.IsDigit(c) || c == '-')
                        {
                            if (c == '-')
                            {
                                negative = 1;
                                buf += c;
                                state = States.INT;
                            }
                            else
                            {
                                buf += c;
                                dt = (int)(c - '0');
                                state = States.INT;
                            }

                        }
                        else if (c == '#')
                        {
                            buf += c;
                            state = States.COLOR;
                        }
                        else if (c == '\'')
                        {
                            buf += c;
                            state = States.STRING;
                        }
                        else if (Separator.Contains(c))
                        {
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", c, Separator.IndexOf(c));
                        }
                        else
                        {
                            buf += c;
                            state = States.ERROR;
                        }
                        break;
                    case States.ID:
                        if (char.IsLetter(c) || ((c == '1' || c == '2' || c == '3') && buf == "h") || c == '-')
                        {
                            buf += c;
                        }
                        else
                        {
                            if (c == ' ' || c == '\t' || c == '\0' || c == '\r' || c == '\n' || Separator.Contains(c))
                            {
                                if (KeyWords.Contains(buf))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "KEYWORD", buf, KeyWords.IndexOf(buf));
                                    buf = "";
                                }
                                else
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ID", buf, "NONE");
                                    listID.Add(buf);
                                    buf = "";
                                }
                                if (c == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(c))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", c, Separator.IndexOf(c));
                                }
                                state = States.S;
                            }
                            else
                            {
                                buf += c;
                                state = States.ERROR;
                            }
                        }
                        break;
                    case States.INT:
                        if (Char.IsDigit(c))
                        {
                            buf += c;
                            dt = dt * 10 + (int)(c - '0');
                        }
                        else
                        {
                            if (c == ' ' || c == '\t' || c == '\0' || c == '\r' || c == '\n' || Separator.Contains(c))
                            {
                                if (negative == 1)
                                {
                                    dt *= -1;
                                    negative = 0;
                                }
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "INT", dt, "NONE");
                                listINT.Add(dt);
                                dt = 0;
                                buf = "";
                                if (c == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(c))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", c, Separator.IndexOf(c));
                                }
                                state = States.S;
                            }
                            else
                            {
                                buf += c;
                                state = States.ERROR;
                            }
                        }
                        break;
                    case States.COLOR:
                        if (Char.IsLetterOrDigit(c))
                        {
                            buf += c;
                        }
                        else
                        {
                            if (c == ' ' || c == '\t' || c == '\0' || c == '\r' || c == '\n' || Separator.Contains(c))
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "COLOR", buf, "NONE");
                                buf = "";
                                if (c == '\n')
                                {
                                    countLine++;
                                }
                                if (Separator.Contains(c))
                                {
                                    sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", c, Separator.IndexOf(c));
                                }
                                state = States.S;
                            }
                            else
                            {
                                buf += c;
                                state = States.ERROR;
                            }
                        }
                        break;
                    case States.STRING:
                        if (c != '\'')
                        {
                            buf += c;
                        }
                        else
                        {
                            buf += c;
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "STRING", buf, "NONE");
                            buf = "";
                            state = States.S;
                        }
                        break;
                    case States.ERROR:
                        if (c != ' ' && c != '\t' && c != '\0' && c != '\r' && c != '\n' && !Separator.Contains(c))
                        {
                            buf += c;
                        }
                        else
                        {
                            sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "ERROR", buf, "NONE");
                            if (c == '\n')
                            {
                                countLine++;
                            }
                            if (Separator.Contains(c))
                            {
                                sw.WriteLine("{0,5}| {1,-10}| {2, -50}| {3, -6}|", countLine, "SEPARATOR", c, Separator.IndexOf(c));
                            }
                            buf = "";
                            state = States.S;
                        }
                        break;
                }
            }
            sr.Close();
            sw.Close();
        }
    }
}
