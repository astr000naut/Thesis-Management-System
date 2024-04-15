namespace TMS.API.Param
{
    public class FilterParam
    {
        public int Skip { get; set; }
        public int Take { get; set; }
        public string KeySearch { get; set; }

        public IEnumerable<string>? FilterColumns { get; set; }

    }
}
