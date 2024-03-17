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

        public TableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}
