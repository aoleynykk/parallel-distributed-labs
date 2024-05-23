using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace FormulaCalculator.Implementations.laboratory_3
{
    public class FormulaCalculatorLabThree : IFormulaCalculator
    {
        private readonly SampleData _data;
        private readonly int _maxThreadsPerMethod;

        public FormulaCalculatorLabThree(SampleData data, int maxThreadsPerMethod)
        {
            _data = data;
            _maxThreadsPerMethod = maxThreadsPerMethod;
        }

        /// <summary>
        /// D = B * (ME + MZ) - E * (MM + ME)
        /// </summary>
        public double[][] CalcFormulaA()
        {
            // ME + MZ
            var p1 = new double[_data.ME.Length][];
            var p1Thread = new Thread(() =>
            {
                AsyncOperations.SumMatrices(_data.ME, _data.MZ, p1, _maxThreadsPerMethod);
            });
            p1Thread.Start();

            // MM + ME
            var p3 = new double[_data.MM.Length][];
            var p3Thread = new Thread(() =>
            {
                AsyncOperations.SumMatrices(_data.MM, _data.ME, p3, _maxThreadsPerMethod);
            });
            p3Thread.Start();

            p1Thread.Join();
            p3Thread.Join();

            // B * (ME + MZ)
            var p2 = new double[_data.B.Length][];
            var p2Thread = new Thread(() =>
            {
                AsyncOperations.MultiplyMatrices(_data.B, p1, p2, _maxThreadsPerMethod);
            });
            p2Thread.Start();

            // E * (MM + ME)
            var p4 = new double[_data.E.Length][];
            AsyncOperations.MultiplyMatrices(_data.E, p3, p4, _maxThreadsPerMethod);

            p2Thread.Join();

            // B * (ME + MZ) - E * (MM + ME)
            var res = new double[p2.Length][];
            AsyncOperations.SubtractMatrices(p2, p4, res, _maxThreadsPerMethod);
            return res;
        }

        /// <summary>
        /// MA = min(MM) * (ME + MZ) - ME * MM
        /// </summary>
        public double[][] CalcFormulaB()
        {
            // ME + MZ
            var p1 = new double[_data.ME.Length][];
            var p1Thread = new Thread(() =>
            {
                AsyncOperations.SumMatrices(_data.ME, _data.MZ, p1, _maxThreadsPerMethod);
            });
            p1Thread.Start();

            // min(MM) * (ME + MZ)
            var minMM = Operations.GetMinValInMatrix(_data.MM);
            p1Thread.Join();
            var p2 = Operations.MultiplyMatrixByScalar(p1, minMM);

            // ME * MM
            var p3 = new double[_data.ME.Length][];
            var p3Thread = new Thread(() =>
            {
                AsyncOperations.MultiplyMatrices(_data.ME, _data.MM, p3, _maxThreadsPerMethod);
            });
            p3Thread.Start();

            p3Thread.Join();

            // min(MM) * (ME + MZ) - ME * MM
            var result = new double[p2.Length][];
            AsyncOperations.SubtractMatrices(p2, p3, result, _maxThreadsPerMethod);
            return result;
        }

        private static class AsyncOperations
        {
            /// <summary>
            /// Executes an action in parallel within specified range of iterations.
            /// Custom parallel for loop
            /// </summary>
            /// <param name="action">Action/<int/> that executes on every iteration, int represents current index of iteration</param>
            /// <param name="startIndex">Starting index</param>
            /// <param name="endIndex">End index, Action with this value IS NOT EXECUTED, UPPER BOUND</param>
            /// <param name="threadsLimit">Max amount of threads to be used</param>
            private static void ExecuteInParallel(Action<int> action, int startIndex, int endIndex, int threadsLimit)
            {
                var totalWork = endIndex - startIndex;
                var countdown = new CountdownEvent(threadsLimit);
                var maxWorkPerThread = (int)Math.Ceiling((double)totalWork / threadsLimit);

                for (var threadIndex = 0; threadIndex < threadsLimit; threadIndex++)
                {
                    var startWork = startIndex + threadIndex * maxWorkPerThread;
                    var endWork = Math.Min(startWork + maxWorkPerThread, endIndex);

                    ThreadPool.QueueUserWorkItem((obj) =>
                    {
                        for (var workIndex = startWork; workIndex < endWork; workIndex++)
                        {
                            action(workIndex);
                        }
                        countdown.Signal();
                    });
                }

                countdown.Wait();
            }

            public static void MultiplyVectorByMatrix(double[] v, double[][] m, double[] resultVector, int threadsLimit)
            {
                var columns = m[0].Length;
                ExecuteInParallel(colIndex =>
                {
                    double sum = 0.0;
                    double c = 0.0; // Kahan Summation compensation

                    for (int k = 0; k < v.Length; k++)
                    {
                        double y = v[k] * m[k][colIndex] - c;
                        double t = sum + y;
                        c = t - sum - y;
                        sum = t;
                    }

                    resultVector[colIndex] = sum;
                }, 0, columns, threadsLimit);
            }

            public static void MultiplyMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
            {
                var rows = m1.Length;
                ExecuteInParallel(rowIndex =>
                {
                    resultMatrix[rowIndex] = new double[m2[0].Length];

                    for (int j = 0; j < m2[0].Length; j++)
                    {
                        double sum = 0.0;
                        for (int k = 0; k < m1[rowIndex].Length; k++)
                        {
                            sum += m1[rowIndex][k] * m2[k][j];
                        }
                        resultMatrix[rowIndex][j] = sum;
                    }
                }, 0, rows, threadsLimit);
            }

            public static void SubtractMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
            {
                ExecuteInParallel(rowIndex =>
                {
                    resultMatrix[rowIndex] = Operations.SubtractVectors(m1[rowIndex], m2[rowIndex]);
                }, 0, m1.Length, threadsLimit);
            }

            public static void SumMatrices(double[][] m1, double[][] m2, double[][] resultMatrix, int threadsLimit)
            {
                ExecuteInParallel(rowIndex =>
                {
                    resultMatrix[rowIndex] = Operations.SumVectors(m1[rowIndex], m2[rowIndex]);
                }, 0, m1.Length, threadsLimit);
            }
        }

        private static class Operations
        {
            public static double GetMinValInMatrix(double[][] matrix)
            {
                double min = double.MaxValue;
                foreach (var row in matrix)
                {
                    foreach (var val in row)
                    {
                        if (val < min)
                        {
                            min = val;
                        }
                    }
                }
                return min;
            }

            public static double[][] MultiplyMatrixByScalar(double[][] matrix, double scalar)
            {
                var result = new double[matrix.Length][];
                for (int i = 0; i < matrix.Length; i++)
                {
                    result[i] = new double[matrix[i].Length];
                    for (int j = 0; j < matrix[i].Length; j++)
                    {
                        result[i][j] = matrix[i][j] * scalar;
                    }
                }
                return result;
            }

            public static double[] MultiplyVectorByScalar(double[] vector, double scalar)
            {
                var result = new double[vector.Length];
                for (int i = 0; i < vector.Length; i++)
                {
                    result[i] = vector[i] * scalar;
                }
                return result;
            }

            public static double[] SubtractVectors(double[] v1, double[] v2)
            {
                var result = new double[v1.Length];
                for (int i = 0; i < v1.Length; i++)
                {
                    result[i] = v1[i] - v2[i];
                }
                return result;
            }

            public static double[] SumVectors(double[] v1, double[] v2)
            {
                var result = new double[v1.Length];
                for (int i = 0; i < v1.Length; i++)
                {
                    result[i] = v1[i] + v2[i];
                }
                return result;
            }
        }
    }
}
