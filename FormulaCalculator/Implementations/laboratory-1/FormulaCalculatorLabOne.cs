using BenchmarkDotNet.Attributes;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_1;

public class FormulaCalculatorLabOne : IFormulaCalculator
{
    private readonly SampleData _data;

    public FormulaCalculatorLabOne(SampleData data)
    {
        _data = data;
    }
    /// <summary>
    /// D = B*(ME+MZ)-E*(MM+ME)
    /// </summary>
    public double[][] CalcFormulaA()
    {
        // ME + MZ
        var p1 = new double[_data.ME.Length][];
        var p1Thread = new Thread(() =>
        {
            AsyncOperations.SumMatrices(_data.ME, _data.MZ, p1);
        });
        p1Thread.Start();

        // B * (ME + MZ)
        var p2 = new double[_data.B.Length][];
        p1Thread.Join(); // Ensure p1 is calculated before using it
        var p2Thread = new Thread(() =>
        {
            AsyncOperations.MultiplyMatrices(_data.B, p1, p2);
        });
        p2Thread.Start();

        // MM + ME
        var p3 = new double[_data.MM.Length][];
        var p3Thread = new Thread(() =>
        {
            AsyncOperations.SumMatrices(_data.MM, _data.ME, p3);
        });
        p3Thread.Start();

        // E * (MM + ME)
        var p4 = new double[_data.E.Length][];
        p3Thread.Join(); // Ensure p3 is calculated before using it
        AsyncOperations.MultiplyMatrices(_data.E, p3, p4);

        p2Thread.Join();

        // B * (ME + MZ) - E * (MM + ME)
        var res = new double[p2.Length][];
        AsyncOperations.SubtractMatrices(p2, p4, res);
        return res;
    }

    /// <summary>
    /// MA = min(MM)*(ME+MZ)-ME*MM
    /// </summary>
    public double[][] CalcFormulaB()
    {
        // ME + MZ
        var p1 = new double[_data.ME.Length][];
        var p1Thread = new Thread(() =>
        {
            AsyncOperations.SumMatrices(_data.ME, _data.MZ, p1);
        });
        p1Thread.Start();

        // min(MM) * (ME + MZ)
        var minMM = Operations.GetMinValInMatrix(_data.MM);
        p1Thread.Join(); // Ensure p1 is calculated before using it
        var p2 = Operations.MultiplyMatrixByScalar(p1, minMM);

        // ME * MM
        var p3 = new double[_data.ME.Length][];
        var p3Thread = new Thread(() =>
        {
            AsyncOperations.MultiplyMatrices(_data.ME, _data.MM, p3);
        });
        p3Thread.Start();

        p3Thread.Join();

        // min(MM) * (ME + MZ) - ME * MM
        var result = new double[p2.Length][];
        AsyncOperations.SubtractMatrices(p2, p3, result);
        return result;
    }

    private static class AsyncOperations
    {
        public static void MultiplyMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
        {
            var threads = new Thread[m1.Length];

            for (var i = 0; i < m1.Length; i++)
            {
                resultMatrix[i] = new double[m2[0].Length];

                var localI = i;
                threads[i] = new Thread(() =>
                {
                    for (var j = 0; j < m2[0].Length; j++)
                    {
                        resultMatrix[localI][j] = 0;

                        for (var k = 0; k < m1[localI].Length; k++)
                        {
                            resultMatrix[localI][j] += m1[localI][k] * m2[k][j];
                        }
                    }
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        public static void SumMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
        {
            var threads = new Thread[m1.Length];

            for (var i = 0; i < m1.Length; i++)
            {
                var localI = i;

                threads[i] = new Thread(() =>
                {
                    resultMatrix[localI] = Operations.SumVectors(m1[localI], m2[localI]);
                });

                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        public static void SubtractMatrices(double[][] m1, double[][] m2, double[][] resultMatrix)
        {
            var threads = new Thread[m1.Length];

            for (var i = 0; i < m1.Length; i++)
            {
                var localI = i;

                threads[i] = new Thread(() =>
                {
                    resultMatrix[localI] = Operations.SubtractVectors(m1[localI], m2[localI]);
                });

                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
