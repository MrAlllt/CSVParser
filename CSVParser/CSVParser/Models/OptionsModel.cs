namespace CSVParser.Models;

public class OptionsModel
{
    public bool firstLineAsProperties { get; set; }
    public char separator { get; set; }
    public int numberOfLines { get; set; }
    public string[] customProperties { get; set; }
}