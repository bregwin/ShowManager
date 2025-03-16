using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BJ2247A5.Data
{
    public class Actor
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [StringLength(150)]
        public string AlternateName { get; set; }

        public DateTime? BirthDate { get; set; }
        public double? Height { get; set; }

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        [Required, StringLength(250)]
        public string Executive { get; set; }

        public string Biography { get; set; }

        public ICollection<Show> Shows { get; set; }

        public ICollection<ActorMediaItem> ActorMediaItems { get; set; }

        public Actor()
        {
            Shows = new HashSet<Show>();
        }
    }

}