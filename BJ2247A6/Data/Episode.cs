using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BJ2247A5.Data
{

    public class Episode
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required]
        public int SeasonNumber { get; set; }

        [Required]
        public int EpisodeNumber { get; set; }

        [Required, StringLength(50)]
        public string Genre { get; set; }

        [Required]
        public DateTime AirDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        [Required, StringLength(250)]
        public string Clerk { get; set; } 

        public Show Show { get; set; }

        public string Premise { get; set; }

        public string VideoContentType { get; set; }
        public byte[] Video { get; set; }
    }

}