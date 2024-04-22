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

        public TableAttribute(string tableName, string viewName)
        {
            TableName = tableName;
            ViewName = viewName;
        }
    }
}
