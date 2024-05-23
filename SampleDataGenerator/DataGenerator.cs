namespace SampleDataGenerator;

/// <summary>
/// Generates matrices, vectors and scalars with random values.
/// This class is intended to be used with doubles
/// </summary>
public class DataGenerator
{
	private Random _random;

	public DataGenerator(Random random)
	{
		_random = random;
	}

	public DataGenerator()
	{
		_random = new Random();
	}

	public double[][] GenerateMatrix(int rows, int cols, double minVal = 0, double maxVal = 1)
	{
		var matrix = new double[rows][];

		for (var r = 0; r < rows; r++)
		{
			matrix[r] = new double[cols];
			for (var c = 0; c < cols; c++)
			{
				matrix[r][c] = GetRandomDouble(minVal, maxVal);
			}
		}

		return matrix;
	}

	public double[] GenerateVector(int dims, double minVal = 0, double maxVal = 1)
	{
		var vector = new double[dims];

		for (var d = 0; d < dims; d++)
		{
			vector[d] = GetRandomDouble(minVal,maxVal);
		}

		return vector;
	}

	public double GenerateScalar(double minVal = 0, double maxVal = 1)
	{
		return GetRandomDouble(minVal, maxVal);
	}

	private double GetRandomDouble(double minVal, double maxVal)
	{
		return _random.NextDouble() * (maxVal - minVal) + minVal;
	}
}