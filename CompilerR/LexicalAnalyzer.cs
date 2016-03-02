using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompilerR
{
    class LexicalAnalyzer
    {
        /*  protected struct Token
          {
              public Token(int value, string lexeme)
              {
                  this.value = value;
                  this.lexeme = lexeme;
              }
              int value;
              string lexeme;
          }
          protected const Token EOF = new Token(0, "/0");
          protected const Token SEMICOLON = new Token(0, ";");
          protected const Token PLUS = new Token(0, "+");
          protected const Token MINUS = new Token(0, "-");
          protected const Token TIMES = new Token(0, "*");
          protected const Token OVER = new Token(0, "/");
          protected const Token LEFT_PAR = new Token(0, "(");
          protected const Token RIGHT_PAR = new Token(0, ")");
          protected const Token EQUAL = new Token(0, "=");
          protected Token NUMBER = new Token(0, "/0");
          protected Token VARIABLE = new Token(0, "/0");
          */
         protected const int EOF = 0;
         protected const int SEMICOLON = 1;
         protected const int PLUS = 2;
         protected const int MINUS = 3;
         protected const int TIMES = 4;
         protected const int OVER = 5;
         protected const int LEFT_PAR = 6;
         protected const int RIGHT_PAR = 7;
         protected const int EQUAL = 8;
         protected const int NUMBER = 9;
         protected const int VARIABLE = 10;

        public int lookAhead;
        List<char> buffer = new List<char>();
        int current = -1;
        protected string value;

        public void initialize(string text)
        {
            lookAhead = -1;
            foreach (char a in text)
                buffer.Add(a);
        }

        int getNextToken()
        {
            //value = "";
            current++;

            while (buffer.Count > current && isSpace(buffer[current]))
                current++;
            if (endOfFile())
                return EOF;

            switch (buffer[current])
            {
                case ';': return SEMICOLON;
                case '+': return PLUS;
                case '-': return MINUS;
                case '*': return TIMES;
                case '/': return OVER;
                case '(': return LEFT_PAR;
                case ')': return RIGHT_PAR;
                case '=': return EQUAL;
                default:
                    value = "";
                    if (!isNumber(buffer[current]))
                    {
                        if (isVariable(buffer[current]))
                        {

                            while (!endOfFile() && isVariable(buffer[current]))
                            {
                                value += "" + buffer[current];
                                current++;
                            }
                            if (!endOfFile())
                                current--;

                            return VARIABLE;
                        }
                        else
                        {
                            Form1.code += "/n + expected number got :" + buffer[current];
                            return EOF;
                        }
                    }
                    else
                    {
                        while (!endOfFile() && isNumber(buffer[current]))
                        {
                            value += "" + buffer[current];
                            // current++;
                            current++;
                        }
                        //Form1.code += "returned number \n ";
                            if (!endOfFile())
                        current--;

                        return NUMBER;
                    }
            }
        }

        public bool match(int token)
        {
            if (lookAhead == -1)
                lookAhead = getNextToken();
            return (token == lookAhead) ? true : false;
        }
        public void advance()
        {
            lookAhead = getNextToken();
        }

        bool isSpace(char a)
        {
            return (a == ' ' || a == '\n' || a == '\t') ? true : false;
        }
        bool isNumber(char a)
        {
            return (Regex.IsMatch(a + "", "^[0-9]+$")) ? true : false;
        }
        protected bool isVariable(char a)
        {
            return (Regex.IsMatch(a + "", "^[A-Za-z]+$")) ? true : false;
        }
        bool endOfFile()
        {
            return (buffer.Count > current) ? false : true;
        }
    }
}
