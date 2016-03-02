using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class Variable
    {
        public string lexeme = "";
        public string type = "";
        public bool isInitialized = false;

        public Variable(string lexeme,string type)
        {
            this.lexeme = lexeme;
            this.type = type;
        }
    }
}
