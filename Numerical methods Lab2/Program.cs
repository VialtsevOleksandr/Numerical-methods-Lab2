using System;
using System.Dynamic;
using System.Net.Http;
using System.Numerics;
using System.Text;
using System.Web;
using System.Xml.Linq;

//  { 1, 3, -2, 0, -2 },
//  { 3, 4, -5, 1, -3 },
//  { -2, -5, 3, -2, 2 },
//  { 0, 1, -2, 5, 3 },
//  { -2, -3, 2, 3, 4 }

//  { 0.5, 5.4, 5, 7.5, 3.3 }

class Program
{

    static void ConsoleClear((int Left, int Top) position)
    {
        Console.Write("Коректно введіть число!");
        Console.SetCursorPosition(position.Left, position.Top);
        Console.Write("                       ");
        Console.SetCursorPosition(position.Left, position.Top);
    }
    static void GetInfo()
    {
        Console.WriteLine("Натисніть:\n" +
           "1 -- для методу квадратного кореня\n" +
           "2 -- для методу Зейделя\n" +
           "3 -- для очищення консолі\n" +
           "4 -- для виходу\n" +
           "5 -- для перегляду інформації\n");
    }

    static double GetE()
    {
        double ε;
        Console.Write("Вкажiть точнiсть ε з проміжку (0;1).       ε = ");
        var position = Console.GetCursorPosition();
        bool res = false;
        do
        {
            res = double.TryParse(Console.ReadLine(), out ε);
            if (!res || ε <= 0 || ε >= 1)
            {
                ConsoleClear(position);
            }
        } while (!res || ε <= 0 || ε >= 1);
        {
            position = Console.GetCursorPosition();
            Console.Write("                       ");
            Console.SetCursorPosition(position.Left, position.Top);
        }
        return ε;
    }

    static void PrintMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (matrix[i, j] == 0)
                    Console.Write("0\t");
                else
                    Console.Write(matrix[i, j].ToString("F2") + "\t");
            }
            Console.WriteLine();
        }
    }

    static double Determinant(double[,] S, double[,] D)
    {
        int n = S.GetLength(0);
        double det = 1;
        for (int i = 0; i < n; i++)
            det *= D[i, i] * Math.Pow(S[i, i], 2);
        return det;
    }
    static double[,] MultiplyTwoMatrix(double[,] matrix1, double[,] matrix2)
    {
        int n = matrix1.GetLength(0);
        double[,] result = new double[n, n];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
            {
                double sum = 0;
                for (int k = 0; k < n; k++)
                    sum += matrix1[i, k] * matrix2[k, j];
                result[i, j] = sum;
            }
        return result;
    }
    static double[] FindY(double[,] matrix, double[] vector)
    {
        int n = vector.Length;
        double[] result = new double[n];
        for (int i = 0; i < n; i++)
        {
            result[i] = vector[i];
            for (int j = 0; j < i; j++)
            {
                result[i] -= matrix[i, j] * result[j];
            }
            result[i] /= matrix[i, i];
        }
        return result;
    }
    static double[] FindX(double[,] matrix, double[] vector)
    {
        int n = vector.Length;
        double[] result = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            result[i] = vector[i];
            for (int j = n - 1; j > i; j--)
            {
                result[i] -= matrix[i, j] * result[j];
            }
            result[i] /= matrix[i, i];
        }
        return result;
    }

    static double[,] ReversMatrixA(double[,] S, double[,] D, double[,] St)
    {
        int n = S.GetLength(0);
        double[,] result = new double[n, n];
        for (int i = 0; i < n; i++)
        {
            double[] e = new double[n];
            e[i] = 1;
            double[] y = FindY(MultiplyTwoMatrix(St, D), e);
            double[] x = FindX(S, y);
            for (int j = 0; j < n; j++)
                result[j, i] = x[j];
        }
        return result;
    }

    static double NormMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        double max = 0;
        for (int i = 0; i < n; i++)
        {
            double sum = 0;
            for (int j = 0; j < n; j++)
                sum += Math.Abs(matrix[i, j]);
            if (sum > max)
                max = sum;
        }
        return max;
    }
    
    static bool CheckTransposeMatrix(double[,] matrix)
    {
        int n = matrix.GetLength(0);
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (matrix[i, j] != matrix[j, i])
                {
                    Console.WriteLine("At != A, а отже не можемо застосувати метод Зейделя.");
                    return false;
                }
        Console.WriteLine("At = A, можемо застосувати метод Зейделя.");
        return true;
    }

    static bool CheckMinor(double[,] S, double[,] D)
    {
        int n = S.GetLength(0);
        double[] minors = new double[n];
        minors[0] = D[0, 0] * Math.Pow(S[0, 0], 2);
        for (int k = 1; k < n; k++)
        {
            double det = 1;
            for (int i = 0; i <= k; i++)
            {
                det *= D[i, i] * Math.Pow(S[i, i], 2);
            }
            minors[k] = det;
        }
        for (int i = 0; i < n; i++)
        {
            Console.Write($"Мінор {i + 1}: {minors[i]:F2}\t");
        }
        Console.WriteLine();
        for (int i = 0; i < n; i++)
        {
            if (minors[i] <= 0)
            {
                Console.WriteLine("Не всі мінори додатні, а отже не можемо застосувати метод Зейделя.");
                return false;
            }
        }
        Console.WriteLine("Всі мінори додатні, можемо застосувати метод Зейделя.");
        return true;
    }
    static void Seidel(double[,] A, double[] b, double[] x, int maxIterations, double tolerance)
    {
        int n = b.Length;
        double[] xOld = new double[n];

        for (int k = 0; k < maxIterations; k++)
        {
            Array.Copy(x, xOld, n); // Зберігаємо попередні значення

            for (int i = 0; i < n; i++)
            {
                double sum1 = 0, sum2 = 0;

                // Сума для j = 1, ..., i - 1 (раніше вже знайдені значення x_j)
                for (int j = 0; j < i; j++)
                {
                    sum1 += A[i, j] * x[j];
                }

                // Сума для j = i + 1, ..., n (використовуємо значення з попередньої ітерації)
                for (int j = i + 1; j < n; j++)
                {
                    sum2 += A[i, j] * xOld[j];
                }

                // Обчислення x_i на k+1 ітерації
                x[i] = (b[i] - sum1 - sum2) / A[i, i];
            }

            // Перевірка на збіжність
            double[] converge = new double[n];
            for (int i = 0; i < n; i++)
            {
                converge[i] = Math.Abs(x[i] - xOld[i]);
            }

            if (converge.Max() < tolerance)
            {
                Console.WriteLine($"Метод збігся на {k + 1}-й ітерації");
                Console.WriteLine("Вектор x:");
                for (int i = 0; i < n; i++)
                    Console.WriteLine(x[i]);
                Console.WriteLine();
                break;
            }
        }
    }

    static void SquareRoots(double[,] A, double[,] S, double[,] D, double[,] St, int n)
    {
        // Обчислення матриць S, St і D за методом квадратного кореня
        for (int i = 0; i < n; i++)
        {
            // Обчислення елементів діагональної матриці D
            double sum = 0;
            for (int p = 0; p <= i - 1; p++)
                sum += Math.Pow(S[p, i], 2) * D[p, p];

            D[i, i] = Math.Sign(A[i, i] - sum);

            // Обчислення діагональних елементів матриці S і St
            S[i, i] = Math.Sqrt(Math.Abs(A[i, i] - sum));
            St[i, i] = S[i, i];

            // Обчислення недіагональних елементів матриці S і St
            for (int j = i + 1; j <= n - 1; j++)
            {
                sum = 0;
                for (int p = 0; p <= i - 1; p++)
                    sum += S[p, i] * S[p, j] * D[p, p];

                S[i, j] = (A[i, j] - sum) / (S[i, i] * D[i, i]);
                St[j, i] = S[i, j];
            }
        }
    }
    // Метод для виведення вектора
    static void PrintVector(double[] vector)
    {
        for (int i = 0; i < vector.Length; i++)
            Console.WriteLine(vector[i].ToString("F2"));
        Console.WriteLine();
    }
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        GetInfo();
        char choice;
        do
        {
            Console.Write("Обрана клавіша -- ");
            var position = Console.GetCursorPosition();
            choice = Console.ReadKey().KeyChar;
            Console.WriteLine();
            switch (choice)
            {
                case '1':
                    Console.WriteLine("---------------Метод квадратного кореня---------------");
                    {
                        int n = 5;
                        double[,] A = new double[5, 5]
                        {
                          { 1, 3, -2, 0, -2 },
                          { 3, 4, -5, 1, -3 },
                          { -2, -5, 3, -2, 2 },
                          { 0, 1, -2, 5, 3 },
                          { -2, -3, 2, 3, 4 }
                        };
                        double[] b = new double[5] { 0.5, 5.4, 5, 7.5, 3.3 };

                        // Ініціалізація матриць S, St і D
                        double[,] S = new double[n, n];
                        double[,] D = new double[n, n];
                        double[,] St = new double[n, n];

                        SquareRoots(A, S, D, St, n);

                        double[] x = FindX(S, FindY(MultiplyTwoMatrix(St, D), b));
                        Console.WriteLine("Вектор x:");
                        PrintVector(x);
                        ////Для перегляду матриць
                        //Console.WriteLine("Матриця S:");
                        //PrintMatrix(S);

                        //Console.WriteLine("Матриця D:");
                        //PrintMatrix(D);

                        //Console.WriteLine("Матриця St:");
                        //PrintMatrix(St);
                        Console.WriteLine("Детермінант: " + Determinant(S, D).ToString("F2"));
                        Console.WriteLine();

                        Console.WriteLine("Обернена матриця А:");
                        double[,] A1 = ReversMatrixA(S, D, St);
                        PrintMatrix(A1);
                        Console.WriteLine();

                        Console.WriteLine("Число обумовленості = " + (NormMatrix(A) * NormMatrix(A1)).ToString("F2"));
                        Console.WriteLine();

                        CheckMinor(S, D);
                        Console.WriteLine();
                    }
                    break;
                case '2':
                    Console.WriteLine("---------------Метод Зейделя---------------");
                    {
                        int n = 4;
                        double[,] A = new double[4, 4]
                             {
                                { 4, -1, 1, 0 },
                                { -1, 4, -1, 1 },
                                { 1, -1, 4, -1 },
                                { 0, 1, -1, 4 }
                             };
                        double[] b = new double[4] { 1, 1, 1, 1 };
                        double[] x0 = new double[n];
                        int maxIterations = 100;
                        double ε = GetE();
                        // Ініціалізація матриць S, St і D
                        double[,] S = new double[n, n];
                        double[,] D = new double[n, n];
                        double[,] St = new double[n, n];
                        CheckTransposeMatrix(A);
                        Console.WriteLine();

                        SquareRoots(A, S, D, St, n);

                        CheckMinor(S, D);
                        Console.WriteLine();

                        Seidel(A, b, x0, maxIterations, ε);

                        double[] x = FindX(S, FindY(MultiplyTwoMatrix(St, D), b));
                        Console.WriteLine("Вектор x знайдений методом квадратних коренів:");
                        for (int i = 0; i < n; i++)
                            Console.WriteLine(x[i]);
                        Console.WriteLine();
                    }
                    break;
                case '3':
                    Console.Clear();
                    GetInfo();
                    break;
                case '4':
                    Console.WriteLine("---------------Вихід---------------");
                    break;
                case '5':
                    GetInfo();
                    break;
                default:
                    Console.SetCursorPosition(position.Left, position.Top);
                    Console.WriteLine("                     ");
                    Console.SetCursorPosition(0, position.Top);
                    break;
            }
        } while (choice != '4');
    }
}
