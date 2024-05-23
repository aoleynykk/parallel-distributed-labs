using BenchmarkDotNet.Attributes;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Implementations.laboratory_2;
using FormulaCalculator.Implementations.laboratory_3;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace ConsoleApp.Benchmarks;


public class Benchmark
{
    private FormulaCalculatorLabTwo _calcL2;
    private FormulaCalculatorLabThree _calcL3;

    public Benchmark()
    {
        var data = "data.json";
        var samepleData = DataSerializer.Deserialize<SampleData>(Path.Combine("/Users/oleynyk/parallel-distributed-labs/ConsoleApp/Data/", data));


        _calcL2 = new FormulaCalculatorLabTwo(samepleData!, 5);
        _calcL3 = new FormulaCalculatorLabThree(samepleData!, 5);
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
}