using Helpers.DataTables.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Helpers.DataTables
{
    public class DataTablesBinder : BaseDataTablesBinder, IModelBinder
    {
        #region [ Implementação IModelBinder ]

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            HttpRequest request = bindingContext.ActionContext.HttpContext.Request;

            IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> requestParameters = GetRequestParameters(request);
            IDataTablesRequest dataTableRequest = new DataTablesRequest
            {
                Draw = GetParam<int>(requestParameters, DrawFormatting),
                Start = GetParam<int>(requestParameters, StartFormatting),
                Length = GetParam<int>(requestParameters, LengthFormatting),
                Search = new DataTablesSearch(GetParam<string>(requestParameters, SearchValueFormatting), GetParam<bool>(requestParameters, SearchRegexValueFormatting)),
                Columns = GetColumns(requestParameters)
            };
            SetColumnOrdering(requestParameters, dataTableRequest.Columns);

            bindingContext.Result = ModelBindingResult.Success(dataTableRequest);

            return Task.CompletedTask;
        }

        #endregion


    }
}
