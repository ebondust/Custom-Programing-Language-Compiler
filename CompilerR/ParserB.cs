using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class ParserB
    {
        protected const int eof = 0;
        protected const int Expression = 1;
        protected const int Term = 2;
        protected const int BTerm = 3;
        protected const int Assigment = 4;
        protected const int Factor = 5;
        protected const int BFactor = 6;
        protected const int Declaration = 7;
        protected const int Condition = 8;

        protected const string AddOp = "AddOp";
        protected const string MulOp = "MulOp";
        protected const string Number = "Number";
        protected const string Word = "String";
        protected const string Identifier = "Identifier";
        protected const string Assigmen = "Assigmen";
        protected const string Conditional = "Conditional";
        protected const string ParenthezisL = "ParenthezisL";
        protected const string ParenthezisR = "ParenthezisR";
        protected const string SqBracketL = "SqBracketL";
        protected const string SqBracketR = "SqBracketR";
        protected const string BraceL = "BraceL";
        protected const string BraceR = "BraceR";
        protected const string Semicolon = "Semicolon";
        protected const string Comma = "Comma";
        protected const string EOF = "EOF";

        LexicalAnalyzerB lex = new LexicalAnalyzerB();
        Stack<string> tempVariables = new Stack<string>();
        List<Variable> DeclaredVariables = new List<Variable>();
        List<Function> DeclaredFunctions = new List<Function>();
        Function curFunc;
        public List<string> pAsmCode = new List<string>();
        public List<string> errorList = new List<string>();
        string code = "";

        public void addTempVar()
        {
            tempVariables.Push("temp" + tempVariables.Count);
        }
        public void addTempVar(string tmp)
        {
            tempVariables.Push(tmp);
        }

        public void Parse(string code)
        {
            Form1.code = "";
            DeclaredFunctions = new List<Function>();

            Function func = new Function("WriteString","void",new List<Variable>(){new Variable("text","string")});

            DeclaredFunctions.Add(func);
            func = new Function("WriteNum", "void", new List<Variable>() { new Variable("number", "num") });
            DeclaredFunctions.Add(func);
            func = new Function("ReadNum", "num", new List<Variable>());
            DeclaredFunctions.Add(func);

            curFunc = null;
            lex.Read(new Preprocessor(code).getCode());
            Statements();
        }

        private void Statements()
        {
            while (!matchType(EOF))
            {
             
                if (TypeTable.isFunctionType(lex.getCurrentToken().lexeme))
                {
                    if (lex.getLookAheadToken(2).type == ParenthezisL)
                        funcExpression();
                    else
                    {
                        outputError("cannot declare code outside of functions");
                        return;
                    }
                }
                else
                {
                    //expression();
                    outputError("cannot declare code outside of functions");
                    return;
                }
                if (matchType(Semicolon))
                    lex.getNextToken();
                else
                {
                    outputError("missing semicolon");
                }
            }
            createFinalCode();
        }

        private void inFuncStatements()
        {
            if (match("if"))
            {
                string sdx = ".if ";
                while (lex.getNextToken().lexeme != ";")
                    sdx += lex.getCurrentToken().lexeme;
                curFunc.Code.Add(sdx);
                lex.getNextToken();
                return;
                    
            }
            if (match("else"))
            {
                string sdx = ".else ";
                curFunc.Code.Add(sdx);
                lex.getNextToken();
                lex.getNextToken();
                return;
            }
            if (match("endif"))
            {
                string sdx = ".endif ";
                curFunc.Code.Add(sdx);
                lex.getNextToken();
                lex.getNextToken();
               return;
            }
            if (TypeTable.isVariableType(lex.getCurrentToken().lexeme))
                varExpression();
            else
                if (lex.getCurrentToken().lexeme == "return")
                {
                    lex.getNextToken();
                    expression();
                    //   outputLine("EAX = "+lex.getCurrentToken().lexeme);
                    //    lex.getNextToken();
                }
                else
                    expression();
            if (matchType(Semicolon))
                lex.getNextToken();
            else
            {
                outputError("missing semicolon");

            }
        }

        void varExpression()
        {
            if (lex.getNextToken().type == Identifier)
            {
                curFunc.declaredVariables.Add(new Variable(lex.getCurrentToken().lexeme, lex.getPreviousToken().lexeme));
                expression();
            }

        }
        void funcExpression()
        {
            string returnType = lex.getCurrentToken().lexeme;
            if (lex.getNextToken().type == Identifier)
            {
                // DeclaredFunctions.Add(new Function(lex.getCurrentToken().lexeme, lex.getPreviousToken().lexeme, new List<Variable>()));
                string name = lex.getCurrentToken().lexeme;
                List<Variable> varList = new List<Variable>();
                List<string> varLexemes = new List<string>();
                Variable var = new Variable("", "");
                lex.getNextToken();
                lex.getNextToken();
                while (!matchType(ParenthezisR))
                {
                    if (matchType(Identifier) && TypeTable.isVariableType(lex.getCurrentToken().lexeme))
                        var.type = lex.getCurrentToken().lexeme;
                    else
                    {
                        outputError("expected variable type got : " + lex.getCurrentToken().lexeme);
                        break;
                    }
                    lex.getNextToken();

                    if (matchType(Identifier) && varLexemes.Where(x => x == lex.getCurrentToken().lexeme).ToList().Count == 0)
                        var.lexeme = lex.getCurrentToken().lexeme;
                    else
                    {
                        outputError("expected variable name or variable already declared : " + lex.getCurrentToken().lexeme);
                        break;
                    }
                    varLexemes.Add(lex.getCurrentToken().lexeme);
                    varList.Add(var);
                    lex.getNextToken();
                    if (matchType(Comma))
                        lex.getNextToken();
                    else if (!matchType(ParenthezisR))
                    {
                        outputError("expected comma(,) got :" + lex.getCurrentToken().lexeme);
                        break;
                    }
                }
                DeclaredFunctions.Add(new Function(name, returnType, varList));
                curFunc = DeclaredFunctions.Last();

                string vars = "";

                foreach (Variable v in varList)
                    vars += "," + v.lexeme;
                if (curFunc.lexeme.ToLower() == "main")
                    curFunc.Code.Add("\n" + name + ":" + "\n");
                else
                {
                    curFunc.Code.Add("\n" + name + " dd _" + name);
                    curFunc.Code.Add("proc _" + name + vars + "\n");
                    curFunc.Code.Add("locals");
                    curFunc.Code.Add("endl\n");
                }
                lex.getNextToken();
                if (matchType(BraceL))
                {
                    lex.getNextToken();
                    while (!matchType(BraceR))
                    {
                        inFuncStatements();

                        if (matchType(EOF))
                        {
                            outputError("expected curly bracket righ got :" + lex.getCurrentToken().lexeme);
                            break;
                        }
                    }
                    if (curFunc.lexeme.ToLower() != "main")
                    {
                        curFunc.Code.Add("\n" + "ret");
                        curFunc.Code.Add("endp" + "\n");
                    }
                    curFunc = null;
                    lex.getNextToken();

                }
                else
                    outputError("expected curly bracket left got :" + lex.getCurrentToken().lexeme);
            }
            else
                outputError("Expected Left Parenthesis got :" + lex.getCurrentToken().lexeme);

        }

        void expression()
        {

            if (match("-"))
                outputLine("EAX" + " = " + "0");
            else
                term();
            while (matchType(AddOp))
            {
                outputLine("Push(EAX)");
                if (match("+"))
                {
                    lex.getNextToken();
                    term();

                    outputLine("EBX" + " = " + "Pop()");
                    outputLine("EAX" + " += " + "EBX");
                }
                else
                {
                    lex.getNextToken();
                    term();

                    outputLine("EBX" + " = " + "Pop()");
                    outputLine("EAX" + " -= " + "EBX");
                    outputLine("Neg(EAX)");
                }
                //shiftToVariable(tempvar);
            }
        }


        void term()
        {
            assigment();
            while (matchType(MulOp))
            {
                outputLine("Push(EAX)");
                if (match("*"))
                {
                    lex.getNextToken();
                    factor();
                    outputLine("EBX" + " = " + "Pop()");
                    outputLine("EAX *= EBX");
                }
                else
                {
                    lex.getNextToken();
                    factor();
                    outputLine("EBX = Pop()");
                    outputLine("Dx = 0");
                    outputLine("EAX /= EBX");
                }

            }
        }
        void bTerm()
        {

        }
        void assigment()
        {
            factor();
            while (matchType(Assigmen))
            {
                if (lex.getPreviousToken().type == Number)
                {
                    outputError("Expected Variable Recived : " + lex.getPreviousToken().lexeme);
                    lex.getNextToken();
                    return;
                }



                if (lex.getPreviousToken().type == Identifier && !isVariableDeclared(lex.getPreviousToken().lexeme))
                {
                    outputError("Variable undeclared : " + lex.getPreviousToken().lexeme);
                    lex.getNextToken();
                    return;
                }
                //outputLine("Expected Variable Recived : " + value); }
                if (match("="))
                {
                    lex.getNextToken();
                    expression();
                    string tmp;
                    try
                    {
                        tmp = tempVariables.Pop();
                    }
                    catch (Exception e) { tmp = "Null"; }

                    outputLine("[" + tmp + "]" + " = " + "EAX");
                }
                else if (match("+="))
                {
                    lex.getNextToken();
                    expression();
                    outputLine("[" + tempVariables.Pop() + "]" + " += " + "EAX");
                }
                else if (match("-="))
                {
                    lex.getNextToken();
                    expression();
                    outputLine("[" + tempVariables.Pop() + "]" + " -= " + "EAX");
                }
                else if (match("*="))
                {
                    lex.getNextToken();
                    expression();
                    outputLine("[" + tempVariables.Pop() + "]" + " *= " + "EAX");
                }
                else if (match("/="))
                {
                    lex.getNextToken();
                    expression();
                    outputLine("[" + tempVariables.Pop() + "]" + " /= " + "EAX");
                }


            }
        }

        void factor()
        {
            if (matchType(Identifier))
            {
                if (matchType(Identifier) && lex.getLookAheadToken().type == ParenthezisL)
                {
                    callFunc();
                    lex.getNextToken();
                }
                else
                {
                    if (lex.getLookAheadToken().type != Assigmen)
                        outputLine("EAX = " + "[" + lex.getCurrentToken().lexeme + "]");
                    else
                        addTempVar(lex.getCurrentToken().lexeme);
                    lex.getNextToken();
                }
            }
            else
                if (matchType(Number))
                {
                    outputLine("EAX = " + lex.getCurrentToken().lexeme);

                    // addTempVar(lex.getCurrentToken().lexeme);
                    lex.getNextToken();
                }
                else
                    if (matchType(Word))
                    {
                        outputLine("EAX = " + lex.getCurrentToken().lexeme);

                        // addTempVar(lex.getCurrentToken().lexeme);
                        lex.getNextToken();
                    }
                    else if (matchType(ParenthezisL))
                    {
                        lex.getNextToken();
                        expression();
                        if (matchType(ParenthezisR))
                            lex.getNextToken();
                        else
                        {
                            outputError("Expected Right parenthesis");
                            lex.getNextToken();
                        }
                    }
                    else
                    {
                        outputError("Expected parenthesis , or variable");
                        lex.getNextToken();
                    }
        }

        public void callFunc()
        {
            if (DeclaredFunctions.Where(x => x.lexeme == lex.getCurrentToken().lexeme).ToList().Count > 0)
            {
                List<string> varList = new List<string>();

                string funcType = DeclaredFunctions.Where(x => x.lexeme == lex.getCurrentToken().lexeme).ToList()[0].type;
                string funcName = lex.getCurrentToken().lexeme;

                string var = "";
                lex.getNextToken();
                lex.getNextToken();
                while (!matchType(ParenthezisR))
                {

                    if (matchType(Identifier) || matchType(Number) || matchType(Word))
                    {
                        if(!matchType(Identifier))
                            var = lex.getCurrentToken().lexeme;
                        else
                            var = "["+lex.getCurrentToken().lexeme+"]";
                    }
                    else
                    {
                        outputError("Special haracters cannot be passed to function: " + lex.getCurrentToken().lexeme);
                        break;
                    }

                    varList.Add(var);
                    lex.getNextToken();
                    if (matchType(Comma))
                        lex.getNextToken();
                    else if (!matchType(ParenthezisR))
                    {
                        outputError("expected comma(,) got :" + lex.getCurrentToken().lexeme);
                        break;
                    }
                }

                string f = "";
                if (varList.Count > 0)
                    foreach (string s in varList)
                    {
                        f = "," + s;
                        // outputLine("push EAX");
                        // f += s + ",";
                    }
                //   else
                //  f += ")";

                // List<char> cL = f.ToList();
                //   cL[cL.Count-1]=')';
                //   f="";
                //  foreach (char s in cL)
                {
                    //      f+=s;
                }

                outputLine("invoke " + funcName + f);
            }
            else
                outputError("Function " + lex.getCurrentToken().lexeme + "() undeclared");

        }

        void bFactor()
        {

        }

        void declaration()
        {

        }

        void condition()
        {

        }


        public bool matchType(string token)
        {
            if (lex.getCurrentToken().type == token)
                return true;
            else
                return false;
        }
        public bool match(string lexeme)
        {
            if (lex.getCurrentToken().lexeme == lexeme)
                return true;
            else
                return false;
        }

        public bool isVariableDeclared(string lexeme)
        {
            if (curFunc.declaredVariables.Count(s => s.lexeme == lexeme) > 0)
                return true;
            else
                return false;
        }

        private void outputLine(string line)
        {
            if (curFunc != null)
            {
                curFunc.Code.Add(line);
            }
            else
            {
                Form1.code += "Code must be declared in function" + "\n";
                outputError("Code must be declared in function");
            }
        }

        private void outputError(string line)
        {
            errorList.Add(line+"\n");
        }

        private void addCode(string line)
        {
            code += line;
            pAsmCode.Add(line);

            Form1.code += line;
        }

        public string getCode()
        {
            return code;
        }

        private void createFinalCode()
        {
            Function mainFunction = DeclaredFunctions.Where(x => x.lexeme.ToLower() == "main").ToList().FirstOrDefault();
            if (mainFunction == null)
                outputError("No main function");
            else
            {
                foreach (Variable var in mainFunction.declaredVariables)
                    addCode(var.type + " " + var.lexeme + "\n");

                foreach (string line in mainFunction.Code)
                    addCode(line + "\n");
                addCode(" ");
                addCode("mov dword [esp],p");
                addCode("call [system]");
                addCode("mov dword [esp],0");
                addCode("call [exit]");

                foreach (Function func in DeclaredFunctions)
                {
                    if (func.lexeme.ToLower() != "main")
                    {
                        foreach (string line in func.Code)
                        {
                            addCode(line + "\n");
                            if (line == "locals")
                                foreach (Variable var in func.declaredVariables)
                                {
                                    if (!(func.Arguments.Where(x => x.lexeme == var.lexeme).ToList().Count > 0))
                                        addCode(var.type + " " + var.lexeme + "\n");
                                }

                        }
                    }
                }
            }
        }
    }
}