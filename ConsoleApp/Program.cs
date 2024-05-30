using BenchmarkDotNet.Running;
using ConsoleApp;
using ConsoleApp.Benchmarks;
using FormulaCalculator.Implementations.laboratory_3;
using FormulaCalculator.Implementations.laboratory_4;
using FormulaCalculator.Implementations.laboratory_5;
using FormulaCalculator.Implementations.laboratory_6;
using SampleDataGenerator;

var data = "data.json";
Console.WriteLine("DataSet:  " + data);
var sampleData = DataSerializer.Deserialize<SampleData>("../../../Data/" + data);

var calc3 = new FormulaCalculatorLabThree(sampleData!, 5);
var calc4 = new FormulaCalculatorLabFour(sampleData!, 5);
var calc5 = new FormulaCalculatorLabFifth(sampleData!, 5);
var calc6 = new FormulaCalculatorLabSixth(sampleData!, 5);

Printer.PrintResults(calc3.CalcFormulaA(), 5, "Formula A Lab 3");
Printer.PrintResults(calc3.CalcFormulaB(), 5, "Formula B Lab 3");

Printer.PrintResults(calc4.CalcFormulaA(), 5, "Formula A Lab 4");
Printer.PrintResults(calc4.CalcFormulaB(), 5, "Formula B Lab 4");

Printer.PrintResults(calc5.CalcFormulaA(), 5, "Formula A Lab 5");
Printer.PrintResults(calc5.CalcFormulaB(), 5, "Formula B Lab 5");

Printer.PrintResults(calc6.CalcFormulaA(), 5, "Formula A Lab 6");
Printer.PrintResults(calc6.CalcFormulaB(), 5, "Formula B Lab 6");

BenchmarkRunner.Run<Benchmark>();
