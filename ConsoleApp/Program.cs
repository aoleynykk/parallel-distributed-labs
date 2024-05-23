using BenchmarkDotNet.Running;
using ConsoleApp;
using ConsoleApp.Benchmarks;
using FormulaCalculator.Implementations.laboratory_2;
using FormulaCalculator.Implementations.laboratory_3;
using SampleDataGenerator;

var data = "data.json";
Console.WriteLine("DataSet:  " + data);
var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/" + data);

var calc2 = new FormulaCalculatorLabTwo(sampleData!, 5);
var calc3 = new FormulaCalculatorLabThree(sampleData!, 5);

Printer.PrintResults(calc2.CalcFormulaA(), 5, "Formula A Lab 2");
Printer.PrintResults(calc2.CalcFormulaB(), 5, "Formula B Lab 2");

Printer.PrintResults(calc3.CalcFormulaA(), 5, "Formula A Lab 3");
Printer.PrintResults(calc3.CalcFormulaB(), 5, "Formula B Lab 3");

BenchmarkRunner.Run<Benchmark>();