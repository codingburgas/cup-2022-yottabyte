using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared
{
    public enum ROLES
    {
        USER,
        ADMIN,
    }

    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FName { get; set; }

        [Required]
        public string LName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Salt { get; set; }

        [Required]
        public ROLES Role { get; set; }

        public string AvatarURL { get; set; }
    }
}
