using Helpers.DataTables.Interface;
using System.Collections;

namespace Helpers.DataTables
{
    public class DataTablesResponse : IDataTablesResponse
    {
        public int draw { get; private set; }
        public IEnumerable data { get; private set; }
        public int recordsTotal { get; private set; }
        public int recordsFiltered { get; private set; }

        public DataTablesResponse(int draw, IEnumerable data, int recordsFiltered, int recordsTotal)
        {
            this.draw = draw;
            this.data = data;
            this.recordsFiltered = recordsFiltered;
            this.recordsTotal = recordsTotal;
        }
    }
}
