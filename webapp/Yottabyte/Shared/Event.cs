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
}
