using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_6
{
    public class FormulaCalculatorLabSixth : IFormulaCalculator
    {
        private readonly SampleData _data;
        private readonly int _maxDegreeOfParallelism;
        private readonly BlockingCollection<double[][]> _queue;

        public FormulaCalculatorLabSixth(SampleData data, int maxDegreeOfParallelism)
        {
            _data = data;
            _maxDegreeOfParallelism = maxDegreeOfParallelism;
            _queue = new BlockingCollection<double[][]>();
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
            // Producer Task
            var producerTask = Task.Factory.StartNew(() =>
            {
                var mePlusMz = Operations.SumMatrices(_data.ME, _data.MZ);
                var mmPlusMe = Operations.SumMatrices(_data.MM, _data.ME);
                _queue.Add(mePlusMz);
                _queue.Add(mmPlusMe);
                _queue.CompleteAdding();
            });

            // Consumer Task
            var consumerTask = Task.Factory.StartNew(() =>
            {
                var mePlusMz = _queue.Take();
                var mmPlusMe = _queue.Take();

                var bTimesMePlusMz = Operations.MultiplyMatrices(_data.B, mePlusMz);
                var eTimesMmPlusMe = Operations.MultiplyMatrices(_data.E, mmPlusMe);

                var result = Operations.SubtractMatrices(bTimesMePlusMz, eTimesMmPlusMe);
                return result;
            });

            await producerTask;
            var result = await consumerTask;

            return result;
        }

        private async Task<double[][]> CalcFormulaBAsync()
        {
            // Producer Task
            var producerTask = Task.Factory.StartNew(() =>
            {
                var mePlusMz = Operations.SumMatrices(_data.ME, _data.MZ);
                _queue.Add(mePlusMz);
                _queue.CompleteAdding();
            });

            // Consumer Task
            var consumerTask = Task.Factory.StartNew(() =>
            {
                var mePlusMz = _queue.Take();

                var minMm = Operations.GetMinValInMatrix(_data.MM);
                var minMmTimesMePlusMz = Operations.MultiplyMatrixByScalar(mePlusMz, minMm);
                var meTimesMm = Operations.MultiplyMatrices(_data.ME, _data.MM);

                var result = Operations.SubtractMatrices(minMmTimesMePlusMz, meTimesMm);
                return result;
            });

            await producerTask;
            var result = await consumerTask;

            return result;
        }

        private static class Operations
        {
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
                        result[i][j] = sum;
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
                    result[i] = sum;
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

            public static double GetMinValInMatrix(double[][] matrix)
            {
                return matrix.Min(row => row.Min());
            }
        }
    }
}
