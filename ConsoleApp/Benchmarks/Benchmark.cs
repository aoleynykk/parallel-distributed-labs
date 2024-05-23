using BenchmarkDotNet.Attributes;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace ConsoleApp.Benchmarks;

public class Benchmark
{
	private FormulaCalculatorLabOne _calcL1;

	public Benchmark()
	{
		var data = "data-big.json";
      
        var samepleData = DataSerializer.Deserialize<SampleData>(Path.Combine("/Users/oleynyk/parallel-distributed-labs/ConsoleApp/Data/", data));

		_calcL1 = new FormulaCalculatorLabOne(samepleData!);
	}

	[Benchmark]
	public void CalcFormulaALab1()
	{
		_calcL1.CalcFormulaA();
	}

	[Benchmark]
	public void CalcFormulaBLab1()
	{
		_calcL1.CalcFormulaB();
	}
}