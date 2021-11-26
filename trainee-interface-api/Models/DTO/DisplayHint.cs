namespace trainee_interface_api.Models.DTO
{
    public class DisplayHint
    {
        public string HintText { get; set; }
        public string ImageUrl { get; set; }

        public DisplayHint(string hintText, string imageUrl)
        {
            HintText = hintText;
            ImageUrl = imageUrl;
        }
    }
}
