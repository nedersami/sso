using Helpers.DataTables.Interface;

namespace Helpers.DataTables
{
    public class DataTablesColumn : IDataTablesColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }
        public IDataTablesSearch Search { get; set; }
        public int? OrderIndex { get; set; }
        public SortDirection? SortDirection { get; set; }

        public DataTablesColumn(string data, string name, bool searchable, bool orderable, string searchValue, bool isSearchRegex)
        {
            Data = data;
            Name = name;
            Searchable = searchable;
            Orderable = orderable;
            Search = new DataTablesSearch(searchValue, isSearchRegex);
        }
    }
}
