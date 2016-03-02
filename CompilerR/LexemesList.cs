using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    static class LexemesList
    {
        static List<string> lexemes = new List<string>();
        static LexemesList()
        {
            lexemes.Add("+");
            lexemes.Add("-");
            lexemes.Add("*");
            lexemes.Add("/");
        }
    }
}
