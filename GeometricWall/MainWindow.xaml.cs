using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeometricWall
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Compilar_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Background = Brushes.AliceBlue;
            myCanvas.Children.Clear();
            string text = myTextBox.Text;

            if (text != "")
            {
                Lexer lexer = new Lexer(text);
                Parser parser = new Parser(lexer);
                SymbolTable symbolTable = new SymbolTable();
                Draw draw = new Draw(myCanvas);
                Interpreter interpreter = new Interpreter(parser, symbolTable, draw);
                var result = interpreter.Interpret();
            }
        }

        private void Nuevo_Click(object sender, RoutedEventArgs e)
        {
            myCanvas.Children.Clear();
            myTextBox.Text = "";
        }

        public void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
