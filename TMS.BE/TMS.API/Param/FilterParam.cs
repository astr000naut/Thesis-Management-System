﻿namespace TMS.API.Param
{
    public class FilterParam
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string KeySearch { get; set; }
        public IEnumerable<string>? FilterColumns { get; set; }

        public List<CustomWhere>? CustomWhere { get; set; }

    }

    public class CustomWhere
    {
        public string Command { get; set; }
        public string ColumnName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
}
