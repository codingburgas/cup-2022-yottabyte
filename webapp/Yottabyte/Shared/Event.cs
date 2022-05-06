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
        public double Lat { get; set; }

        [Required]
        public double Long { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public string ImageURL { get; set; }
    }

    public class EventIM
    {
        [Required]
        [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$")]
        public double Lat { get; set; }
        
        [Required]
        [RegularExpression(@"^[-+]?([1-8]?\d(\.\d+)?|90(\.0+)?),\s*[-+]?(180(\.0+)?|((1[0-7]\d)|([1-9]?\d))(\.\d+)?)$")]
        public double Long { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
