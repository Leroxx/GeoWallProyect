using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeometricWall.Token;
using System.Xml.Linq;
using System.Windows;

namespace GeometricWall
{
    public class Parser
    {
        private Lexer lexer;
        private Token currentToken;

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            this.currentToken = this.lexer.GetNextToken();
        }

        public void Error()
        {
            throw new Exception("Invalid syntax");
        }

        public void Eat(TokenType tokenType)
        {
            if (currentToken.Type == tokenType)
            {
                currentToken = lexer.GetNextToken();
            }
            else
            {
                Error();
            }
        }

        public AST Factor()
        {
            Token token = currentToken;

            if (token.Type == Token.TokenType.PLUS)
            {
                Eat(Token.TokenType.PLUS);
                UnaryOP node = new(token, Factor());
                return node;
            }
            else if (token.Type == Token.TokenType.MINUS)
            {
                Eat(Token.TokenType.MINUS);
                UnaryOP node = new(token, Factor());
                return node;
            }
            else if (token.Type == Token.TokenType.LPAREN)
            {
                Eat(Token.TokenType.LPAREN);
                AST node = Expr();
                Eat(Token.TokenType.RPAREN);
                return node;
            }
            else if (token.Type == TokenType.NUMBER)
            {
                Eat(TokenType.NUMBER);
                return new Num(token);
            }
            else
            {
                Error();
                return null; // Just to satisfy the return type
            }
        }

        public AST Term()
        {
            AST node = Factor();

            while (currentToken.Type == Token.TokenType.MUL || currentToken.Type == Token.TokenType.DIV)
            {
                Token token = currentToken;

                if (token.Type == Token.TokenType.MUL)
                    Eat(Token.TokenType.MUL);
                else if (token.Type == Token.TokenType.DIV)
                    Eat(Token.TokenType.DIV);

                node = new BinOp(node, token, Factor());
            }

            return node;
        }

        public AST Expr()
        {
            AST node = Term();

            while (currentToken.Type == TokenType.PLUS || currentToken.Type == TokenType.MINUS)
            {
                Token token = currentToken;

                if (token.Type == TokenType.PLUS)
                    Eat(TokenType.PLUS);
                else if (token.Type == Token.TokenType.MINUS)
                    Eat(TokenType.MINUS);

                node = new BinOp(node, token, Term());
            }

            return node;
        }

        private AST Var()
        {
            Var node = new(currentToken);

            if (currentToken.Type == Token.TokenType.ID)
                Eat(TokenType.ID);
            else
                throw new ArgumentException("Syntax Error: Missing variable name");

            return node;
        }

        public AST GeometricDeclaration()
        {
            switch (currentToken.Type)
            {
                case TokenType.POINT:
                    Eat(TokenType.POINT);
                    PointAST point = new(Var());
                    return point;
                case TokenType.CIRCLE:
                    Eat(TokenType.CIRCLE);
                    CircleAST circle = new(Var());
                    return circle;
                case TokenType.SEGMENT:
                    Eat(TokenType.SEGMENT);
                    SegmentAST segement = new(Var());
                    return segement;
                case TokenType.RAY:
                    Eat(TokenType.RAY);
                    RayAST ray = new(Var());
                    return ray;
                case TokenType.LINE:
                    Eat(TokenType.LINE);
                    LineAST line = new(Var());
                    return line;
                default:
                    break;
            }
            Error();
            return null;
        }

        public AST DrawStatement()
        {
            LinkedList<string> geometrics = new LinkedList<string>();
            LinkedList<string> coordenadas = new LinkedList<string>();
            string figure = "";

            if (currentToken.Type == TokenType.LKEY)
            {
                Eat(TokenType.LKEY);
                while (currentToken.Type != TokenType.RKEY)
                {
                    if (currentToken.Type == TokenType.ID)
                    {
                        geometrics.AddLast(currentToken.Value);
                        Eat(TokenType.ID);
                    }
                    else
                        Eat(TokenType.COMMA);
                }
                Eat(TokenType.RKEY);


            }
            else if (currentToken.Type == TokenType.ID)
            {
                geometrics.AddLast(currentToken.Value);
                Eat(TokenType.ID);
                Eat(TokenType.SEMI);
            }
            else
            {
                switch (currentToken.Type)
                {
                    case TokenType.POINT:
                        figure = "point";
                        Eat(TokenType.POINT);
                        break;
                    case TokenType.LINE:
                        figure = "line";
                        Eat(TokenType.LINE);
                        break;
                    case TokenType.SEGMENT:
                        figure = "segment";
                        Eat(TokenType.SEGMENT);
                        break;
                    case TokenType.RAY:
                        figure = "ray";
                        Eat(TokenType.RAY);
                        break;
                    case TokenType.CIRCLE:
                        figure = "circle";
                        Eat(TokenType.CIRCLE);
                        break;
                }

                Eat(TokenType.LPAREN);
                while (currentToken.Type != TokenType.RPAREN)
                {
                    if (currentToken.Type == TokenType.ID)
                    {
                        coordenadas.AddLast(currentToken.Value);
                        Eat(TokenType.ID);
                    }
                    else
                        Eat(TokenType.COMMA);

                }
                Eat(TokenType.RPAREN);

            }

            DrawStatement draw = new(geometrics, figure, coordenadas);

            return draw;
        }

        public AST MeasureStatement()
        {
            Eat(TokenType.LPAREN);
            Token[] tokens = new Token[2];
            Token token;
                
            token = currentToken;
            Eat(TokenType.ID);
            tokens[0] = token;

            Eat(TokenType.COMMA);

            token = currentToken;
            Eat(TokenType.ID);
            tokens[1] = token;


            MessageBox.Show(currentToken.Type.ToString());
            Eat(TokenType.RPAREN);
            MeasureStatement node = new(tokens[0], tokens[1]);

            return node;
        }

        // TODO
        public AST ConstantStatement()
        {
            return null;
        }

        public AST Statement()
        {
            if (Token.GeometricDeclaration.ContainsKey(currentToken.Value))
            {
                return GeometricDeclaration();
            }
            else if (currentToken.Type == TokenType.DRAW)
            {
                Eat(TokenType.DRAW);
                return DrawStatement();
            }
            else if (currentToken.Type == TokenType.MEASURE)
            {
                Eat(TokenType.MEASURE);
                return MeasureStatement();
            }
            else if (currentToken.Type == TokenType.ID)
            {
                return ConstantStatement();
            }
            else
            {
                return Expr();
            }
        }

        public AST Tree()
        {
            AST node = Statement();

            LinkedList<AST> nodes = new();
            nodes.AddLast(node);

            while (currentToken.Type == TokenType.SEMI)
            {
                Eat(TokenType.SEMI);
                if (currentToken.Type != TokenType.EOF)
                    nodes.AddLast(Statement());
            }

            ProgramAST tree = new(nodes);

            return tree;
        }

        public AST Parse()
        {
            return Tree();
        }
    }
}
