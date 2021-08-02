using Helpers.DataTables.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.DataTables
{
    public class DataTablesFilterRequest<T> : IDataTablesFilterRequest<T>
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public IEnumerable<IDataTablesColumn> Columns { get; set; }
        public T FilterModel { get; set; }

        public IEnumerable<IDataTablesColumn> GetSortedColumns()
        {
            return Columns.Where(x => x.Orderable && x.OrderIndex.HasValue).OrderBy(x => x.OrderIndex);
        }
    }
}
