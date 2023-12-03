using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricWall
{
    public class SymbolTable : NodeVisitor
    {
        private Dictionary<string, Point> PointList;

        private Dictionary<string, Line> LineList;

        private Dictionary<string, Circle> CircleList;

        private Dictionary<string, Segment> SegmentList;

        private Dictionary<string, Ray> RayList;

        public SymbolTable()
        {
            PointList = new Dictionary<string, Point>();
            LineList = new Dictionary<string, Line>();
            CircleList = new Dictionary<string, Circle>();
            SegmentList = new Dictionary<string, Segment>();
            RayList = new Dictionary<string, Ray>();
        }

        public void AddGeometric(string name, string type = "")
        {
            Random random = new Random();

            switch (type)
            {
                case "point":
                    if (PointList.ContainsKey(name))
                        throw new ArgumentException("Ponit, " + name + " ya ha sido declarada");
                    PointList[name] = new Point(name, random.Next(50, 500), random.Next(50, 500));
                    break;
                case "line":
                    if (LineList.ContainsKey(name))
                        throw new ArgumentException("Line, " + name + " ya ha sido declarada");
                    LineList[name] = new Line(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                    break;
                case "segment":
                    if (SegmentList.ContainsKey(name))
                        throw new ArgumentException("Segment, " + name + " ya ha sido declarada");
                    SegmentList[name] = new Segment(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                    break;
                case "ray":
                    if (RayList.ContainsKey(name))
                        throw new ArgumentException("Ray, " + name + " ya ha sido declarada");
                    RayList[name] = new Ray(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), new Point("p2", random.Next(50, 500), random.Next(50, 500)));
                    break;
                case "circle":
                    if (CircleList.ContainsKey(name))
                        throw new ArgumentException("Circle, " + name + " ya ha sido declarada");
                    CircleList[name] = new Circle(name, new Point("p1", random.Next(50, 500), random.Next(50, 500)), random.Next(25, 50));
                    break;
                default:
                    break;
            }
        }

        public dynamic GetGeometric(string name)
        {
            if (PointList.ContainsKey(name))
            {
                return PointList[name];
            }
            else if (CircleList.ContainsKey(name))
            {
                return CircleList[name];
            }
            else if (SegmentList.ContainsKey(name))
            {
                return SegmentList[name];
            }
            else if (RayList.ContainsKey(name))
            {
                return RayList[name];
            }
            else if (LineList.ContainsKey(name))
            {
                return LineList[name];
            }
            else
                throw new ArgumentException(name + "No ha sido declarado");
        }

        public Point GetPoint(string name)
        {
            if (!PointList.ContainsKey(name))
                throw new ArgumentException("Point, " + name + " no ha sido declarada");
            return PointList[name];
        }

        public Circle GetCircle(string name)
        {
            if (!CircleList.ContainsKey(name))
                throw new ArgumentException("Circle, " + name + " no ha sido declarada");
            return CircleList[name];
        }

        public Segment GetSegment(string name)
        {
            if (!SegmentList.ContainsKey(name))
                throw new ArgumentException("Segment, " + name + " no ha sido declarada");
            return SegmentList[name];
        }

        public Line GetLine(string name)
        {
            if (!LineList.ContainsKey(name))
                throw new ArgumentException("Line, " + name + " no ha sido declarada");
            return LineList[name];
        }

        public Ray GetRay(string name)
        {
            if (!RayList.ContainsKey(name))
                throw new ArgumentException("Ray, " + name + " no ha sido declarada");
            return RayList[name];
        }
    }
}
