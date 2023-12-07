using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricWall
{
    public class Point
    {
        public Point(string id, double x, double y)
        {
            this.ID = id;
            this.X = x;
            this.Y = y;
        }

        public string ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString()
        {
            return "ID: " + ID + "(x: " + this.X + "y: " + this.Y + ")";
        }
    }

    public class Circle
    {
        public Circle(string id, Point center, double radio)
        {
            this.ID = id;
            this.Center = center;
            this.Radio = radio;
        }

        public string ID { get; set; }
        public Point Center { get; set; }
        public double Radio { get; set; }
    }

    public class Line
    {
        public Line(string id, Point p1, Point p2)
        {
            this.ID = id;
            this.P1 = p1;
            this.P2 = p2;
        }

        public string ID { get; set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
    }

    public class Ray
    {
        public Ray(string id, Point p1, Point p2)
        {
            this.ID = id;
            this.P1 = p1;
            this.P2 = p2;
        }

        public string ID { get; set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
    }

    public class Segment
    {
        public Segment(string id, Point p1, Point p2)
        {
            this.ID = id;
            this.P1 = p1;
            this.P2 = p2;
        }

        public string ID { get; set; }
        public Point P1 { get; set; }
        public Point P2 { get; set; }
    }
}
