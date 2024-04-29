namespace TMS.API.Param
{
    public class ChangePasswordParam
    {
        public string OldPass { get; set; }

        public string NewPass { get; set; }

        public string ConfirmPass { get; set;}
    }
}
