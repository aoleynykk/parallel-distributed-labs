using SampleDataGenerator;

var g = new DataGenerator();

var sampleData = new SampleData(
    g.GenerateMatrix(100, 100, 1, 100), // B
    g.GenerateMatrix(100, 100, 1, 100), // E
    g.GenerateMatrix(100, 100, 1, 100), // ME
    g.GenerateMatrix(100, 100, 1, 100), // MZ
    g.GenerateMatrix(100, 100, 1, 100)  // MM
);

DataSerializer.Serialize(sampleData, "../../../ConsoleApp/Data/data.json"); // Automatically adds data to the ConsoleApp
