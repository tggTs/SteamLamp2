using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamLamp
{
    [Table("Users")]
    public class User
    {
        [Key] 
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Nickname { get; set; }

        [Required]
        public string Password { get; set; }

        public decimal Balance { get; set; } = 0;
    }
}