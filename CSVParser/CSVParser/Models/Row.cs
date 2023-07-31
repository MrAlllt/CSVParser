namespace CSVParser.Models;

public class Row
{
    public int LineNum { get; set; }
    public string[] Data { get; set; }

    public Row(int lineNum, string[] data)
    {
        LineNum = lineNum;
        Data = data;
    }
}