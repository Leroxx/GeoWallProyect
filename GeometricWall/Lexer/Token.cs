using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricWall
{
    public class Token
    {
        public enum TokenType
        {
            NUMBER,
            PLUS,
            MINUS,
            MUL,
            DIV,
            LPAREN,
            RPAREN,
            SEMI,
            COMMA,
            POINT,
            CIRCLE,
            SEGMENT,
            RAY,
            LINE,
            ID,
            ASSIGN,
            LET,
            IN,
            IF,
            ELSE,
            THEN,
            MODULE,
            LESS_THAN,
            GREATER_THAN,
            LESS_THAN_OR_EQUAL,
            GREATER_THAN_OR_EQUAL,
            EQUAL,
            NOT_EQUAL,
            AND,
            OR,
            FUNCTION_CALL,
            DRAW,
            MEASURE,
            LKEY,
            RKEY,
            UNDER_SCORE,
            REST,
            INTERSECT,
            EOF
        }

        public Token(TokenType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        public TokenType Type { get; set; }
        public string Value { get; set; }

        public static Dictionary<string, Token> ReserveKeyword = new Dictionary<string, Token>()
        {
            { "draw", new Token(TokenType.DRAW, "draw") },
            { "measure", new Token(TokenType.MEASURE, "measure") },
            { "rest", new Token(TokenType.REST, "rest") },
            { "intersect", new Token(TokenType.INTERSECT, "intersect") },
            { "let", new Token(TokenType.LET, "let")},
            { "in", new Token(TokenType.IN, "in")},
            { "if", new Token(TokenType.IF, "if")},
            { "else", new Token(TokenType.ELSE, "else")},
            { "then", new Token(TokenType.THEN, "then")}
        };
        
        public static Dictionary<string, Token> GeometricDeclaration = new Dictionary<string, Token>()
        {
            { "point", new Token(TokenType.POINT, "point") },
            { "circle", new Token(TokenType.CIRCLE, "circle") },
            { "segment", new Token(TokenType.SEGMENT, "segment") },
            { "line", new Token(TokenType.LINE, "line") },
            { "ray", new Token(TokenType.RAY, "ray") }
        };

        public static Dictionary<string, Token> LogicOperators = new Dictionary<string, Token>()
        {
            { "and", new Token(TokenType.AND, "point") },
            { "or", new Token(TokenType.OR, "circle") }
        };

        public static List<string> Functions = new List<string>();

        public override string ToString()
        {
            return "Token(" + this.Type + "," + this.Value + ")";
        }
    }
}
