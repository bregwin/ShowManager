using BJ2247A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BJ2247A5.Models
{
    public class ActorAddViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(150)]
        [Display(Name = "Alternate Name")]
        public string AlternateName { get; set; }

        [Display(Name = "Birth Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Height(m)")]
        public double? Height { get; set; }

        [Display(Name = "Image")]
        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }

    }

    public class ActorBaseViewModel: ActorAddViewModel
    {
        [Display(Name = "Executive")]
        [Required, StringLength(250)]
        public string Executive { get; set; }
    }

    public class ActorWithShowInfoViewModel: ActorBaseViewModel
    {
        public ActorWithShowInfoViewModel()
        {
            Shows = new List<Show>();
        }

        [Display(Name = "Appeared In")]
        public List<Show> Shows { get; set; }

        public int ShowCount
        {
            get { return Shows.Count; }
        }

        public IEnumerable<ActorMediaItemBaseViewModel> ActorMediaItems { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> Photos { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> Documents { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> AudioClips { get; set; }
        public IEnumerable<ActorMediaItemBaseViewModel> VideoClips { get; set; }
    }

}