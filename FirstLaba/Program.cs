using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{
    class Algebra
    {
        static double Delta;
        static Expression<Func<double, double>> First;
        static Expression<Func<double, double>> Second;
        static Expression<Func<double, double>> Third;

        static double SecondAlgoritm(double x)
        {
            var nextX = x - First.Compile()(x) / Second.Compile()(x);
            return Math.Abs(x - nextX) < Delta ? nextX : SecondAlgoritm(nextX);
        }

        static double GetInitial(double a, double b)
        {
            return First.Compile()(a) * Third.Compile()(a) > 0 ? a : b;
        }

        static void Main(string[] args)
        {
            First = x => x*x*x+x-1;
            Second = Differentiate(First);
            Third = Differentiate(Second);
            Console.WriteLine("Введите погрешность:");
            Delta = double.Parse(Console.ReadLine().Replace(".", ","));
            Console.WriteLine("Введите начало интервала:");
            var a = double.Parse(Console.ReadLine().Replace(".", ","));
            Console.WriteLine("Введите конец интервала:");
            var b = double.Parse(Console.ReadLine().Replace(".", ","));
            Console.WriteLine("Ответ {0}", SecondAlgoritm(GetInitial(a, b)));
            Console.ReadKey();
        }

        private static int SecondAlgo(double a, double b) => throw new NotImplementedException();

        internal static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
        {
            return Expression.Lambda<Func<double, double>>(DifferentiateFunc(function.Body), function.Parameters);
        }

        private static Expression DifferentiateFunc(Expression body)
        {
            if (body is ConstantExpression)
                return Expression.Constant(0.0);
            if (body is ParameterExpression)
                return Expression.Constant(1.0);

            if (body is BinaryExpression binaryExpression)
            {
                if (body.NodeType == ExpressionType.Add)
                    return Expression.Add(DifferentiateFunc(binaryExpression.Left), DifferentiateFunc(binaryExpression.Right));
                if (body.NodeType == ExpressionType.Multiply)
                {
                    return Expression.Add(Expression.Multiply(DifferentiateFunc(binaryExpression.Left), binaryExpression.Right),
                        Expression.Multiply(binaryExpression.Left, DifferentiateFunc(binaryExpression.Right)));
                }
                if (body.NodeType == ExpressionType.Subtract)
                {
                    return Expression.Subtract(DifferentiateFunc(binaryExpression.Left), DifferentiateFunc(binaryExpression.Right));
                }
            }
            throw new ArgumentException("Неизвестная функция");
        }
    }
}
