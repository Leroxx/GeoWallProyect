using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeometricWall
{
    public class Draw
    {
        public Draw(Canvas miCanvas)
        {
            this.MiCanvas = miCanvas;
        }

        public Canvas MiCanvas { get; set; }

        public void DrawPoint(Point p1)
        {
            Ellipse punto = new Ellipse();
            punto.Height = 3;
            punto.Width = 3;
            punto.Fill = Brushes.Black;

            double x = p1.X;
            double y = p1.Y;

            Canvas.SetLeft(punto, x);
            Canvas.SetTop(punto, y);

            MiCanvas.Children.Add(punto);
        }

        public void DrawCircle(Circle c1)
        {
            double radio = c1.Radio;
            Point center = c1.Center;

            double x = center.X;
            double y = center.Y;

            Ellipse circunferencia = new Ellipse();
            circunferencia.Width = radio * 2;
            circunferencia.Height = radio * 2;
            circunferencia.Stroke = Brushes.Black;
            circunferencia.Fill = Brushes.Transparent;

            Canvas.SetLeft(circunferencia, x - radio);
            Canvas.SetTop(circunferencia, y - radio);

            DrawPoint(center);
            MiCanvas.Children.Add(circunferencia);
        }

        public void DrawSegment(Segment s1)
        {
            System.Windows.Shapes.Line segmento = new();
            segmento.X1 = s1.P1.X;
            segmento.Y1 = s1.P1.Y;
            segmento.X2 = s1.P2.X;
            segmento.Y2 = s1.P2.Y;


            segmento.Stroke = Brushes.Black;
            segmento.StrokeThickness = 1;

            DrawPoint(s1.P1);
            DrawPoint(s1.P2);

            MiCanvas.Children.Add(segmento);
        }

        public void DrawLine(Line l1)
        {
            System.Windows.Shapes.Line recta = new();
            recta.Stroke = Brushes.Black;
            recta.StrokeThickness = 1;
            Point p1 = l1.P1;
            Point p2 = l1.P2;

            recta.X1 = p1.X;
            recta.Y1 = p1.Y;

            recta.X2 = p2.X;
            recta.Y2 = p2.Y;

            double factorExtension = 0.2;

            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;

            recta.X1 -= deltaX * factorExtension;
            recta.Y1 -= deltaY * factorExtension;
            recta.X2 += deltaX * factorExtension;
            recta.Y2 += deltaY * factorExtension;

            MiCanvas.Children.Add(recta);
        }

        public void DrawRay(Ray r1)
        {
            System.Windows.Shapes.Line recta = new();
            recta.Stroke = Brushes.Blue;
            recta.StrokeThickness = 2;
            Point p1 = r1.P1;
            Point p2 = r1.P2;

            recta.X1 = p1.X;
            recta.Y1 = p1.Y;

            recta.X2 = p2.X;
            recta.Y2 = p2.Y;

            double factorExtension = 0.2;

            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;

            recta.X1 -= deltaX * factorExtension;
            recta.Y1 -= deltaY * factorExtension;

            DrawPoint(p1);
            DrawPoint(p2);
            MiCanvas.Children.Add(recta);
        }

        public List<Point> FindCircleIntersections(Circle c1, Circle c2)
        {
            double distance = Math.Sqrt(Math.Pow(c2.Center.X - c1.Center.X, 2) + Math.Pow(c2.Center.Y - c1.Center.Y, 2));

            if (distance > c1.Radio + c2.Radio || distance < Math.Abs(c1.Radio - c2.Radio))
            {
                return new List<Point>();
            }

            double a = (Math.Pow(c1.Radio, 2) - Math.Pow(c2.Radio, 2) + Math.Pow(distance, 2)) / (2 * distance);
            double h = Math.Sqrt(Math.Pow(c1.Radio, 2) - Math.Pow(a, 2));

            double intersectionX1 = c1.Center.X + a * (c2.Center.X - c1.Center.X) / distance;
            double intersectionY1 = c1.Center.Y + a * (c2.Center.Y - c1.Center.Y) / distance;

            double intersectionX2 = intersectionX1 + h * (c2.Center.Y - c1.Center.Y) / distance;
            double intersectionY2 = intersectionY1 - h * (c2.Center.X - c1.Center.X) / distance;

            double intersectionX3 = intersectionX1 - h * (c2.Center.Y - c1.Center.Y) / distance;
            double intersectionY3 = intersectionY1 + h * (c2.Center.X - c1.Center.X) / distance;

            List<Point> intersections = new List<Point>
            {
                new Point("", intersectionX2, intersectionY2),
                new Point("", intersectionX3, intersectionY3)
            };

            return intersections;
        }

        public List<Point> FindLineIntersection(dynamic l1, dynamic l2)
        {
            double x1 = l1.P1.X;
            double y1 = l1.P1.Y;
            double x2 = l1.P2.X;
            double y2 = l1.P2.Y;

            double x3 = l2.P1.X;
            double y3 = l2.P1.Y;
            double x4 = l2.P2.X;
            double y4 = l2.P2.Y;

            double denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denominator == 0)
            {
                throw new InvalidOperationException("Las rectas son paralelas y no se intersecan.");
            }

            double interseccionX = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denominator;
            double interseccionY = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denominator;

            List<Point> intersections = new List<Point>
            {
                new Point("", interseccionX, interseccionY)
            };

            return intersections;
        }

        public List<Point> FindLineCircleIntersection(dynamic l1, Circle c1)
        {
            List<Point> puntosInterseccion = new List<Point>();

            double x1 = l1.P1.X;
            double y1 = l1.P1.Y;
            double x2 = l1.P2.X;
            double y2 = l1.P2.Y;

            double a = Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2);
            double b = 2 * (x1 - c1.Center.X) * (x2 - x1) + 2 * (y1 - c1.Center.Y) * (y2 - y1);
            double c = Math.Pow(x1 - c1.Center.X, 2) + Math.Pow(y1 - c1.Center.Y, 2) - Math.Pow(c1.Radio, 2);

            double discriminante = Math.Pow(b, 2) - 4 * a * c;

            if (discriminante < 0)
            {
                return puntosInterseccion;
            }
            else if (discriminante == 0)
            {
                double t = -b / (2 * a);
                double x = x1 + t * (x2 - x1);
                double y = y1 + t * (y2 - y1);
                puntosInterseccion.Add(new Point("", x, y));
                return puntosInterseccion;
            }
            else
            {
                double t1 = (-b + Math.Sqrt(discriminante)) / (2 * a);
                double X1 = x1 + t1 * (x2 - x1);
                double Y1 = y1 + t1 * (y2 - y1);
                double t2 = (-b - Math.Sqrt(discriminante)) / (2 * a);
                double X2 = x1 + t2 * (x2 - x1);
                double Y2 = y1 + t2 * (y2 - y1);
                puntosInterseccion.Add(new Point("", X1, Y1));
                puntosInterseccion.Add(new Point("", X2, Y2));
                return puntosInterseccion;
            }
        }
    }
}
