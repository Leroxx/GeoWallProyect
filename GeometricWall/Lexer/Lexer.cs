using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeometricWall.Token;

namespace GeometricWall
{
    public class Lexer
    {
        private string text;
        private int pos;
        private char currentChar;

        public Lexer(string text)
        {
            this.text = text;
            this.pos = 0;
            this.currentChar = this.text[this.pos];
        }

        private void Error(string messege)
        {
            throw new ArgumentException("Lexical Error: '" + messege + "' is not valid token");
        }

        public void Advance()
        {
            pos++;
            if (pos > text.Length - 1)
            {
                currentChar = '\0'; // Indicates end of input
            }
            else
            {
                currentChar = text[pos];
            }
        }

        public void SkipWhitespace()
        {
            while (currentChar != '\0' && char.IsWhiteSpace(currentChar))
            {
                Advance();
            }
        }

        public string Integer()
        {
            string result = "";
            while (currentChar != '\0' && char.IsDigit(currentChar))
            {
                result += currentChar;
                Advance();
            }

            if (currentChar == '.')
            {
                result += currentChar;
                Advance();

                while (currentChar != '\0' && char.IsDigit(currentChar))
                {
                    result += currentChar;
                    Advance();
                }
            }

            if (currentChar != ' ' && currentChar != ';' && currentChar != ',' && currentChar != ')')
                Error(result + currentChar);

            return result;
        }

        //TODO
        public Token ID()
        {
            string result = "";

            while (currentChar != '\0' && char.IsLetterOrDigit(currentChar))
            {
                result += currentChar;
                Advance();
            }

            Token token;

            if (Token.GeometricDeclaration.ContainsKey(result))
            {
                token = Token.GeometricDeclaration[result];
            }
            else if (Token.ReserveKeyword.ContainsKey(result))
            {
                token = Token.ReserveKeyword[result];
            }
            else
            {
                token = new Token(Token.TokenType.ID, result);
            }

            return token;
        }

        public Token GetNextToken()
        {
            while (currentChar != '\0')
            {
                if (char.IsWhiteSpace(currentChar))
                {
                    SkipWhitespace();
                    continue;
                }

                if (char.IsDigit(currentChar))
                {
                    return new Token(TokenType.NUMBER, Integer());
                }

                if (char.IsLetter(currentChar))
                {
                    return ID();
                }

                if (currentChar == '+')
                {
                    Advance();
                    return new Token(TokenType.PLUS, "+");
                }

                if (currentChar == '-')
                {
                    Advance();
                    return new Token(TokenType.MINUS, "-");
                }

                if (currentChar == '*')
                {
                    Advance();
                    return new Token(TokenType.MUL, "*");
                }

                if (currentChar == '/')
                {
                    Advance();
                    return new Token(TokenType.DIV, "/");
                }

                if (currentChar == '(')
                {
                    Advance();
                    return new Token(Token.TokenType.LPAREN, "(");
                }

                if (currentChar == ')')
                {
                    Advance();
                    return new Token(Token.TokenType.RPAREN, ")");
                }

                if (currentChar == '{')
                {
                    Advance();
                    return new Token(TokenType.LKEY, "{");
                }

                if (currentChar == '}')
                {
                    Advance();
                    return new Token(TokenType.RKEY, "}");
                }

                if (currentChar == ',')
                {
                    Advance();
                    return new Token(TokenType.COMMA, ",");
                }

                if (currentChar == ';')
                {
                    Advance();
                    return new Token(TokenType.SEMI, ";");
                }

                Error(currentChar.ToString());
            }

            return new Token(TokenType.EOF, null);
        }
    }
}
