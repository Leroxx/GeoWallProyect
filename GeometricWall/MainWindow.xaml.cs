using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
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
                try
                {
                    Lexer lexer = new Lexer(text);
                    Parser parser = new Parser(lexer);
                    SymbolTable symbolTable = new SymbolTable();
                    Draw draw = new Draw(myCanvas);
                    Interpreter interpreter = new Interpreter(parser, symbolTable, draw);
                    var result = interpreter.Interpret();
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                    {
                        Exception? original = ex;

                        while (original is TargetInvocationException targetInvocationException)
                        {
                            original = targetInvocationException.InnerException;
                        }

                        if (original is not null)
                        {

                            MessageBox.Show("Semantic Error: " + original.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

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

        private void Cargar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos personalizados (*.geo)|*.geo";
            openFileDialog.Title = "Cargar archivo";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                string texto = File.ReadAllText(filePath);
                myTextBox.Text = texto;
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivos personalizados (*.geo)|*.geo";
            saveFileDialog.Title = "Guardar archivo";

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string texto = myTextBox.Text;
                File.WriteAllText(filePath, texto);
            }
        }
    }
}
