namespace CSVParser.Views.Home
{
    public class IndexModel
    {
        private readonly ILogger<IndexModel> _logger;

        public string? FilePath;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        
    }
}