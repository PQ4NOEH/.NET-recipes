using System.ComponentModel.DataAnnotations;

namespace Heracles.Models.Home
{
    public class LogInModel
    {
        [Required]
        public string Username { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        public bool Remember { get; set; }
    }
}
