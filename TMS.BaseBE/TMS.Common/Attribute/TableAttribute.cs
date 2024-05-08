using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Common.Attribute
{
    public class TableAttribute : System.Attribute
    {
        public string TableName;
        public string ViewName;
        public bool HasDetail;

        public TableAttribute(string tableName, string viewName, bool hasDetail = false)
        {
            TableName = tableName;
            ViewName = viewName;
            HasDetail = hasDetail;
        }
    }
}
