namespace TelegramNews.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class TelegramCodeViewModel
    {
        [Required]
        public string Code { get; set; }
    }
}