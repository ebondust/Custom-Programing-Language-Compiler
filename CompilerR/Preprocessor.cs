using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompilerR
{
    class Preprocessor
    {
        string output = "";
        public Preprocessor(string code)
        {
            List<string> tempList = new List<string>();

            tempList = code.Split('\n').ToList();

            foreach(string line in tempList)
            {
                if(line != null&&line!="")
                    if (line[0] != '#')
                    {
                        if (line.Length > 1 && line[0] == '/' && line[1] == '/')
                            continue;
                        if (line.Contains("//"))
                        {
                            string [] temp = line.Split(new string[1] { "//" }, StringSplitOptions.None);
                            output += temp[0];
                            continue;
                        }

                        output += line + '\n';
                    }
            }
        }

        public string getCode()
        {
            return output;
        }

    }
}
