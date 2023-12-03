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
            DRAW,
            MEASURE,
            LKEY,
            RKEY,
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
        };
        
        public static Dictionary<string, Token> GeometricDeclaration = new Dictionary<string, Token>()
        {
            { "point", new Token(TokenType.POINT, "point") },
            { "circle", new Token(TokenType.CIRCLE, "circle") },
            { "segment", new Token(TokenType.SEGMENT, "segment") },
            { "line", new Token(TokenType.LINE, "line") },
            { "ray", new Token(TokenType.RAY, "ray") }
        };

        public override string ToString()
        {
            return "Token(" + this.Type + "," + this.Value + ")";
        }
    }
}
