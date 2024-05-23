using BenchmarkDotNet.Attributes;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Implementations.laboratory_2;
using FormulaCalculator.Implementations.laboratory_3;
using FormulaCalculator.Implementations.laboratory_4;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace ConsoleApp.Benchmarks;

public class Benchmark
{
    private FormulaCalculatorLabThree _calcL3;
    private FormulaCalculatorLabFour _calcL4;

    public Benchmark()
    {
        var data = "data.json";
        var sampleData = DataSerializer.Deserialize<SampleData>(Path.Combine("/Users/oleynyk/parallel-distributed-labs/ConsoleApp/Data/", data));

        _calcL3 = new FormulaCalculatorLabThree(sampleData!, 5);
        _calcL4 = new FormulaCalculatorLabFour(sampleData!, 5);
    }

    [Benchmark]
    public void CalcFormulaALab3()
    {
        _calcL3.CalcFormulaA();
    }

    [Benchmark]
    public void CalcFormulaBLab3()
    {
        _calcL3.CalcFormulaB();
    }

    [Benchmark]
    public void CalcFormulaALab4()
    {
        _calcL4.CalcFormulaA();
    }

    [Benchmark]
    public void CalcFormulaBLab4()
    {
        _calcL4.CalcFormulaB();
    }
}