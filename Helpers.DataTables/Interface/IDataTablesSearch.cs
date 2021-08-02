namespace Helpers.DataTables.Interface
{
    public interface IDataTablesSearch
    {
        bool IsRegexValue { get; set; }
        string Value { get; set; }
    }
}

