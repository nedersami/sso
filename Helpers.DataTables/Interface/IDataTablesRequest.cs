using System.Collections.Generic;

namespace Helpers.DataTables.Interface
{ 
	public interface IDataTablesRequest
    {
        int Draw { get; set; }
        int Start { get; set; }
        int Length { get; set; }
        IDataTablesSearch Search { get; set; }
        IEnumerable<IDataTablesColumn> Columns { get; set; }

        IEnumerable<IDataTablesColumn> GetSortedColumns();
        IEnumerable<IDataTablesColumn> GetSearchableColumns();
    }
}
