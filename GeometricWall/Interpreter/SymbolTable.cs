using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometricWall
{
    public class SymbolTable : NodeVisitor
    {
        public enum VariableType
        {
            Double,
            String,
            Bool,
            Circle,
            Line,
            Segment,
            Ray,
            Point
        }

        // Stack to store variable declarations in each environment
        private Stack<Dictionary<string, Tuple<object, VariableType>>> SymbolStack;

        public SymbolTable()
        {
            SymbolStack = new Stack<Dictionary<string, Tuple<object, VariableType>>>();
        }

        public void PushTable()
        {
            SymbolStack.Push(new Dictionary<string, Tuple<object, VariableType>>());
        }

        // Deletes an environment
        public Dictionary<string, Tuple<object, VariableType>> PopTable()
        {
            if (SymbolStack.Count > 0)
            {
                return SymbolStack.Pop();
            }
            else
                throw new Exception("Error");
        }

        // If an environment has been created, then add the variable declaration to the stack.
        public void AddSymbol(string name, object value, VariableType type)
        {
            if (SymbolStack.Count > 0)
            {
                SymbolStack.Peek()[name] = Tuple.Create(value, type);
            }
            else
                throw new Exception("Error");
        }

        public Tuple<object, VariableType> GetSymbol(string name)
        {
            foreach (var table in SymbolStack)
            {
                if (table.ContainsKey(name))
                    return table[name];
            }

            throw new ArgumentException("'" + name + "' has not been declared");
        }
    }
}
