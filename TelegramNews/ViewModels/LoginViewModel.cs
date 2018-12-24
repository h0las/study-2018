﻿namespace TelegramNews.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [DataType(DataType.Password), Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
