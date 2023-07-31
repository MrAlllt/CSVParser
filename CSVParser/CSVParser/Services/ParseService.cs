using System.Text;
using CSVParser.Models;

namespace CSVParser.Services;

public class ParseService
{
    private List<Row> RowsCollection { get; }
    private int NumOfLines { get; set; }

    public ParseService(List<Row> rowsCollection)
    {
        RowsCollection = rowsCollection;
    }

    public string ParseToJsonString(int numOfLines = -1, string[]? customProperties = null)
    {
        StringBuilder builder = new StringBuilder("{\n");

        if (customProperties == null)
            customProperties = GetProperties(RowsCollection[0]);
        else
            customProperties = PrepareCustomProperties(customProperties);

        int lines = RowsCollection.Count;
        if (numOfLines >= 0)
            lines = numOfLines;
            
        for (int line = 0; line < lines; line++)
        {

            Row row = RowsCollection[line];
            builder.Append("\t\"row_" + row.LineNum + "\": {\n");
            
            for (int i = 0; i < row.Data.Length; i++)
            {
                var data = row.Data[i];
                    
                if (data.Contains('"')) 
                    data = TrimQuotationMarks(data);

                if (ValidateType(data) != "bool")
                    builder.Append($"\t\t\"{customProperties[i]}\": \"{data}\"");
                else
                    builder.Append($"\t\t\"{customProperties[i]}\": {data}");

                if (i != row.Data.Length - 1)
                    builder.Append(",\n");
            }

            if (line == lines - 1)
                builder.Append("\n\t}\n");
            else
                builder.Append("\n\t},\n");
            
        }
        
        return builder.Append("}").ToString();
    }

    private string ValidateType(string data)
    {
        if (double.TryParse(data, out double n))
            return "number";

        if (bool.TryParse(data, out bool n2))
            return "bool";

        return "string";
    }

    private string[] GetProperties(Row row)
    {
        string[] properties = row.Data;

        for (int i = 0; i < properties.Length; i ++)
        {
            properties[i] = TrimQuotationMarks(properties[i]);
        }
        
        return properties;
    }

    private string[] PrepareCustomProperties(string[] customProperties)
    {
        for (int i = 0; i < customProperties.Length; i++)
        {
            customProperties[i] = TrimQuotationMarks(customProperties[i]);
        }

        return customProperties;
    }

    private string TrimQuotationMarks(string str)
    {
        return str.Trim('"');
    }
}