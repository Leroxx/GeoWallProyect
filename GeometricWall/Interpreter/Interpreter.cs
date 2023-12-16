using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using static GeometricWall.Token;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Random random = new Random();
            Point p1 = new Point(name, random.Next(250, 350), random.Next(250, 350));
            SymbolTable.AddSymbol(name, p1, SymbolTable.VariableType.Point);

            return node;
        }

        public dynamic Visit_CircleAST(dynamic node)
        {
            string name = node.ID.VarName;

            if (name == "circle")
            {
                var center = Visit(node.Center);
                var radius = Visit(node.Radius);
                Circle circle = new("", center, radius);
                return circle;
            }
            else
            {
                Random random = new Random();
                Circle c1 = new Circle(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), random.Next(25, 50));
                SymbolTable.AddSymbol(name, c1, SymbolTable.VariableType.Circle);
                return node;
            }
        }

        public dynamic Visit_SegmentAST(dynamic node)
        {
            string name = node.ID.VarName;

            if (name == "segment")
            {
                var p1 = Visit(node.Point1);
                var p2 = Visit(node.Point2);
                Segment segment = new("",  p1, p2);
                return segment;
            }
            else
            {
                Random random = new Random();
                Segment s1 = new Segment(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                SymbolTable.AddSymbol(name, s1, SymbolTable.VariableType.Segment);
                return node;
            }
        }

        public dynamic Visit_LineAST(dynamic node)
        {
            string name = node.ID.VarName;

            if (name == "line")
            {
                var p1 = Visit(node.Point1);
                var p2 = Visit(node.Point2);
                Line line = new("", p1, p2);
                return line;
            }
            else
            {
                Random random = new Random();
                Line l1 = new Line(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                SymbolTable.AddSymbol(name, l1, SymbolTable.VariableType.Segment);
                return node;
            }
        }

        public dynamic Visit_RayAST(dynamic node)
        {
            string name = node.ID.VarName;

            if (name == "ray")
            {
                var p1 = Visit(node.Point1);
                var p2 = Visit(node.Point2);
                Ray ray = new("", p1, p2);
                return ray;
            }
            else
            {
                Random random = new Random();
                Ray ray = new Ray(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                SymbolTable.AddSymbol(name, ray, SymbolTable.VariableType.Segment);
                return node;
            }
        }

        public dynamic Visit_DrawStatement(dynamic node)
        {
            if (node.Figure != "")
            {
                var param1 = Visit(node.Param1);
                var param2 = Visit(node.Param2);

                switch (node.Figure)
                {
                    case "line":
                        Line line = new(" ", param1, param2);
                        Draw.DrawLine(line);
                        break;
                    case "segment":
                        Segment segment = new(" ", param1, param2);
                        Draw.DrawSegment(segment);
                        break;
                    case "ray":
                        Ray ray = new(" ", param1, param2);
                        Draw.DrawRay(ray);
                        break;
                    case "circle":
                        Circle c1 = new(" ", param1, param2);
                        Draw.DrawCircle(c1);
                        break;
                }
            }
            else
            {
                foreach(var item in node.Geometrics)
                {
                    var geometric = Visit(item);
                    string method_name = geometric.GetType().Name;

                    switch(method_name)
                    {
                        case "Point":
                            Draw.DrawPoint((Point)geometric);
                            break;
                        case "Circle":
                            Draw.DrawCircle((Circle)geometric);
                            break;
                        case "Segment":
                            Draw.DrawSegment((Segment)geometric);
                            break;
                        case "Line":
                            Draw.DrawLine((Line)geometric);
                            break;
                        case "Ray":
                            Draw.DrawRay((Ray)geometric);
                            break;
                        default:
                            throw new ArgumentException("El objeto indicado no es valido");
                    }
                }
            }

            return node;
        }

        public dynamic Visit_MeasureStatement(dynamic node)
        {
            string point1 = node.P1.VarName;
            string point2 = node.P2.VarName;
            
            Point p1 = (Point)SymbolTable.GetSymbol(point1).Item1;
            Point p2 = (Point)SymbolTable.GetSymbol(point2).Item1;

            double distance = Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            return distance;
        }

        public dynamic Visit_IntersectStatement(dynamic node)
        {
            dynamic value1 = Visit(node.Value1);
            dynamic value2 = Visit(node.Value2);

            if (value1.GetType().Name == "Circle" && value2.GetType().Name == "Circle")
                return Draw.FindCircleIntersections(value1, value2);
            else if (LineType(value1) && LineType(value2))
                return Draw.FindLineIntersection(value1, value2);
            else if ((LineType(value1) && value2.GetType().Name == "Circle"))
                return Draw.FindLineCircleIntersection(value1, value2);
            else if (value1.GetType().Name == "Circle" && LineType(value2))
                return Draw.FindLineCircleIntersection(value2, value1);
            else
                return null;
        }

        public bool LineType(dynamic line)
        {
            if (line.GetType().Name == "Line" || line.GetType().Name == "Segment" || line.GetType().Name == "Ray")
                return true;
            return false;
        }

        public dynamic Visit_Assign(dynamic node)
        {
            if (node.Vars is not null)
            {
                var secuencue = Visit(node.Expression);
                int count = 0;

                foreach (Var var in node.Vars)
                {
                    if (secuencue[count] != null)
                    {
                        SymbolTable.AddSymbol(var.VarName, secuencue[count], SymbolTable.VariableType.Secuence);
                    }
                    else
                        SymbolTable.AddSymbol(var.VarName, "", SymbolTable.VariableType.Undefine);
                    count++;
                }

                return node;
            }
            else
            {
                string name = node.Variable.VarName;
                var value = Visit(node.Expression);

                string method_name = value.GetType().Name;

                switch (method_name)
                {
                    case "Point":
                        value.ID = name;
                        SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Point);
                        break;
                    case "Circle":
                        value.ID = name;
                        SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Circle);
                        break;
                    case "Segment":
                        value.ID = name;
                        SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Segment);
                        break;
                    case "Line":
                        value.ID = name;
                        SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Line);
                        break;
                    case "Ray":
                        value.ID = name;
                        SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Ray);
                        break;
                    default:
                        break;
                }

                if (value is double)
                {
                    SymbolTable.AddSymbol(name, value, SymbolTable.VariableType.Double);
                }
                return name;
            }
        }

        public dynamic Visit_Var(dynamic node)
        {
            string name = node.VarName;
            return SymbolTable.GetSymbol(name).Item1;
        }

        public dynamic Visit_Secuence(dynamic node)
        {
            LinkedList<dynamic> values = new LinkedList<dynamic>();

            foreach (var value in node.Values)
            {
                values.AddLast(Visit(value));
            }

            return values;
        }

        public dynamic Visit_LetIN(dynamic node)
        {
            SymbolTable.PushTable();

            foreach (AST item in node.Expresions)
            {
                Visit(item);
            }

            var Value = Visit(node.InNode);
            SymbolTable.PopTable();
            return Value;
        }

        public dynamic Visit_IfElse(dynamic node)
        {
            if (Visit(node.Condition))
                return Visit(node.IfBlock);
            else
                return Visit(node.ElseBlock);
        }

        // Send the data to save a function on the SymbolTable
        public dynamic Visit_FunctionDeclaration(dynamic node)
        {
            List<string> parameters = new List<string>();

            foreach (var item in node.Parameters)
                parameters.Add(item.VarName);

            SymbolTable.AddFunction(node.Name, parameters, node.Expression);
            return node;
        }

        // Evaluate a function then have been declare
        public dynamic Visit_FunctionCall(dynamic node)
        {
            List<object> parameters = new List<object>();

            foreach (var item in node.Parameters)
                parameters.Add(Visit(item));

            AST result = SymbolTable.CallFunction(node.FunctionName, parameters);

            var Value = Visit(result);
            SymbolTable.PopTable();
            return Value;
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

        public dynamic Visit_LogicOP(dynamic node)
        {
            if (node.OP.Type == TokenType.LESS_THAN)
            {
                return Visit(node.Left) < Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.GREATER_THAN)
            {
                return Visit(node.Left) > Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.LESS_THAN_OR_EQUAL)
            {
                return Visit(node.Left) <= Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.GREATER_THAN_OR_EQUAL)
            {
                return Visit(node.Left) >= Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.EQUAL)
            {
                return Visit(node.Left) == Visit(node.Right);
            }
            else if (node.OP.Type == TokenType.NOT_EQUAL)
            {
                return Visit(node.Left) != Visit(node.Right);
            }

            throw new Exception("Invalid operator");
        }

        public dynamic Visit_ANDNode(dynamic node)
        {
            if (Visit(node.Left) == 1 && Visit(node.Right) == 1)
                return true;
            else
                return false;
        }

        public dynamic Visit_ORNode(dynamic node)
        {
            if (Visit(node.Left) == 1 || Visit(node.Right) == 1)
                return true;
            else
                return false;
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
            SymbolTable.PushTable();
            return Visit(tree);
        }
    }
}