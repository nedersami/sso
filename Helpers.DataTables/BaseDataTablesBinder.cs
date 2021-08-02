using Helpers.DataTables.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers.DataTables
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class BaseDataTablesBinder
    {
        #region [ Constantes Datatable ]

        protected const string DrawFormatting = "draw";
        protected const string StartFormatting = "start";
        protected const string LengthFormatting = "length";
        protected const string ColumnDataFormatting = "columns[{0}][data]";
        protected const string ColumnNameFormatting = "columns[{0}][name]";
        protected const string OrderColumnFormatting = "order[{0}][column]";
        protected const string OrderDirectionFormatting = "order[{0}][dir]";
        protected const string ColumnSearchableFormatting = "columns[{0}][searchable]";
        protected const string ColumnOrderableFormatting = "columns[{0}][orderable]";
        protected const string ColumnSearchValueFormatting = "columns[{0}][search][value]";
        protected const string ColumnSearchRegexFormatting = "columns[{0}][search][regex]";
        protected const string SearchValueFormatting = "search[value]";
        protected const string SearchRegexValueFormatting = "search[regex]";

        #endregion

        #region [ Métodos Datatables ]

        protected IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> GetRequestParameters(HttpRequest request)
        {
            var method = request.Method.ToLower();
            if (method.Equals("get")) return request.Query.AsEnumerable();
            if (method.ToLower().Equals("post")) return request.Form.AsEnumerable();
            throw new ArgumentException(string.Format("The provided HTTP method ({0}) is not a valid method to use with DataTableBinder. Please, use HTTP GET or POST.", request.Method), "request");
        }

        protected IEnumerable<IDataTablesColumn> GetColumns(IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> collection)
        {
            var columns = new List<IDataTablesColumn>();

            for (int i = 0; i < collection.Count(); i++)
            {
                var columnData = GetParam<string>(collection, string.Format(ColumnDataFormatting, i));
                var columnName = GetParam<string>(collection, string.Format(ColumnNameFormatting, i));

                if (columnData != null && columnName != null)
                {
                    var columnSearchable = GetParam<bool>(collection, string.Format(ColumnSearchableFormatting, i));
                    var columnOrderable = GetParam<bool>(collection, string.Format(ColumnOrderableFormatting, i));
                    var columnSearchValue = GetParam<string>(collection, string.Format(ColumnSearchValueFormatting, i));
                    var columnSearchRegex = GetParam<bool>(collection, string.Format(ColumnSearchRegexFormatting, i));

                    columns.Add(new DataTablesColumn(columnData, columnName, columnSearchable, columnOrderable, columnSearchValue, columnSearchRegex));
                }
                else break;
            }

            return columns;
        }

        protected void SetColumnOrdering(IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> collection, IEnumerable<IDataTablesColumn> columns)
        {
            var idx = 0;
            for (int i = 0; i < collection.Count(); i++)
            {
                var orderColumn = GetParam<int>(collection, String.Format(OrderColumnFormatting, i));
                var orderDirection = GetParam<string>(collection, String.Format(OrderDirectionFormatting, i));

                if (orderColumn <= -1 || string.IsNullOrWhiteSpace(orderDirection)) break;

                var column = columns.ElementAt(orderColumn);

                column.OrderIndex = idx;
                column.SortDirection = orderDirection.ToLower().Equals("asc")
                    ? SortDirection.Ascending
                    : SortDirection.Descending;

                idx++;
            }
        }

        protected TParam GetParam<TParam>(IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> collection, string key)
        {
            try
            {
                var collectionItem = collection.Where(x => x.Key == key).Single();
                return (TParam)Convert.ChangeType(collectionItem.Value.ToString(), typeof(TParam));
            }
            catch
            {
                return default(TParam);
            }
        }

        #endregion
    }
}
