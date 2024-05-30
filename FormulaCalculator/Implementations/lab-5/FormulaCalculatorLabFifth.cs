using System;
using System.Linq;
using System.Threading.Tasks;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_5
{
    public class FormulaCalculatorLabFifth : IFormulaCalculator
    {
        private readonly SampleData _data;
        private readonly int _maxDegreeOfParallelism;

        public FormulaCalculatorLabFifth(SampleData data, int maxDegreeOfParallelism)
        {
            _data = data;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public double[][] CalcFormulaA()
        {
            var task = CalcFormulaAAsync();
            task.Wait();
            return task.Result;
        }

        public double[][] CalcFormulaB()
        {
            var task = CalcFormulaBAsync();
            task.Wait();
            return task.Result;
        }

        private async Task<double[][]> CalcFormulaAAsync()
        {
            // ME + MZ
            var mePlusMzTask = Task.Factory.StartNew(() =>
                Operations.SumMatrices(_data.ME, _data.MZ), TaskCreationOptions.LongRunning);

            // MM + ME
            var mmPlusMeTask = Task.Factory.StartNew(() =>
                Operations.SumMatrices(_data.MM, _data.ME), TaskCreationOptions.LongRunning);

            var mePlusMz = await mePlusMzTask;
            var mmPlusMe = await mmPlusMeTask;

            // B * (ME + MZ)
            var bTimesMePlusMzTask = Task.Factory.StartNew(() =>
                Operations.MultiplyMatrices(_data.B, mePlusMz), TaskCreationOptions.LongRunning);

            // E * (MM + ME)
            var eTimesMmPlusMeTask = Task.Factory.StartNew(() =>
                Operations.MultiplyMatrices(_data.E, mmPlusMe), TaskCreationOptions.LongRunning);

            var bTimesMePlusMz = await bTimesMePlusMzTask;
            var eTimesMmPlusMe = await eTimesMmPlusMeTask;

            // D = B * (ME + MZ) - E * (MM + ME)
            var result = Operations.SubtractMatrices(bTimesMePlusMz, eTimesMmPlusMe);

            return result;
        }

        private async Task<double[][]> CalcFormulaBAsync()
        {
            // ME + MZ
            var mePlusMzTask = Task.Factory.StartNew(() =>
                Operations.SumMatrices(_data.ME, _data.MZ), TaskCreationOptions.LongRunning);

            var mePlusMz = await mePlusMzTask;

            // min(MM)
            var minMm = Operations.GetMinValInMatrix(_data.MM);

            // min(MM) * (ME + MZ)
            var minMmTimesMePlusMzTask = Task.Factory.StartNew(() =>
                Operations.MultiplyMatrixByScalar(mePlusMz, minMm), TaskCreationOptions.LongRunning);

            var minMmTimesMePlusMz = await minMmTimesMePlusMzTask;

            // ME * MM
            var meTimesMmTask = Task.Factory.StartNew(() =>
                Operations.MultiplyMatrices(_data.ME, _data.MM), TaskCreationOptions.LongRunning);

            var meTimesMm = await meTimesMmTask;

            // MA = min(MM) * (ME + MZ) - ME * MM
            var result = Operations.SubtractMatrices(minMmTimesMePlusMz, meTimesMm);

            return result;
        }

        private static class Operations
        {
            private static readonly object LockObject = new object();

            public static double[][] SubtractMatrices(double[][] m1, double[][] m2)
            {
                var result = new double[m1.Length][];
                Parallel.For(0, m1.Length, i =>
                {
                    result[i] = SubtractVectors(m1[i], m2[i]);
                });
                return result;
            }

            public static double[] SubtractVectors(double[] v1, double[] v2)
            {
                return v1.Zip(v2, (a, b) => a - b).ToArray();
            }

            public static double[][] MultiplyMatrices(double[][] m1, double[][] m2)
            {
                var result = new double[m1.Length][];
                Parallel.For(0, m1.Length, i =>
                {
                    result[i] = new double[m2[0].Length];
                    for (int j = 0; j < m2[0].Length; j++)
                    {
                        double sum = 0;
                        for (int k = 0; k < m1[0].Length; k++)
                        {
                            sum += m1[i][k] * m2[k][j];
                        }
                        lock (LockObject)
                        {
                            result[i][j] = sum;
                        }
                    }
                });
                return result;
            }

            public static double[] MultiplyVectorByMatrix(double[] v, double[][] m)
            {
                var result = new double[m[0].Length];
                Parallel.For(0, m[0].Length, i =>
                {
                    double sum = 0;
                    for (int j = 0; j < v.Length; j++)
                    {
                        sum += v[j] * m[j][i];
                    }
                    lock (LockObject)
                    {
                        result[i] = sum;
                    }
                });
                return result;
            }

            public static double[] MultiplyVectorByScalar(double[] v, double scalar)
            {
                return v.Select(x => x * scalar).ToArray();
            }

            public static double[][] MultiplyMatrixByScalar(double[][] m, double scalar)
            {
                var result = new double[m.Length][];
                Parallel.For(0, m.Length, i =>
                {
                    result[i] = m[i].Select(x => x * scalar).ToArray();
                });
                return result;
            }

            public static double[][] SumMatrices(double[][] m1, double[][] m2)
            {
                var result = new double[m1.Length][];
                Parallel.For(0, m1.Length, i =>
                {
                    result[i] = SumVectors(m1[i], m2[i]);
                });
                return result;
            }

            public static double[] SumVectors(double[] v1, double[] v2)
            {
                return v1.Zip(v2, (a, b) => a + b).ToArray();
            }

            public static double GetMinValInMatrix(double[][] m)
            {
                double minVal = double.MaxValue;
                Parallel.ForEach(m, row =>
                {
                    double rowMin = row.Min();
                    lock (LockObject)
                    {
                        if (rowMin < minVal)
                        {
                            minVal = rowMin;
                        }
                    }
                });
                return minVal;
            }
        }
    }
}