using System.Text.Json;
using CSVParser.Models;

namespace CSVParser.Services;

public class JsonConverterService
{
    public static OptionsModel? GetOptionsModel(string jsonString)
    {
        return JsonSerializer.Deserialize<OptionsModel>(jsonString, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
    }
}