using System.Collections;

namespace Helpers.DataTables.Interface
{ 
	public interface IDataTablesResponse
    {
        int draw { get; }
        IEnumerable data { get; }
        int recordsTotal { get; }
        int recordsFiltered { get; }
    }
}
