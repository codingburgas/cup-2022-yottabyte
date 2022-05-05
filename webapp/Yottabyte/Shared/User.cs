using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

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

        [Required]
        public string Salt { get; set; }

        [Required]
        public ROLES Role { get; set; }

        [Required]
        public string AvatarURL { get; set; }
    }

    public class UserIM
    {
        [Required]
        [RegularExpression(@"^(?=.*[A-ZAa-z])([A-Z])([a-z]{2,29})$")]
        public string FName { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])([A-Z])([a-z]{2,29})$")]
        public string LName { get; set; }

        [Required]
        [RegularExpression(@"(\w+)(\.|_)?(\w*)@(\w+)(\.(\w+))+")]
        public string Email { get; set; }
        
        [Required]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])[a-zA-Z0-9]{8,}")]
        public string Password { get; set; }

        public IFormFile Avatar { get; set; }
    }

    public class UserVM
    {
        public int Id { get; set; }
     
        public string FName { get; set; }

        public string LName { get; set; }

        public string Email { get; set; }

        public string AvatarURL { get; set; }
    }
}
