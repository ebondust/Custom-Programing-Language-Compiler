using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CompilerR
{
    class Token
    {
        public Token(string lexeme)
        {
            this.lexeme = lexeme;
            if (isAddOp(lexeme))
                type = "AddOp";
            else if (isMulOp(lexeme))
                type = "MulOp";
            else if (isNumber(lexeme))
                type = "Number";
            else if (isString(lexeme))
                type = "String";
            else if (isIdentifier(lexeme))
                type = "Identifier";
            else if (isAssigment(lexeme))
                type = "Assigmen";
            else if (isConditional(lexeme))
                type = "Conditional";
            else if (isParenthesisL(lexeme))
                type = "ParenthezisL";
            else if (isParenthesisR(lexeme))
                type = "ParenthezisR";
            else if (isSqBracketL(lexeme))
                type = "SqBracketL";
            else if (isSqBracketR(lexeme))
                type = "SqBracketR";
            else if (isBraceL(lexeme))
                type = "BraceL";
            else if (isBraceR(lexeme))
                type = "BraceR";
            else if (isSemicolon(lexeme))
                type = "Semicolon";
            else if (isComma(lexeme))
                type = "Comma";
            else
                type = "Invalid";
        }
        public Token(string lexeme,string type)
        {
            this.lexeme = lexeme;
            this.type = type;
        }
        public string lexeme;
        public string type;

        bool isAddOp(string lexeme)
        {
            return (Regex.IsMatch(lexeme, "^[\\+|\\-]+$")) ? true : false;
        }
        bool isMulOp(string lexeme)
        {
            return (Regex.IsMatch(lexeme, "^[\\*|\\/]+$")) ? true : false;
        }
        bool isConditional(string lexeme)
        {
            return (Regex.IsMatch(lexeme, "[\\|\\||\\&&|==|\\>=|\\<=|\\!=]")) ? true : false;
        }
        bool isAssigment(string lexeme)
        {
            return (lexeme == "=" || lexeme == "+=" || lexeme == "-=" || lexeme == "*=" || lexeme == "\\=") ? true : false;
        }
        bool isNumber(string lexeme)
        {
            //return (Regex.IsMatch(lexeme, "^[0-9]+$|^[0-9]+$\\.[0-9]+")) ? true : false;
            return (Regex.IsMatch(lexeme, "^[0-9]+$|^[0-9]+\\.[0-9]+$|^[0-9]+\\.[0-9]*$")) ? true : false; 
        }
        bool isString(string lexeme)
        {
            return (Regex.IsMatch(lexeme, "^\".*\"$")) ? true : false;
        }
        bool isIdentifier(string lexeme)
        {
            return (Regex.IsMatch(lexeme, "^[a-z|A-Z]")&&!Regex.IsMatch(lexeme, "[\\+|\\-|\\*|\\/]+")) ? true : false;
        }
        bool isSemicolon(string lexeme)
        {
            return (lexeme==";") ? true : false;
        }
        bool isParenthesisL(string lexeme)
        {
            return (lexeme == "(") ? true : false;
        }
        bool isParenthesisR(string lexeme)
        {
            return (lexeme == ")") ? true : false;
        }
        bool isBraceL(string lexeme)
        {
            return (lexeme == "{") ? true : false;
        }
        bool isBraceR(string lexeme)
        {
            return (lexeme == "}") ? true : false;
        }
        bool isSqBracketL(string lexeme)
        {
            return (lexeme == "[") ? true : false;
        }
        bool isSqBracketR(string lexeme)
        {
            return (lexeme == "]") ? true : false;
        }
        bool isComma(string lexeme)
        {
            return (lexeme == ",") ? true : false;
        }
    }
}
