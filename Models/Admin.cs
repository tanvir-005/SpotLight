using System.ComponentModel.DataAnnotations;

namespace SpotLight.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = "admin";

        public string Password { get; set; } = "admin";
    }
}
