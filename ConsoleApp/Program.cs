using BenchmarkDotNet.Running;
using ConsoleApp;
using ConsoleApp.Benchmarks;
using FormulaCalculator.Implementations.laboratory_1;
using SampleDataGenerator;

var data = "data-big.json";
Console.WriteLine("DataSet:  " + data);
var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/" + data);
var calc = new FormulaCalculatorLabOne(sampleData!);

Printer.PrintResults(calc.CalcFormulaA(),5,"Formula A Lab 1");
Printer.PrintResults(calc.CalcFormulaB(),5,"Formula B Lab 1");

BenchmarkRunner.Run<Benchmark>();