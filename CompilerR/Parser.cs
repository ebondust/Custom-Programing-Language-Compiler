using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class Parser:LexicalAnalyzer
    {
       /* const int EOF = 0;
        const int SEMICOLON = 1;
        const int PLUS = 2;
        const int MINUS = 3;
        const int TIMES = 4;
        const int OVER = 5;
        const int LEFT_PAR = 6;
        const int RIGHT_PAR = 7;
        const int EQUAL = 8;
        const int NUMBER = 9;
        const int VARIABLE = 10;*/

        List<string> variableList = new List<string>() {"t0","t1","t2","t3","t4"};
        int varPointer = -1;
        string currentVar
        {
            get {return(variableList.Count>0)? variableList[varPointer]:"Null"; }
        }
        string tempvar = "";
        string tempvar2 = "";
        public Parser(string text)
        {
            Form1.code = "";
            initialize(text);
            statements();
        }

        void statements()
        {
            while(!match(EOF))
            {
               tempvar = expression();
               shiftToVariable(tempvar);

                if (match(SEMICOLON))
                    advance();
                else
                {
                    outputLine("missing semicolon");
                }

            }
        }

        string expression()
        {
            string tempvar = term();
            while (match(PLUS) || match(MINUS))
            {
                //nextVariable();
                advance();
                tempvar2 = term();
                outputLine(tempvar +"+=" + tempvar2);
                shiftToVariable(tempvar);
            }
            return tempvar;
        }

        string term()
        {
            string tempvar =  assigment();
            while (match(TIMES) || match(OVER))
            {
                advance();
                tempvar2 = factor();
                outputLine(tempvar + "*=" + tempvar2);
            }
            return tempvar;
        }
        string assigment()
        {
            string tempvar = factor();
            while (match(EQUAL))
            {
                if (!isVariable(value[0]))
                { outputLine("Expected Variable Recived : " + value); }
                advance();
                tempvar2 = expression();
                outputLine(tempvar + "=" + tempvar2);
            }
            return tempvar;
        }

        string factor()
        {
            if (match(VARIABLE))
            {
                //outputLine("b" + "=" + value);
                advance();
                return value;
            }
            else
            if (match(NUMBER))
            {
                nextVariable();

                outputLine(currentVar + "="+value);
                advance();
            }
            else if (match(LEFT_PAR))
            {
                advance();
                string tempvar = expression();
                if (match(RIGHT_PAR))
                    advance();
                else
                {
                    outputLine("Expected Right parenthesis");
                }
            }
            else
            {
                outputLine("Expected parenthesis or number");
            } 
            return currentVar;
        }
        private void outputLine(string line)
        {
            Form1.code += line + "\n";
        }

        public string nextVariable()
        {
           // variableList.Add("t"+variableList.Count);
            varPointer++;
           // tempvar = currentVar;
            return currentVar;
        }
        public void shiftToVariable(string var)
        {
            varPointer = variableList.IndexOf(var);
            if (varPointer > 5|| varPointer == -1)
            {
                varPointer = 1;
                outputLine("Unexpected error occurred");
            }
        }
    }
}
