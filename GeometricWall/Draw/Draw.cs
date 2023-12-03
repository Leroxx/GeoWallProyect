using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            punto.Height = 4;
            punto.Width = 4;
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

            // Crear el Path para la circunferencia
            Path circunferencia = new Path();
            circunferencia.Stroke = Brushes.Black;
            circunferencia.StrokeThickness = 2;

            // Crear el EllipseGeometry para la circunferencia
            EllipseGeometry ellipseGeometry = new EllipseGeometry();
            ellipseGeometry.Center = new(x, y);
            ellipseGeometry.RadiusX = radio;
            ellipseGeometry.RadiusY = radio;

            // Establecer el Geometry del Path como el EllipseGeometry
            circunferencia.Data = ellipseGeometry;

            DrawPoint(center);
            // Agregar la circunferencia al Canvas
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
            segmento.StrokeThickness = 2;

            DrawPoint(s1.P1);
            DrawPoint(s1.P2);

            // Agregar la circunferencia al Canvas
            MiCanvas.Children.Add(segmento);
        }

        public void DrawLine(Line l1)
        {
            System.Windows.Shapes.Line recta = new();
            recta.Stroke = Brushes.Black;
            recta.StrokeThickness = 2;
            Point p1 = l1.P1;
            Point p2 = l1.P2;

            recta.X1 = p1.X;
            recta.Y1 = p1.Y;

            recta.X2 = p2.X;
            recta.Y2 = p2.Y;

            double factorExtension = 0.2; // Ajusta este valor según tu necesidad

            double deltaX = p2.X - p1.X;
            double deltaY = p2.Y - p1.Y;

            recta.X1 -= deltaX * factorExtension;
            recta.Y1 -= deltaY * factorExtension;
            recta.X2 += deltaX * factorExtension;
            recta.Y2 += deltaY * factorExtension;

            DrawPoint(p1);
            DrawPoint(p2);

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
    }

}
