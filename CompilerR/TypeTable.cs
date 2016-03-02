using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    static class TypeTable
    {
        static List<string> typeTable = new List<string>();

        static TypeTable()
        {
            typeTable.Add("num");
            typeTable.Add("string");
            typeTable.Add("bool");
            typeTable.Add("void");
        }

        public static bool isVariableType(string lexeme)
        {
            foreach(string type in typeTable)
            {
                if(lexeme == type&&lexeme!="void")
                    return true;
            }
            return false;
        }
        public static bool isFunctionType(string lexeme)
        {
            foreach (string type in typeTable)
            {
                if (lexeme == type)
                    return true;
            }
            return false;
        }
    }
}
