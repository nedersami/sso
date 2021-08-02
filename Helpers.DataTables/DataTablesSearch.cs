using Helpers.DataTables.Interface;

namespace Helpers.DataTables
{
    public class DataTablesSearch : IDataTablesSearch
    {
        public bool IsRegexValue { get; set; }
        public string Value { get; set; }

        public DataTablesSearch(string value, bool isRegexValue)
        {
            Value = value;
            IsRegexValue = isRegexValue;
        }
    }
}
