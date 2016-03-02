using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class EndPoint
    {
        List<string> Code = new List<string>();
        List<string> CodeToParse;
        bool main = true;
        bool cFlag = false;
        string buffor = "";
        public EndPoint(List<string> CodeToParse)
        {
            Code = new List<string>();
            this.CodeToParse = CodeToParse;
            CreateCode();
        }

        private void Add(string line)
        {
            Code.Add(line+"\n");
        }

        private void addTop()
        {
            Add("format PE console");
            Add("entry main \n");
            Add("include 'macro/import32.inc'");
            Add("include 'macro/proc32.inc'");
            Add("include 'win32ax.inc'\n");  
            Add("section '.data' data readable writeable");
            Add("p db \"pause>nul\",0");
        }

        private void addMain()
        {
            Parse();
        }

        private void Parse()
        {
            foreach (String line in CodeToParse)
            {
                string[] delimeters = { "+=", "-=", "*=", "//=","==",">=","<=","&&","||","!=","=" };
                string[] s = line.Split(delimeters, StringSplitOptions.None);
                if (cFlag)
                {
                    cFlag = false;

                    string db = s[0].Remove(0, 1);
                    db = db.Remove(db.Length - 1, 1);
                    db = db.Remove(db.Length-1, 1);
                    string ds = db + " DB 24 DUP(?)";
                    for (int i = 0; i < Code.Count;i++)
                    {

                        if (Code[i] == ds)
                        {
                            Code[i] = db + ". DB " + buffor + ",10,0";
                            Code.Insert(i,db+" DD "+db+".");
                        }
                    }

                    continue;
                }
                if (s.Length > 1)
                {
                    if (line.Contains(".if"))
                    {
                        string ifLine = line.Replace("==","=");
                        ifLine = ifLine.Replace("!=", "~=");
                        ifLine = ifLine.Replace("&&", "&");
                        ifLine = ifLine.Replace("||", "|");

                        string adLine = s[0].Replace(".if ", "");
                        Code.Add("MOV EAX,"+"["+adLine+"]");

                        s[1] = s[1].Replace("\n","");

                        ifLine = ifLine.Replace(s[1], "["+s[1]+"]");

                        int index = ifLine.IndexOf(adLine);
                        ifLine = ifLine.Substring(0, index) + "EAX" + ifLine.Substring(index + 1);

                        Code.Add(ifLine);
                        
                    }
                    else if (line.Contains("+="))
                    {
                        Code.Add("ADD " + s[0] + "," + s[1]);
                    }
                    else if (line.Contains("*="))
                    {
                        Code.Add("MUL " + s[1]);
                    }
                    else if (line.Contains("-="))
                    {
                        if (s[1].Contains("Pop()"))
                            Code.Add("POP " + s[0]);
                        else
                            Code.Add("SUB " + s[0] + "," + s[1]);
                    }
                    else if (line.Contains("="))
                    {
                        if (s[1].Contains("Pop()"))
                            Code.Add("POP " + s[0]);
                        else if (s[1].Contains("\""))
                        {
                            cFlag = true;
                            buffor = s[1];
                            buffor = buffor.Remove(buffor.Length - 1, 1);
                        }
                        else
                            Code.Add("MOV " + s[0] + "," + s[1]);
                    }
                  
                }
                else
                    if ((s[0].Contains("num") || s[0].Contains("string") || s[0].Contains("bool")))
                    {
                        if (s[0].Contains("num"))
                        {
                            string db = s[0].Remove(0, 3);
                            db = db.Remove(db.Length - 1);
                            Code.Add(db+ " DD ?" );
                        }
                        if (s[0].Contains("string"))
                        {
                            string db = s[0].Remove(0, 6);
                            db = db.Remove(0,1);
                            db = db.Replace("\n"," ");
                            Code.Add(db + "DB 24 DUP(?)");
                        }
                        if (s[0].Contains("bool"))
                        {
                            string db = s[0].Remove(0, 6);
                            Code.Add(db + "byte 0");
                        }
                    }
                    else if(s[0].ToLower().Contains("main:"))
                    {
                       // main = false;
                        Add("section '.code' code readable executable");
                        Code.Add(line);
                    }
                    else if (s[0].Contains("Push"))
                    {
                        string result = "";
                        bool flag = false;
                        foreach (char c in s[0])
                        {
                            if (flag)
                            {
                                if (c == ')')
                                    break;
                                result += c;
                            }
                            if (c == '(')
                                flag = true;

                        }
                        Code.Add("Push " + result);
                    }
                    else if (s[0].Contains("Neg"))
                    {
                        string result = "";
                        bool flag = false;
                        foreach (char c in s[0])
                        {
                            if (flag)
                            {
                                if (c == ')')
                                    break;
                                result += c;
                            }
                            if (c == '(')
                                flag = true;

                        }
                        Code.Add("NEG " + result);
                    }
                    else
                    {
                        Code.Add(line);
                    }

            }
        }

        private void addBottom()
        {
            addWriteLine();
            addWriteNumber();
            addReadLine();

            Add("section '.idata' import data readable");
            Add("library msvcrt,'msvcrt.dll'");
            Add("import msvcrt,\\");
            Add("printf,'printf',\\");
            Add("scanf,'scanf',\\");
            Add("system,'system',\\");
            Add("exit,'exit'");
        }


        public void CreateCode()
        {
            addTop();
            addMain();
            addBottom();
        }

        public string getCode()
        {
            string result = "";
            foreach (String line in Code)
            {
                result += line;
                if (line[line.Length - 1] != '\n')
                    result+='\n';
            }
            return result;
        }

        private void addWriteLine()
        {
            string writeline =
             " WriteString dd _WriteString\n"
            + "proc _WriteString,text\n"
            + "invoke printf,[text]\n"
            + "ret\n"
            + "endp\n";

            Code.Add(writeline);
        }
        private void addReadLine()
        {
            string readline =
              "ReadNum dd _ReadNum\n"
             + "proc _ReadNum\n"
             + "locals\n"
             + "ftm dw \"%d\",0\n"
             + "tmp dd ?\n"
             + "endl  \n"
             + "LEA EAX,[ftm]\n"
             + "LEA EBX,[tmp]\n"
             + "invoke scanf,EAX,EBX\n"
             + "MOV EAX,[tmp]\n"
             + "ret\n"
             + "endp\n";

            Code.Add(readline);
        }

        private void addWriteNumber()
        {
            string writeline =
             " WriteNum dd _WriteNum\n"
             + "proc _WriteNum,number\n"
             + " locals \n"
             + "ftm dw \"%d\",0\n"
             + "endl   \n"

             + " LEA EAX,[ftm] \n"

             + "invoke printf,EAX,[number]\n"
             + "ret\n"
             + "endp\n";
            Code.Add(writeline);
        }
    }
}
