using BenchmarkDotNet.Running;
using ConsoleApp;
using ConsoleApp.Benchmarks;
using FormulaCalculator.Implementations.laboratory_1;
using FormulaCalculator.Implementations.laboratory_2;
using SampleDataGenerator;

var data = "data.json";
Console.WriteLine("DataSet:  " + data);
var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/" + data);
var calc1 = new FormulaCalculatorLabOne(sampleData!);
var calc2 = new FormulaCalculatorLabTwo(sampleData!, 5);

Printer.PrintResults(calc1.CalcFormulaA(), 5, "Formula A Lab 1");
Printer.PrintResults(calc1.CalcFormulaB(), 5, "Formula B Lab 1");

Printer.PrintResults(calc2.CalcFormulaA(), 5, "Formula A Lab 2");
Printer.PrintResults(calc2.CalcFormulaB(), 5, "Formula B Lab 2");

BenchmarkRunner.Run<Benchmark>();