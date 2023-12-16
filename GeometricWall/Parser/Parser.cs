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
using System.Reflection.Emit;

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

        public void Error(Token token)
        {
            throw new Exception("Invalid syntax el Token: " + token + "no era el token esperado");
        }

        public void Eat(TokenType tokenType)
        {
            if (currentToken.Type == tokenType)
            {
                currentToken = lexer.GetNextToken();
            }
            else
            {
                Error(currentToken);
            }
        }

        public AST Factor()
        {
            Token token = currentToken;

            if (token.Type == TokenType.PLUS)
            {
                Eat(TokenType.PLUS);
                UnaryOP node = new(token, Factor());
                return node;
            }
            else if (token.Type == TokenType.MINUS)
            {
                Eat(TokenType.MINUS);
                UnaryOP node = new(token, Factor());
                return node;
            }
            else if (token.Type == TokenType.LPAREN)
            {
                Eat(TokenType.LPAREN);
                AST node = Expr();
                Eat(TokenType.RPAREN);
                return node;
            }
            else if (token.Type == TokenType.NUMBER)
            {
                Eat(TokenType.NUMBER);
                return new Num(token);
            }
            if (Token.GeometricDeclaration.ContainsKey(currentToken.Value))
            {
                return GeometricDeclaration();
            }
            else if (token.Type == Token.TokenType.FUNCTION_CALL)
            {
                AST node = FunctionCall();
                return node;
            }
            else
            {
                AST node = Var();
                return node;
            }
        }

        public AST Term()
        {
            AST node = Factor();

            while (currentToken.Type == TokenType.MUL || currentToken.Type == TokenType.DIV || currentToken.Type == TokenType.MODULE)
            {
                Token token = currentToken;

                if (token.Type == TokenType.MUL)
                    Eat(TokenType.MUL);
                else if (token.Type == TokenType.DIV)
                    Eat(TokenType.DIV);
                else if (token.Type == TokenType.MODULE)
                    Eat(TokenType.MODULE);


                node = new BinOp(node, token, Factor());
            }

            if (currentToken.Type == TokenType.LESS_THAN ||
                currentToken.Type == TokenType.GREATER_THAN ||
                currentToken.Type == TokenType.LESS_THAN_OR_EQUAL ||
                currentToken.Type == TokenType.GREATER_THAN_OR_EQUAL ||
                currentToken.Type == TokenType.EQUAL ||
                currentToken.Type == TokenType.NOT_EQUAL)
            {
                Token token = currentToken;

                if (token.Type == TokenType.LESS_THAN)
                    Eat(TokenType.LESS_THAN);
                else if (token.Type == TokenType.GREATER_THAN)
                    Eat(TokenType.GREATER_THAN);
                else if (token.Type == TokenType.LESS_THAN_OR_EQUAL)
                    Eat(TokenType.LESS_THAN_OR_EQUAL);
                else if (token.Type == TokenType.GREATER_THAN_OR_EQUAL)
                    Eat(TokenType.GREATER_THAN_OR_EQUAL);
                else if (token.Type == TokenType.EQUAL)
                    Eat(TokenType.EQUAL);
                else if (token.Type == TokenType.NOT_EQUAL)
                    Eat(TokenType.NOT_EQUAL);

                node = new LogicOP(node, token, Factor());
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
                else if (token.Type == TokenType.MINUS)
                    Eat(TokenType.MINUS);

                node = new BinOp(node, token, Term());
            }

            while (currentToken.Type == TokenType.AND || currentToken.Type == TokenType.OR)
            {
                Token token = currentToken;

                if (token.Type == TokenType.AND)
                {
                    Eat(TokenType.AND);
                    node = new ANDNode(node, Term());
                }
                else if (token.Type == TokenType.OR)
                {
                    Eat(TokenType.OR);
                    node = new ORNode(node, Term());
                }
            }

            return node;
        }

        private AST Var()
        {
            Var node = new(currentToken);
            if (currentToken.Type == TokenType.ID)
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
            Error(currentToken);
            return null;
        }

        public AST DrawStatement()
        {
            LinkedList<AST> geometrics = new LinkedList<AST>();
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
                if (currentToken.Type == TokenType.LPAREN)
                    Error(currentToken);

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
            LinkedList<AST> vars = new LinkedList<AST>();
            Token token = currentToken;

            if (token.Type == TokenType.COMMA)
            {
                vars.AddLast(left);
                Eat(TokenType.COMMA);
                if (currentToken.Type == TokenType.ID)
                    vars.AddLast(Var());

                while (currentToken.Type == TokenType.COMMA)
                {
                    Eat(TokenType.COMMA);

                    if (currentToken.Type == TokenType.REST || currentToken.Type == TokenType.UNDER_SCORE)
                    {
                        if (currentToken.Type == TokenType.REST)
                            Eat(TokenType.REST);
                        else
                            Eat(TokenType.UNDER_SCORE);
                        break;
                    }
                    else vars.AddLast(Var());
                }

                if (currentToken.Type == TokenType.REST)
                    Eat(TokenType.REST);
                else if (currentToken.Type == TokenType.UNDER_SCORE)
                    Eat(TokenType.UNDER_SCORE);

                Eat(Token.TokenType.ASSIGN);
                AST exp = Statement();

                Assign secuence = new Assign(vars, exp);
                return secuence;
            }

            Eat(Token.TokenType.ASSIGN);
            AST right = Statement();
            Assign node = new(left, token, right);
            return node;
        }

        public AST SecuenceStatement()
        {
            Eat(TokenType.LKEY);
            LinkedList<AST> secuence = new LinkedList<AST>();

            while (currentToken.Type != TokenType.RKEY)
            {
                if (currentToken.Type != TokenType.COMMA)
                {
                    secuence.AddLast(Expr());
                }
                else if (currentToken.Type == TokenType.COMMA)
                    Eat(TokenType.COMMA);
                else
                    Error(currentToken);
            }
            Eat(TokenType.RKEY);

            Secuence node = new Secuence(secuence);
            return node;
        }

        public AST IntersectStatement()
        {
            Eat(TokenType.INTERSECT);

            Eat(TokenType.LPAREN);
            AST value1 = Expr();

            Eat(TokenType.COMMA);
            AST value2 = Expr();
            Eat(TokenType.RPAREN);

            IntersectStatement node = new IntersectStatement(value1, value2);
            return node;
        }

        private AST LetInStatement()
        {
            Token token = currentToken;
            LinkedList<AST> statements = new LinkedList<AST>();

            AST node = Statement();
            statements.AddLast(node);

            while (currentToken.Type != TokenType.IN && currentToken.Type == TokenType.SEMI)
            {
                Eat(TokenType.SEMI);

                if (currentToken.Type == TokenType.EOF)
                    Eat(TokenType.IN);

                if (currentToken.Type == TokenType.IN)
                    break;

                node = Statement();
                statements.AddLast(node);
            }

            Eat(TokenType.IN);
            LetIN let_node = new(statements, Statement());
            return let_node;
        }

        private AST IfStatement()
        {
            AST node_condition;
            AST if_block;

            node_condition = Statement();

            Eat(TokenType.THEN);
            if_block = Statement();

            Eat(TokenType.ELSE);
            IfElse node = new(if_block, node_condition, Statement());

            return node;
        }

        private AST FunctionStatement()
        {
            Token token = currentToken;
            string name = "";
            LinkedList<AST> var_list = new LinkedList<AST>();
            AST node;

            if (currentToken.Type == TokenType.ID)
                Eat(TokenType.ID);
            else
                throw new ArgumentException("Syntax Error: Missing function name");

            name = token.Value;

            Eat(TokenType.LPAREN);

            while (currentToken.Type != TokenType.RPAREN)
            {
                if (currentToken.Type == TokenType.COMMA)
                    Eat(TokenType.COMMA);

                node = Var();
                var_list.AddLast(node);
            }

            Eat(TokenType.RPAREN);
            Eat(TokenType.ASSIGN);

            Functions.Add(name);
            FunctionDeclaration function_node = new(name, var_list, Statement());
            return function_node;
        }

        private AST FunctionCall()
        {
            Token token = currentToken;

            Eat(Token.TokenType.FUNCTION_CALL);
            Eat(Token.TokenType.LPAREN);
            List<AST> list = new List<AST>();

            while (currentToken.Type != Token.TokenType.RPAREN)
            {
                if (currentToken.Type == Token.TokenType.COMMA)
                    Eat(Token.TokenType.COMMA);

                AST exp = Expr();
                list.Add(exp);
            }

            Eat(Token.TokenType.RPAREN);
            FunctionCall node = new(token.Value, list);
            return node;
        }

        public AST Statement()
        {
            if (currentToken.Type == TokenType.DRAW)
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
                if (lexer.PeekToken().Type == TokenType.ASSIGN || lexer.PeekToken().Type == TokenType.COMMA)
                    return ConstantStatement();
                else if (lexer.PeekToken().Type == TokenType.LPAREN)
                    return FunctionStatement();
                else
                    return Expr();
            }
            else if (currentToken.Type == TokenType.LKEY)
            {
                return SecuenceStatement();
            }
            else if (currentToken.Type == TokenType.INTERSECT)
            {
                return IntersectStatement();
            }
            else if (currentToken.Type == TokenType.LET)
            {
                Eat(TokenType.LET);
                return LetInStatement();
            }
            else if (currentToken.Type == TokenType.IF)
            {
                Eat(TokenType.IF);
                return IfStatement();
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
