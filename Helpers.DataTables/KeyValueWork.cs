using System.Collections.Generic;

namespace Helpers.DataTables
{
    internal class KeyValueWork
    {
        internal string ObjIndex { get; set; }
        internal string ParentName { get; set; }
        internal string Key { get; set; }
        internal string Value { get; set; }
        internal KeyValuePair<string, string> SourceKvp { get; set; }
    }
}
