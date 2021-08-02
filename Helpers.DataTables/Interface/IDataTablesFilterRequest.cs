using System.Collections.Generic;

namespace Helpers.DataTables.Interface
{
    public interface IDataTablesFilterRequest<T>
    {
        int Draw { get; set; }
        int Start { get; set; }
        int Length { get; set; }
        IEnumerable<IDataTablesColumn> Columns { get; set; }
        T FilterModel { get; set; }
        IEnumerable<IDataTablesColumn> GetSortedColumns();
    }
}
