using Helpers.DataTables.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Helpers.DataTables
{
    public class DataTablesFilterBinder<T> : BaseDataTablesBinder, IModelBinder
    {
        #region [ Constantes e atributos FilterModel ]

        private const string RexChechNumeric = @"^\d+$";
        private const string RexBrackets = @"\[\d*\]";
        private const string RexSearchBracket = @"\[([^}])\]";

        //Define original source data list
        private List<KeyValuePair<string, string>> kvps;

        //Set default maximum resursion limit
        private const int maxRecursionLimit = 100;
        private int recursionCount = 0;

        #endregion

        #region [ Implementação IModelBinder ]

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            HttpRequest request = bindingContext.HttpContext.Request;
            IEnumerable<KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>> requestParameters = GetRequestParameters(request);
            IDataTablesFilterRequest<T> dataTableRequest = new DataTablesFilterRequest<T>
            {
                Draw = GetParam<int>(requestParameters, DrawFormatting),
                Start = GetParam<int>(requestParameters, StartFormatting),
                Length = GetParam<int>(requestParameters, LengthFormatting),
                Columns = GetColumns(requestParameters)
            };
            SetColumnOrdering(requestParameters, dataTableRequest.Columns);

            // Capturando parâmetros da classe genérica FilterModel
            kvps = new List<KeyValuePair<string, string>>();
            foreach (var item in requestParameters)
                kvps.Add(new KeyValuePair<string, string>(item.Key, item.Value));
            Type type = typeof(T);
            object model = Activator.CreateInstance(type);
            SetPropertyValues(model);
            dataTableRequest.FilterModel = (T)model;

            bindingContext.Result = ModelBindingResult.Success(dataTableRequest);

            return Task.CompletedTask;
        }

        #endregion

        #region [ Métodos FilterModel ]

        public void SetPropertyValues(object obj, object parentObj = null, PropertyInfo parentProp = null)
        {
            //Recursively set PropertyInfo array for object hierarchy
            PropertyInfo[] props = obj.GetType().GetProperties();

            //Set KV Work List for real iteration process so that kvps is not in iteration and
            //its items from kvps can be removed after each iteration
            List<KeyValuePair<string, string>> kvpsWork;

            foreach (var prop in props)
            {
                //Refresh KV Work list from refreshed base kvps list after processing each property
                kvpsWork = new List<KeyValuePair<string, string>>(kvps);

                //Check and process property encompassing complex object recursively 
                if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String")
                {
                    RecurseNestedObj(obj, prop);
                }
                else
                {
                    foreach (var item in kvpsWork)
                    {
                        //Ignore any bracket in a name key 
                        var key = item.Key;
                        var keyParts = Regex.Split(key, RexBrackets);
                        if (keyParts.Length > 1) key = keyParts[keyParts.Length - 1];
                        if (key == prop.Name)
                        {
                            //Populate KeyValueWork and pass it for adding property to object
                            var kvw = new KeyValueWork()
                            {
                                Key = item.Key,
                                Value = item.Value,
                                SourceKvp = item
                            };
                            AddSingleProperty(obj, prop, kvw);
                            break;
                        }
                    }
                }
            }
            //Add property of this object to parent object 
            if (parentObj != null)
            {
                parentProp.SetValue(parentObj, obj, null);
            }
        }

        public void SetPropertyValuesForList(object obj, object parentObj = null, PropertyInfo parentProp = null,
                                             string pParentName = "", string pParentObjIndex = "")
        {
            //Get props for type of object item in collection
            PropertyInfo[] props = obj.GetType().GetProperties();
            //KV Work For each object item in collection
            List<KeyValueWork> kvwsGroup = new List<KeyValueWork>();
            //KV Work for collection
            List<List<KeyValueWork>> kvwsGroups = new List<List<KeyValueWork>>();

            Regex regex;
            Match match;
            bool isGroupAdded = false;
            string lastIndex = "";

            foreach (var item in kvps)
            {
                //Passed parentObj and parentPropName are for List, whereas obj is instance of type for List
                if (item.Key.Contains(parentProp.Name))
                {
                    //Get data only from parent-parent for linked child KV Work
                    if (pParentName != "" & pParentObjIndex != "")
                    {
                        regex = new Regex(pParentName + RexSearchBracket);
                        match = regex.Match(item.Key);
                        if (match.Groups[1].Value != pParentObjIndex)
                            break;
                    }
                    //Get parts from current KV Work
                    regex = new Regex(parentProp.Name + RexSearchBracket);
                    match = regex.Match(item.Key);
                    var brackets = match.Value.Replace(parentProp.Name, "");
                    var objIdx = match.Groups[1].Value;

                    //Point to start next idx and save last kvwsGroup data to kvwsGroups
                    if (lastIndex != "" && objIdx != lastIndex)
                    {
                        kvwsGroups.Add(kvwsGroup);
                        isGroupAdded = true;
                        kvwsGroup = new List<KeyValueWork>();
                    }
                    //Get parts array from Key
                    var keyParts = item.Key.Split(new string[] { brackets }, StringSplitOptions.RemoveEmptyEntries);
                    //Populate KV Work
                    var kvw = new KeyValueWork()
                    {
                        ObjIndex = objIdx,
                        ParentName = parentProp.Name,
                        //Get last part from prefixed name
                        Key = keyParts[keyParts.Length - 1],
                        Value = item.Value,
                        SourceKvp = item
                    };
                    //add KV Work to kvwsGroup list
                    kvwsGroup.Add(kvw);
                    lastIndex = objIdx;
                    isGroupAdded = false;
                }
            }
            //Handle the last kvwsgroup item if not added to final kvwsGroups List.
            if (kvwsGroup.Count > 0 && isGroupAdded == false)
                kvwsGroups.Add(kvwsGroup);

            //Initiate List or Array
            IList listObj = null;
            Array arrayObj = null;
            if (parentProp.PropertyType.IsGenericType || parentProp.PropertyType.BaseType.IsGenericType)
            {
                listObj = (IList)Activator.CreateInstance(parentProp.PropertyType);
            }
            else if (parentProp.PropertyType.IsArray)
            {
                arrayObj = Array.CreateInstance(parentProp.PropertyType.GetElementType(), kvwsGroups.Count);
            }

            int idx = 0;
            foreach (var group in kvwsGroups)
            {
                //Initiate object with type of collection item
                var tempObj = Activator.CreateInstance(obj.GetType());
                foreach (var prop in props)
                {
                    //Check and process nested objects in collection recursively
                    //Pass ObjIndex for child KV Work items only for this parent object
                    if (prop.PropertyType.IsClass && prop.PropertyType.FullName != "System.String")
                    {
                        RecurseNestedObj(tempObj, prop, pParentName: group[0].ParentName, pParentObjIndex: group[0].ObjIndex);
                    }
                    else
                    {
                        //Assign terminal property to object    
                        foreach (var item in group)
                        {
                            if (item.Key == prop.Name)
                            {
                                AddSingleProperty(tempObj, prop, item);
                                break;
                            }
                        }
                    }
                }
                //Add populated object to List or Array                    
                if (listObj != null)
                {
                    listObj.Add(tempObj);
                }
                else if (arrayObj != null)
                {
                    arrayObj.SetValue(tempObj, idx);
                    idx++;
                }
            }
            //Add property for List or Array into parent object 
            if (listObj != null)
            {
                parentProp.SetValue(parentObj, listObj, null);
            }
            else if (arrayObj != null)
            {
                parentProp.SetValue(parentObj, arrayObj, null);
            }
        }

        private void RecurseNestedObj(object obj, PropertyInfo prop, string pParentName = "", string pParentObjIndex = "")
        {
            //Check recursion limit
            if (recursionCount > maxRecursionLimit)
            {
                throw new Exception(string.Format("Exceed maximum recursion limit {0}", maxRecursionLimit));
            }
            recursionCount++;

            //Valicate collection types
            if (prop.PropertyType.IsGenericType || prop.PropertyType.BaseType.IsGenericType)
            {
                if ((prop.PropertyType.IsGenericType && prop.PropertyType.Name != "List`1")
                    || (prop.PropertyType.BaseType.IsGenericType && prop.PropertyType.BaseType.Name != "List`1"))
                {
                    throw new Exception("Only support nested Generic List collection");
                }
                if (prop.PropertyType.GenericTypeArguments.Count() > 1 || prop.PropertyType.BaseType.GenericTypeArguments.Count() > 1)
                {
                    throw new Exception("Only support nested Generic List collection with one argument");
                }
            }

            object childObj = null;
            if (prop.PropertyType.IsGenericType || prop.PropertyType.BaseType.IsGenericType || prop.PropertyType.IsArray)
            {
                //Dynamically create instances for nested collection items
                if (prop.PropertyType.IsGenericType)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GenericTypeArguments[0]);
                }
                else if (!prop.PropertyType.IsGenericType && prop.PropertyType.BaseType.IsGenericType)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.BaseType.GenericTypeArguments[0]);
                }
                else if (prop.PropertyType.IsArray)
                {
                    childObj = Activator.CreateInstance(prop.PropertyType.GetElementType());
                }
                //Call to process collection
                SetPropertyValuesForList(childObj, parentObj: obj, parentProp: prop,
                            pParentName: pParentName, pParentObjIndex: pParentObjIndex);
            }
            else
            {
                //Dynamically create instances for nested object and call to process it
                childObj = Activator.CreateInstance(prop.PropertyType);
                SetPropertyValues(childObj, parentObj: obj, parentProp: prop);
            }
        }

        private void AddSingleProperty(object obj, PropertyInfo prop, KeyValueWork item)
        {
            if (prop.PropertyType.IsEnum)
            {
                var enumValues = prop.PropertyType.GetEnumValues();
                object enumValue = null;
                bool isFound = false;

                //Try to match enum item name first
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if (item.Value.ToLower() == enumValues.GetValue(i).ToString().ToLower())
                    {
                        enumValue = enumValues.GetValue(i);
                        isFound = true;
                        break;
                    }
                }
                //Try to match enum default underlying int value if not matched with enum item name
                if (!isFound)
                {
                    for (int i = 0; i < enumValues.Length; i++)
                    {
                        if (item.Value == i.ToString())
                        {
                            enumValue = i;
                            break;
                        }
                    }
                }
                prop.SetValue(obj, enumValue, null);
            }
            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type t = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                object safeValue = string.IsNullOrEmpty(item.Value) ? null : Convert.ChangeType(item.Value, t);
                prop.SetValue(obj, safeValue, null);
            }
            else
            {
                //Set value for non-enum terminal property 
                prop.SetValue(obj, Convert.ChangeType(item.Value, prop.PropertyType), null);
            }
            kvps.Remove(item.SourceKvp);
        }

        private List<KeyValuePair<string, string>> ConvertToKvps(string sourceString)
        {
            List<KeyValuePair<string, string>> kvpList = new List<KeyValuePair<string, string>>();
            if (sourceString.StartsWith("?", StringComparison.Ordinal)) sourceString = sourceString.Substring(1);
            string[] elements = sourceString.Split('=', '&');
            for (int i = 0; i < elements.Length; i += 2)
            {
                KeyValuePair<string, string> kvp = new KeyValuePair<string, string>
                (
                    elements[i],
                    elements[i + 1]
                );
                kvpList.Add(kvp);
            }
            return kvpList;
        }

        #endregion
    }
}
