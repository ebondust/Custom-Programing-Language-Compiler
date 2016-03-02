using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompilerR
{
    class LexicalAnalyzerB
    {
        private int cIndex;
        private char lookAhead
        {
            get { return (code.Count-1>cIndex)?code[cIndex+1]:'a'; }
        }
        private char current
        {
            get { return code[cIndex]; }
        }
        private char previous
        {
            get { return (cIndex>0) ? code[cIndex - 1] : ' '; }
        }
        string currentLexeme="";
        List<char> code = new List<char>();
        List<Token> tokenizedCode = new List<Token>();

        int currentToken = 0;

        public Token getNextToken()
        {
            currentToken++;
            Token token = tokenizedCode[currentToken];
            return token;
        }
        public Token getCurrentToken()
        {
            Token token = tokenizedCode[currentToken];
            return token;
        }
        public Token getPreviousToken()
        {
            Token token = tokenizedCode[currentToken-1];
            return token;
        }
        public Token getLookAheadToken()
        {   
            Token token = ((tokenizedCode.Count-1>currentToken)?tokenizedCode[currentToken + 1]:new Token(" "," "));
           // Token token = tokenizedCode[currentToken + 1];
            return token;
        }
        public Token getLookAheadToken(int c)
        {
            Token token = ((tokenizedCode.Count - c > currentToken) ? tokenizedCode[currentToken + c] : new Token(" ", " "));
            // Token token = tokenizedCode[currentToken + 1];
            return token;
        }

        public void Read(string text)
        {
            if (text != null)
            {
                foreach (char a in text)
                    code.Add(a);
                tokenize();
            }
            else text = "";
        }
        public string getTypeCode()
        {
            string text="";
            foreach (Token token in tokenizedCode)
            {
                text += token.type + " ";
            }

            return text;
        }

     
        private void tokenize()
        {
            for (int i = 0; i < code.Count; i++)
            {
                cIndex = i;

                if (!isSpace(current))
                {
                    currentLexeme += current;
                    if (!isSpecial(current))
                    {
                        if (isSpecial(lookAhead))
                        {
                            tokenizedCode.Add(new Token(currentLexeme));
                            currentLexeme = "";
                        }
                    }
                    else
                    {
                        if (match('"'))
                        {
                            do
                            {
                                cIndex++;
                                i++;
                                currentLexeme += current;
                            } while (!match('"'));
                            tokenizedCode.Add(new Token(currentLexeme));
                            currentLexeme = "";

                        }
                        else
                        if (isSpecial(previous))
                        {
                            if (!isValidCombination(previous +current+ ""))
                            {
                                tokenizedCode.Add(new Token(currentLexeme));
                                currentLexeme = "";
                            }
                        }
                        else
                        {
                            if (!isSpecial(lookAhead))
                            {
                                tokenizedCode.Add(new Token(currentLexeme));
                                currentLexeme = "";
                            }
                            else if (!isValidCombination(""+current + lookAhead+""))
                            {
                                tokenizedCode.Add(new Token(currentLexeme));
                                currentLexeme = "";
                            }

                        }
                    }


                }
                else
                {
                    if (currentLexeme != "")
                    {
                        tokenizedCode.Add(new Token(currentLexeme));
                        currentLexeme = "";
                    }
                }

            }

            if (currentLexeme != "")
                tokenizedCode.Add(new Token(currentLexeme));
            tokenizedCode.Add(new Token("EOF", "EOF"));
            currentLexeme = "";

        }


        bool isEOF()
        {
            return (cIndex > code.Count) ? true : false;
        }

        public bool match(char token)
        {
            if (current == token)
                return true;
            else
                return false;
        }

        bool isValidCombination(string a)
        {
            return (Regex.IsMatch(a + "", "\\+\\+|\\-\\-|\\|\\||\\&&|==|\\>=|\\<=|\\+\\=|\\-\\=|\\*\\=|\\/\\=")) ? true : false;
        }
        bool isAddOp(char a)
        {
           // return (Regex.IsMatch(a + "", "\\+|\\-|\\*|\\/|\\%|\\=|\\>|\\<")) ? true : false;
            return (Regex.IsMatch(a + "", "\\+|\\-|\\*|\\/|\\%")) ? true : false;
        }
        public bool isSpecial(char a)
        {
            return Regex.IsMatch(a + "","^[\\,|\\+|\\-|\\*|\\/|\\=|\\;|\\%\\|\\&|\\>|\\<|\\(|\\)|\\{|\\}|\\[|\\]|\"]");
        }
        bool isSpace(char a)
        {
            return (a == ' ' || a == '\n' || a == '\t') ? true : false;
        }
    }
}
