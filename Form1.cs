using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void m(object sender, EventArgs e)
        {
            string element = sender.ToString();

            txtInput.Text += element.Substring(element.Length - 1);
           
        }

        private void btnC_Click(object sender, EventArgs e)
        {
            txtInput.Text = "";
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            string copy = txtInput.Text;
            copy = copy.Substring(0, copy.Length - 1);
            txtInput.Text = copy;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        //
        public static string FormatExpression(string expression)
        {
            expression = expression.ToLower().Replace(" ", "");
            expression = Regex.Replace(expression, @"(\+|\-|\*|\/|\%|\^){3,}", match => match.Value[0].ToString());

            expression = Regex.Replace(expression, @"(\+|\-|\*|\/|\%|\^)(\+|\*|\/|\%|\^)", match =>
                match.Value[0].ToString()
            );
            expression = Regex.Replace(expression, @"\+|\-|\*|\/|\%|\^|\)|\(", match =>
                String.Format(" {0} ", match.Value)
            );
            expression = expression.Replace("  ", " ");
            expression = expression.Trim();

            return expression;
        }

        public static int GetPriority(string op)
        {
            if (op == "*" || op == "/" || op == "%" || op == "^")
                return 2;
            if (op == "+" || op == "-")
                return 1;
            return 0;
        }


        public static bool IsOperator(string str)
        {
            return Regex.Match(str, @"^(\+|\-|\*|\/|\%|\^|" + ")$").Success;
        }
        public static bool IsOperand(string str)
        {
            return Regex.Match(str, @"^\d+$|^([a-z]|[A-Z])$").Success;
        }
        private static string ProcessConvert(string[] tokens)
        {
            Stack<string> stack = new Stack<string>();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (IsOperator(token))
                {
                    if ((i == 0) || (i > 0 && (IsOperator(tokens[i - 1]) || tokens[i - 1] == "(")))
                    {
                        if (token == "-")
                        {
                            result.Append(token + tokens[i + 1]).Append(" ");
                            i++;
                        }
                    }
                    else
                    {
                        while (stack.Count > 0 && GetPriority(token) <= GetPriority(stack.Peek()))
                            result.Append(stack.Pop()).Append(" ");
                        stack.Push(token);
                    }
                }

                else if (token == "(")
                    stack.Push(token);
                else if (token == ")")
                {
                    string x = stack.Pop();
                    while (x != "(")
                    {
                        result.Append(x).Append(" ");
                        x = stack.Pop();
                    }
                }
                else// (IsOperand(s))
                {
                    result.Append(token).Append(" ");
                }
            }

            while (stack.Count > 0)
                result.Append(stack.Pop()).Append(" ");

            return result.ToString();
        }
        //

        public static string Infix2Postfix(string infix)
        {
            infix = FormatExpression(infix);

            string[] tokens = infix.Split(' ').ToArray();

            return ProcessConvert(tokens);
        }

        public static double EvaluatePostfix(string postfix)
        {
            return EvaluatePostfix(postfix.Trim().Split(' '));
        }

        private static double EvaluatePostfix(IEnumerable<string> tokens)
        {
            Stack<double> stack = new Stack<double>();

            foreach (string s in tokens)
            {
                if (IsOperator(s))
                {
                    double x = stack.Pop();

                    double y = stack.Pop();

                    switch (s)
                    {
                        case "+": y += x; break;
                        case "-": y -= x; break;
                        case "*": y *= x; break;
                        case "/": y /= x; break;
                        case "%": y %= x; break;
                        case "^": y = Math.Pow(y, x); break;
                        default:
                        throw new Exception("Invalid operator");
                    }
                        stack.Push(y);
                }
                else  // IsOperand
                {
                    stack.Push(double.Parse(s));
                }

            }
            return stack.Pop();
        }
        private void btnEqual_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            string postfix = Infix2Postfix(txtInput.Text);
            //txtResult.Text = CalculatePostfix(postfix);
            txtResult.Text = EvaluatePostfix(postfix)+"";
            

        }
    }
}
 