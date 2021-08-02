using System.Collections.Generic;

namespace Helpers.DataTables
{
    public class DataTablesModel<T>
    {
        public DataTablesModel() { }

        public DataTablesModel(int total, List<T> model)
        {
            this.Total = total;
            this.Model = model;
        }

        public int Total { get; set; }

        public List<T> Model { get; set; }
    }
}
