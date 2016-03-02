using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class Function
    {
        public string lexeme = "";
        private string name = "";

        public string Lexeme
        {
            get {return (name=="")?lexeme:name;}
        }
        public string type = "";
        public List<Variable> Arguments = new List<Variable>();
        public List<String> Code = new List<String>();
        public bool isInitialized = false;
        // public string returnValue;
        public List<Variable> declaredVariables = new List<Variable>();

        public Function(string lexeme, string type, List<Variable> Arguments)
        {
            this.lexeme = lexeme;
            this.type = type;
            this.Arguments = Arguments;
            foreach (Variable vari in Arguments)
                declaredVariables.Add(vari);
        }
        public Function(string lexeme, string type,string name, List<Variable> Arguments)
        {
            this.lexeme = lexeme;
            this.type = type;
            this.name = name;
            this.Arguments = Arguments;
            foreach (Variable vari in Arguments)
                declaredVariables.Add(vari);
        }

       
    }
}
