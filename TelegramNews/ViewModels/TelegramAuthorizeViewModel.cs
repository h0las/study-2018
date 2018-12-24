namespace TelegramNews.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class TelegramAuthorizeViewModel
    {
        [Required]
        public string PhoneNumber { get; set; }
    }
}