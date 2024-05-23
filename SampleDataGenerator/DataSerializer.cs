using System.Text.Json;

namespace SampleDataGenerator;

public static class DataSerializer
{
	public static void Serialize<T>(T data, string path, bool prettyPrinting = true)
	{
		var jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = prettyPrinting});
		File.WriteAllText(path, jsonString);
	}

	public static T? Deserialize<T>(string path)
	{
		var jsonString = File.ReadAllText(path);
		return JsonSerializer.Deserialize<T>(jsonString);
	}
}