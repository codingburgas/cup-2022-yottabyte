using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yottabyte.Shared
{
    public class Event
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string Lat { get; set; }

        [Required]
        public string Long { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public string ImageURL { get; set; }
    }

    public class EventIM
    {
        [Required]
        [RegularExpression(@"^(\+|-)?(?:90(?:(?:\.0{1,6})?)|(?:[0-9]|[1-8][0-9])(?:(?:\.[0-9]{1,6})?))$")]
        public string Lat { get; set; }
        
        [Required]
        [RegularExpression(@"^(\+|-)?(?:180(?:(?:\.0{1,6})?)|(?:[0-9]|[1-9][0-9]|1[0-7][0-9])(?:(?:\.[0-9]{1,6})?))$")]
        public string Long { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
