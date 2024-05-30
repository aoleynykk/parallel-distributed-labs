using BenchmarkDotNet.Attributes;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Implementations.laboratory_2;
using FormulaCalculator.Implementations.laboratory_3;
using FormulaCalculator.Implementations.laboratory_4;
using FormulaCalculator.Implementations.laboratory_5;
using FormulaCalculator.Implementations.laboratory_6;
using FormulaCalculator.Interfaces;
using SampleDataGenerator;

namespace ConsoleApp.Benchmarks;

public class Benchmark
{
    private FormulaCalculatorLabThree _calcL3;
    private FormulaCalculatorLabFour _calcL4;
    private FormulaCalculatorLabFifth _calcL5;
    private FormulaCalculatorLabSixth _calcL6;

    public Benchmark()
    {
        var data = "data.json";
        var sampleData = DataSerializer.Deserialize<SampleData>(Path.Combine("/Users/oleynyk/parallel-distributed-labs/ConsoleApp/Data/", data));

        _calcL3 = new FormulaCalculatorLabThree(sampleData!, 5);
        _calcL4 = new FormulaCalculatorLabFour(sampleData!, 5);
        _calcL5 = new FormulaCalculatorLabFifth(sampleData!, 5);
        _calcL6 = new FormulaCalculatorLabSixth(sampleData!, 5);
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

    [Benchmark]
    public void CalcFormulaALab5()
    {
        _calcL5.CalcFormulaA();
    }

    [Benchmark]
    public void CalcFormulaBLab5()
    {
        _calcL5.CalcFormulaB();
    }

    [Benchmark]
    public void CalcFormulaALab6()
    {
        _calcL6.CalcFormulaA();
    }

    [Benchmark]
    public void CalcFormulaBLab6()
    {
        _calcL6.CalcFormulaB();
    }
}
