namespace ConsoleApp;

public static class Printer
{
	public static void PrintResults(double[][] res, int limitChars, string additionalText)
	{
		PrintResults(res[0], limitChars, additionalText);
	}

	public static void PrintResults(double[] res, int limitChars, string additionalText)
	{
		Console.WriteLine(additionalText);
		var output = "";

		for(var i = 0; i < limitChars && i < res.Length; i++)
		{
			output += res[i] + " ";
		}

		Console.WriteLine(output);
	}
}