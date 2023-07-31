using CSVParser.Models;

namespace CSVParser.Services;

public class FileReadService
{
    private readonly char _separator;
    private readonly string _fileName;

    public FileReadService(string fileName, char separator)
    {
        _fileName = fileName;
        _separator = separator;
    }

    public List<Row> GetRowCollection()
    {
        StreamReader streamReader = new StreamReader(_fileName);
        List<Row> listToReturn = new List<Row>();
        
        int i = 1;

        string line;
        while ((line = streamReader.ReadLine()) != null)
        {
            string[] lineSplit = line.Split(_separator);
            string temp = "";
            List<string> newLineSplit = new List<string>();
            bool quotationMarksEquals1 = false;

            for (int j = 0; j < lineSplit.Length; j++)
            {
                string str = lineSplit[j];
                int quotationMarks = str.Count(character => '"' == character);

                if (quotationMarks == 1)
                    quotationMarksEquals1 = !quotationMarksEquals1;
                
                if (quotationMarksEquals1)
                {
                    temp += str + _separator;
                }
                else
                {
                    if (temp.Length > 0)
                    {
                        newLineSplit.Add(temp + str);
                        temp = "";
                    }
                    else
                        newLineSplit.Add(str);
                }
            }

            listToReturn.Add(new Row(i, newLineSplit.ToArray()));
            i++;
        }

        streamReader.Close();
        return listToReturn;
    }

    public void RemoveFile()
    {
        if (File.Exists(_fileName))
            File.Delete(_fileName);
    }
}