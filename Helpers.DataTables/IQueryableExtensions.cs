using Helpers.DataTables.Interface;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Helpers.DataTables
{
	public static class IQueryableExtensions
	{
        public static IQueryable<T> AplicarPaginacaoOrdenacaoDataTable<T>(this IQueryable<T> query, IDataTablesRequest tableQuery)
        {
            // Ordenação
            if (tableQuery.GetSortedColumns().Any())
            {
                foreach (var sortItem in tableQuery.GetSortedColumns().OrderBy(e => e.OrderIndex))
                {
                    var columnName = sortItem.Data.FirstCharToUpper();
                    if (sortItem.SortDirection.HasValue && sortItem.SortDirection.Value == SortDirection.Descending)
                        query = query.OrderBy($"{columnName} DESC");
                    else
                        query = query.OrderBy($"{columnName}");
                }
            }

            // Paginação
            if (tableQuery.Length != -1)
                query = query.Skip(tableQuery.Start).Take(tableQuery.Length);

            return query;
        }

        public static IQueryable<T> AplicarPaginacaoOrdenacaoDataTable<T, TFilter>(this IQueryable<T> query, IDataTablesFilterRequest<TFilter> tableQuery)
        {
            // Ordenação
            if (tableQuery.GetSortedColumns().Any())
            {
                foreach (var sortItem in tableQuery.GetSortedColumns().OrderBy(e => e.OrderIndex))
                {
                    var columnName = sortItem.Data.FirstCharToUpper();
                    if (sortItem.SortDirection.HasValue && sortItem.SortDirection.Value == SortDirection.Descending)
                        query = query.OrderBy($"{columnName} DESC");
                    else
                        query = query.OrderBy($"{columnName}");
                }
            }

            // Paginação
            if (tableQuery.Length != -1)
                query = query.Skip(tableQuery.Start).Take(tableQuery.Length);

            return query;
        }

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null:
                    throw new ArgumentNullException(nameof(input));
                case "":
                    throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default:
                    return input.First().ToString().ToUpper() + input.Substring(1);
            }
        }

    }
}
