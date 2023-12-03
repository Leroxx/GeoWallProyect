using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static GeometricWall.Token;

namespace GeometricWall
{
    public class Interpreter : NodeVisitor
    {
        private Parser Parser;
        private SymbolTable SymbolTable;
        private Draw Draw;

        public Interpreter(Parser parser, SymbolTable symbolTable, Draw draw)
        {
            this.Parser = parser;
            this.SymbolTable = symbolTable;
            this.Draw = draw;
        }

        public dynamic Visit_ProgramAST(ProgramAST tree)
        {
            foreach (var children in tree.Nodes)
            {
                var result = Visit(children);
            }

            return tree;
        }

        public dynamic Visit_PointAST(dynamic node)
        {
            string name = node.ID.VarName;
            SymbolTable.AddGeometric(name, "point");

            return node;
        }

        public dynamic Visit_CircleAST(dynamic node)
        {
            string name = node.ID.VarName;
            SymbolTable.AddGeometric(name, "circle");

            return node;
        }

        public dynamic Visit_SegmentAST(dynamic node)
        {
            string name = node.ID.VarName;
            SymbolTable.AddGeometric(name, "segment");

            return node;
        }

        public dynamic Visit_LineAST(dynamic node)
        {
            string name = node.ID.VarName;
            SymbolTable.AddGeometric(name, "line");

            return node;
        }

        public dynamic Visit_RayAST(dynamic node)
        {
            string name = node.ID.VarName;
            SymbolTable.AddGeometric(name, "ray");

            return node;
        }

        public dynamic Visit_DrawStatement(DrawStatement node)
        {
            if (node.Figure != "")
            {
                Point[] points = new Point[2];

                for (int i = 0; i < points.Length; i++)
                {
                    Point p1 = SymbolTable.GetPoint(node.Coordenada.ElementAt(i));
                    points[i] = p1;
                }

                switch (node.Figure)
                {
                    case "line":
                        Line line = new Line(" ", points[0], points[1]);
                        Draw.DrawLine(line);
                        break;
                    case "segment":
                        Segment segment = new(" ", points[0], points[1]);
                        Draw.DrawSegment(segment);
                        break;
                    case "ray":
                        Ray ray = new(" ", points[0], points[1]);
                        Draw.DrawRay(ray);
                        break;
                    case "circle":
                        // TODO HACK
                        Circle c1 = new(" ", points[0], 20);
                        Draw.DrawCircle(c1);
                        break;
                }
            }
            else
            {
                foreach(var item in node.Geometrics)
                {
                    var geometric = SymbolTable.GetGeometric(item);
                    string method_name = geometric.GetType().Name;

                    switch(method_name)
                    {
                        case "Point":
                            Draw.DrawPoint(geometric);
                            break;
                        case "Circle":
                            Draw.DrawCircle(geometric);
                            break;
                        case "Segment":
                            Draw.DrawSegment(geometric);
                            break;
                        case "Line":
                            Draw.DrawLine(geometric);
                            break;
                        case "Ray":
                            Draw.DrawRay(geometric);
                            break;
                        default:
                            throw new ArgumentException("El objeto indicado no es valido");
                    }
                }
            }

            return node;
        }

        public dynamic Visit_MeasureStatement(MeasureStatement node)
        {
            Point p1 = SymbolTable.GetPoint(node.P1.Value);
            Point p2 = SymbolTable.GetPoint(node.P2.Value);

            double distance = Math.Sqrt(Math.Pow(p2.X - p2.Y, 2) + Math.Pow(p2.Y - p1.Y, 2));
            MessageBox.Show(distance.ToString());

            return distance;
        }

        public dynamic Visit_BinOp(dynamic node)
        {
            if (node.OP.Type == TokenType.PLUS)
            {
                return Visit(node.Left) + Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.MINUS)
            {
                return Visit(node.Left) - Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.MUL)
            {
                return Visit(node.Left) * Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.DIV)
            {
                return Visit(node.Left) / Visit(node.Right);
            }
            else
            {
                throw new Exception($"Unknown operator: {node.OP.Type}");
            }
        }

        public dynamic Visit_UnaryOP(dynamic node)
        {
            Token.TokenType type = node.TokenType;

            if (type == Token.TokenType.PLUS)
                return +Visit(node.Exp);
            else if (type == Token.TokenType.MINUS)
                return -Visit(node.Exp);

            throw new Exception("Invalid operator");
        }

        public dynamic Visit_Num(Num node)
        {
            return node.Value;
        }

        public dynamic Interpret()
        {
            AST tree = Parser.Parse();
            return Visit(tree);
        }
    }
}