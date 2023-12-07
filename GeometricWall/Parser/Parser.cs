using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GeometricWall.Token;
using System.Xml.Linq;
using System.Windows;
using System.Reflection;

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

                    if (currentToken.Type == TokenType.LPAREN)
                    {
                        Eat(TokenType.LPAREN);

                        AST center = Var();
                        Eat(TokenType.COMMA);
                        AST radius = Var();

                        Eat(TokenType.RPAREN);

                        CircleAST circle = new(new Var(new Token(TokenType.CIRCLE, "circle")), center, radius);
                        return circle;
                    }
                    else
                    {
                        CircleAST circle = new(Var(), null, null);
                        return circle;
                    }
                case TokenType.SEGMENT:
                    Eat(TokenType.SEGMENT);

                    if (currentToken.Type == TokenType.LPAREN)
                    {
                        Eat(TokenType.LPAREN);

                        AST p1 = Var();
                        Eat(TokenType.COMMA);
                        AST p2 = Var();

                        Eat(TokenType.RPAREN);

                        SegmentAST segment = new(new Var(new Token(TokenType.SEGMENT, "segment")), p1, p2);
                        return segment;
                    }
                    else
                    {
                        SegmentAST segement = new(Var(), null, null);
                        return segement;
                    }
                case TokenType.RAY:
                    Eat(TokenType.RAY);

                    if (currentToken.Type == TokenType.LPAREN)
                    {
                        Eat(TokenType.LPAREN);

                        AST p1 = Var();
                        Eat(TokenType.COMMA);
                        AST p2 = Var();

                        Eat(TokenType.RPAREN);

                        RayAST ray = new(new Var(new Token(TokenType.RAY, "ray")), p1, p2);
                        return ray;
                    }
                    else
                    {
                        RayAST ray = new(Var(), null, null);
                        return ray;
                    }
                case TokenType.LINE:
                    Eat(TokenType.LINE);

                    if (currentToken.Type == TokenType.LPAREN)
                    {
                        Eat(TokenType.LPAREN);

                        AST p1 = Var();
                        Eat(TokenType.COMMA);
                        AST p2 = Var();

                        Eat(TokenType.RPAREN);

                        LineAST line = new(new Var(new Token(TokenType.LINE, "line")), p1, p2);
                        return line;
                    }
                    else
                    {
                        LineAST line = new(Var(), null, null);
                        return line;
                    }
                default:
                    break;
            }
            Error();
            return null;
        }

        public AST DrawStatement()
        {
            LinkedList<AST> geometrics = new LinkedList<AST>();
            LinkedList<AST> coordenadas = new LinkedList<AST>();
            string figure = "";

            if (currentToken.Type == TokenType.LKEY)
            {
                Eat(TokenType.LKEY);
                while (currentToken.Type != TokenType.RKEY)
                {
                    if (currentToken.Type == TokenType.ID)
                    {
                        geometrics.AddLast(Var());
                    }
                    else
                        Eat(TokenType.COMMA);
                }
                Eat(TokenType.RKEY);

                DrawStatement draw = new(geometrics, figure, null, null);
                return draw;
            }
            else if (currentToken.Type == TokenType.ID)
            {
                geometrics.AddLast(Var());
                DrawStatement draw = new(geometrics, figure, null, null);
                return draw;
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

                AST param1 = Var();
                Eat(TokenType.COMMA);
                AST param2 = Var();

                Eat(TokenType.RPAREN);

                DrawStatement draw = new(geometrics, figure, param1, param2);
                return draw;
            }
        }

        public AST MeasureStatement()
        {
            Eat(TokenType.LPAREN);

            AST p1 = Var();
            Eat(TokenType.COMMA);
            AST p2 = Var();

            Eat(TokenType.RPAREN);

            MeasureStatement node = new(p1, p2);

            return node;
        }

        public AST ConstantStatement()
        {
            AST node = Assign();
            return node;
        }

        private AST Assign()
        {
            AST left = Var();
            Token token = currentToken;
            Eat(Token.TokenType.ASSIGN);
            AST right = Statement();
            Assign node = new(left, token, right);
            return node;
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
