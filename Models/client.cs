using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace ProductsCRUDMVC.Models
{
    public class client : IdentityUser
    {
        public client()
        {
            // Constructeur par défaut
        }

        public String firstName { get; set; }
        public String lastName  { get; set; }

        public String type { get; set; }
    }
}
