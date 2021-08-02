namespace Helpers.DataTables.Interface
{
    public interface IDataTablesColumn
    {
        string Data { get; set; }
        string Name { get; set; }
        bool Searchable { get; set; }
        bool Orderable { get; set; }
        IDataTablesSearch Search { get; set; }
        int? OrderIndex { get; set; }
        SortDirection? SortDirection { get; set; }
    }
}
