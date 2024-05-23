using FormulaCalculator.Interfaces;
using SampleDataGenerator;
using System.Threading.Tasks;

namespace FormulaCalculator.Implementations.laboratory_4
{
    public class FormulaCalculatorLabFour : IFormulaCalculator
    {
        private readonly SampleData _data;
        private readonly int _maxThreadsPerMethod;

        public FormulaCalculatorLabFour(SampleData data, int maxThreadsPerMethod)
        {
            _data = data;
            _maxThreadsPerMethod = maxThreadsPerMethod;
        }

        public double[][] CalcFormulaA()
        {
            var task = CalcFormulaAAsync();
            task.Wait();
            return task.GetAwaiter().GetResult();
        }

        public double[][] CalcFormulaB()
        {
            var task = CalcFormulaBAsync();
            task.Wait();
            return task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// D = B * (ME + MZ) - E * (MM + ME)
        /// </summary>
        private async Task<double[][]> CalcFormulaAAsync()
        {
            // ME + MZ
            var p1 = AsyncOperations.SumMatrices(_data.ME, _data.MZ, _maxThreadsPerMethod);

            // MM + ME
            var p3 = AsyncOperations.SumMatrices(_data.MM, _data.ME, _maxThreadsPerMethod);

            // B * (ME + MZ)
            var p2 = AsyncOperations.MultiplyMatrices(_data.B, await p1, _maxThreadsPerMethod);

            // E * (MM + ME)
            var p4 = AsyncOperations.MultiplyMatrices(_data.E, await p3, _maxThreadsPerMethod);

            // B * (ME + MZ) - E * (MM + ME)
            return await AsyncOperations.SubtractMatrices(await p2, await p4, _maxThreadsPerMethod);
        }

        /// <summary>
        /// MA = min(MM) * (ME + MZ) - ME * MM
        /// </summary>
        private async Task<double[][]> CalcFormulaBAsync()
        {
            // ME + MZ
            var p1 = AsyncOperations.SumMatrices(_data.ME, _data.MZ, _maxThreadsPerMethod);

            // min(MM) * (ME + MZ)
            var minMM = Operations.GetMinValInMatrix(_data.MM);
            var p2 = Operations.MultiplyMatrixByScalar(await p1, minMM);

            // ME * MM
            var p3 = AsyncOperations.MultiplyMatrices(_data.ME, _data.MM, _maxThreadsPerMethod);

            // min(MM) * (ME + MZ) - ME * MM
            return await AsyncOperations.SubtractMatrices(p2, await p3, _maxThreadsPerMethod);
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
            /// <param name="tasksLimit">Max amount of threads to be used</param>
            private static async Task ExecuteInParallel(Action<int> action, int startIndex, int endIndex, int tasksLimit)
            {
                var totalWork = endIndex - startIndex;
                var tasks = new Task[tasksLimit];
                var maxWorkPerTask = (int)Math.Ceiling((double)totalWork / tasksLimit);

                for (var taskIndex = 0; taskIndex < tasksLimit; taskIndex++)
                {
                    var startWork = startIndex + taskIndex * maxWorkPerTask;
                    var endWork = Math.Min(startWork + maxWorkPerTask, endIndex);

                    tasks[taskIndex] = Task.Run(() =>
                    {
                        for (var workIndex = startWork; workIndex < endWork; workIndex++)
                        {
                            action(workIndex);
                        }
                    });
                }

                await Task.WhenAll(tasks);
            }

            public static async Task<double[]> MultiplyVectorByMatrix(double[] v, double[][] m, int tasksLimit)
            {
                var resultVector = new double[m[0].Length];

                await ExecuteInParallel(colIndex =>
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
                }, 0, resultVector.Length, tasksLimit);

                return resultVector;
            }

            public static async Task<double[][]> MultiplyMatrices(double[][] m1, double[][] m2, int tasksLimit)
            {
                var rows = m1.Length;
                var resultMatrix = new double[m1.Length][];

                await ExecuteInParallel(rowIndex =>
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
                }, 0, rows, tasksLimit);

                return resultMatrix;
            }

            public static async Task<double[][]> SubtractMatrices(double[][] m1, double[][] m2, int tasksLimit)
            {
                var resultMatrix = new double[m1.Length][];

                await ExecuteInParallel(rowIndex =>
                {
                    resultMatrix[rowIndex] = Operations.SubtractVectors(m1[rowIndex], m2[rowIndex]);
                }, 0, m1.Length, tasksLimit);

                return resultMatrix;
            }

            public static async Task<double[][]> SumMatrices(double[][] m1, double[][] m2, int tasksLimit)
            {
                var resultMatrix = new double[m1.Length][];

                await ExecuteInParallel(rowIndex =>
                {
                    resultMatrix[rowIndex] = Operations.SumVectors(m1[rowIndex], m2[rowIndex]);
                }, 0, m1.Length, tasksLimit);

                return resultMatrix;
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
