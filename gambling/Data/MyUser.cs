using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace gambling.Data
{
    public class MyUser : IdentityUser
    {
        [Required]
        public double Balance { get; set; } = 10000.0;
    }
}
