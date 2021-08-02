using Helpers.DataTables.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.DataTables
{
    public class DataTablesRequest : IDataTablesRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public IDataTablesSearch Search { get; set; }
        public IEnumerable<IDataTablesColumn> Columns { get; set; }

        public IEnumerable<IDataTablesColumn> GetSortedColumns()
        {
            return Columns.Where(x => x.Orderable && x.OrderIndex.HasValue).OrderBy(x => x.OrderIndex);
        }

        public IEnumerable<IDataTablesColumn> GetSearchableColumns()
        {
            return Columns.Where(x => x.Searchable);
        }
    }
}
