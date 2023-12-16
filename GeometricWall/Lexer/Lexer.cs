using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static GeometricWall.Token;
using static System.Net.Mime.MediaTypeNames;

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
            Functions.Clear();
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

        private char Peek()
        {
            int peek_pos = pos + 1;
            if (peek_pos > text.Length - 1)
                return ' ';
            else
                return text[peek_pos];
        }

        public Token PeekToken()
        {
            int peek_pos = pos;
            Token token = GetNextToken();
            pos = peek_pos;
            currentChar = text[pos];

            return token;
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

        public Token ID()
        {
            string result = "";

            while (currentChar != '\0' && char.IsLetterOrDigit(currentChar))
            {
                result += currentChar;
                Advance();
            }

            Token token;

            if (GeometricDeclaration.ContainsKey(result))
            {
                token = GeometricDeclaration[result];
            }
            else if (ReserveKeyword.ContainsKey(result))
            {
                token = ReserveKeyword[result];
            }
            else if (LogicOperators.ContainsKey(result))
            {
                token = LogicOperators[result];
            }
            else if (Functions.Contains(result))
            {
                token = new Token(TokenType.FUNCTION_CALL, result);
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

                if (currentChar == '%')
                {
                    Advance();
                    return new Token(TokenType.MODULE, "%");
                }

                if (currentChar == '=')
                {
                    if (Peek() == '=')
                    {
                        Advance(); Advance();
                        return new Token(Token.TokenType.EQUAL, "==");
                    }
                    else
                    {
                        Advance();
                        return new Token(Token.TokenType.ASSIGN, "=");
                    }
                }

                if (currentChar == '<')
                {
                    if (Peek() == '=')
                    {
                        Advance(); Advance();
                        return new Token(Token.TokenType.LESS_THAN_OR_EQUAL, "<=");
                    }
                    else
                    {
                        Advance();
                        return new Token(Token.TokenType.LESS_THAN, "<");
                    }
                }

                if (currentChar == '>')
                {
                    if (Peek() == '=')
                    {
                        Advance(); Advance();
                        return new Token(Token.TokenType.GREATER_THAN_OR_EQUAL, ">=");
                    }
                    else
                    {
                        Advance();
                        return new Token(Token.TokenType.GREATER_THAN, ">");
                    }
                }

                if (currentChar == '!')
                {
                    if (Peek() == '=')
                    {
                        Advance(); Advance();
                        return new Token(Token.TokenType.NOT_EQUAL, "!=");
                    }
                }

                if (currentChar == ',')
                {
                    Advance();
                    return new Token(TokenType.COMMA, ",");
                }

                if (currentChar == '_')
                {
                    Advance();
                    return new Token(TokenType.UNDER_SCORE, "_");
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
