using BenchmarkDotNet.Attributes;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Implementations.laboratory_2;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace ConsoleApp.Benchmarks;

public class Benchmark
{
    private FormulaCalculatorLabOne _calcL1;
    private FormulaCalculatorLabTwo _calcL2;

    public Benchmark()
    {
        var data = "data.json";
        var samepleData = DataSerializer.Deserialize<SampleData>(Path.Combine("/Users/oleynyk/parallel-distributed-labs/ConsoleApp/Data/", data));

        _calcL1 = new FormulaCalculatorLabOne(samepleData!);
        _calcL2 = new FormulaCalculatorLabTwo(samepleData!, 5);
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

    [Benchmark]
    public void CalcFormulaALab2()
    {
        _calcL2.CalcFormulaA();
    }

    [Benchmark]
    public void CalcFormulaBLab2()
    {
        _calcL2.CalcFormulaB();
    }
}