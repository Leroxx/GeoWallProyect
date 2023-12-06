using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using static GeometricWall.Token;

namespace GeometricWall
{
    public abstract class AST { }

    public class ProgramAST : AST
    {
        public LinkedList<AST> Nodes { get; }

        public ProgramAST(LinkedList<AST> nodes)
        {
            Nodes = nodes;
        }
    }

    #region Binary and Unary Operations
    public class BinOp : AST
    {
        public BinOp(AST left, Token op, AST right)
        {
            this.Left = left;
            this.OP = op;
            this.Right = right;
        }

        public AST Left { get; set; }
        public AST Right { get; set; }
        public Token OP { get; set; }
    }

    public class Num : AST
    {
        public Num(Token token)
        {
            this.TokenType = token.Type;
            this.Value = double.Parse(token.Value, CultureInfo.InvariantCulture);
        }

        public TokenType TokenType { get; set; }
        public double Value { get; set; }
    }

    public class UnaryOP : AST
    {
        public UnaryOP(Token op, AST exp)
        {
            this.TokenType = op.Type;
            this.Exp = exp;
        }

        public Token.TokenType TokenType { get; set; }
        public AST Exp { get; set; }
    }
    #endregion

    #region Gemetric
    public class PointAST : AST
    {
        public PointAST(AST id)
        {
            this.ID = id;
        }

        public AST ID { get; set; }
    }

    public class CircleAST : AST
    {
        public CircleAST(AST id, AST center, AST radius)
        {
            this.ID = id;
            this.Center = center;
            this.Radius = radius;
        }

        public AST ID { get; set; }
        public AST Center { get; set; }
        public AST Radius { get; set; }
    }

    public class LineAST : AST
    {
        public LineAST(AST id, AST point1, AST point2)
        {
            this.ID = id;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public AST ID { get; set; }

        public AST Point1 { get; set; }
        public AST Point2 { get; set; }
    }

    public class SegmentAST : AST
    {
        public SegmentAST(AST id, AST point1, AST point2)
        {
            this.ID = id;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public AST ID { get; set; }

        public AST Point1 { get; set; }
        public AST Point2 { get; set; }
    }

    public class RayAST : AST
    {
        public RayAST(AST id, AST point1, AST point2)
        {
            this.ID = id;
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public AST ID { get; set; }

        public AST Point1 { get; set; }
        public AST Point2 { get; set; }
    }
    #endregion

    public class DrawStatement : AST
    {
        public DrawStatement(LinkedList<AST> geometrics, string figure, AST param1, AST param2)
        {
            Geometrics = geometrics;
            Figure = figure;
            Param1 = param1;
            Param2 = param2;
        }

        public LinkedList<AST> Geometrics { get; set; }
        public string Figure { get; set; }
        public AST Param1 { get; set; }
        public AST Param2 { get; set; }
    }

    public class MeasureStatement : AST
    {
        public MeasureStatement(AST p1, AST p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public AST P1 { get; set; }
        public AST P2 { get; set; }
    }

    public class Assign : AST
    {
        public Assign(AST var, Token op, AST expression)
        {
            this.Variable = var;
            this.Token = op;
            this.Expression = expression;
        }

        public AST Variable { get; set; }
        public Token Token { get; set; }
        public AST Expression { get; set; }
    }

    public class Var : AST
    {
        public Var(Token token)
        {
            this.VarName = token.Value;
        }

        public string VarName { get; set; }
    }
}
