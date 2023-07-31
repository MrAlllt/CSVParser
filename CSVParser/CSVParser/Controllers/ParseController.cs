using CSVParser.Models;
using CSVParser.Services;
using Microsoft.AspNetCore.Mvc;

namespace CSVParser.Controllers;

[Route("[controller]")]
public class ParseController : Controller
{
    private static IWebHostEnvironment _environment;
    private readonly string _filePath;

    public ParseController(IWebHostEnvironment environment)
    {
        _environment = environment;
        _filePath = _environment.WebRootPath + "\\SentFiles\\";
    }

    private OptionsModel ParseOptionsFromJson(string options)
    {
        return JsonConverterService.GetOptionsModel(options);
    }

    [HttpPost]
    public async Task<string> Post([FromForm] FileUpload file, [FromForm] string options)
    {
        
        if (file.CsvFile.Length > 0)
        {
            try
            {
                if (!Directory.Exists(_filePath))
                {
                    Directory.CreateDirectory(_filePath);
                }

                OptionsModel optionsModel = ParseOptionsFromJson(options);
                string uniqueFileName = Guid.NewGuid() + ".csv";

                await using (FileStream fileStream = System.IO.File.Create(_filePath + uniqueFileName))
                {
                    await file.CsvFile.CopyToAsync(fileStream);
                }
                
                FileReadService fileReadService = new FileReadService(_filePath + uniqueFileName, optionsModel.separator);
                ParseService parseService = new ParseService(fileReadService.GetRowCollection());
                fileReadService.RemoveFile();


                return !optionsModel.firstLineAsProperties
                    ? parseService.ParseToJsonString(optionsModel.numberOfLines, optionsModel.customProperties)
                    : parseService.ParseToJsonString(optionsModel.numberOfLines);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
        else
        {
            return "Unsuccessful attempt of file upload!";
        }
    }
}