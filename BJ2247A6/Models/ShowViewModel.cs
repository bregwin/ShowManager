using BJ2247A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJ2247A5.Models
{
    public class ShowBaseViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Genre")]
        [Required, StringLength(50)]
        public string Genre { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required, StringLength(250)]
        [Display(Name = "Coordinator")]
        public string Coordinator { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        public ICollection<Actor> Actors { get; set; }

        public ICollection<Episode> Episodes { get; set; }
    }

    public class ShowAddFormViewModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        
        //Needed For Form

        public string ActorName { get; set; }

        public int ActorId { get; set; }

        //List for selection
        [Display(Name = "Genre")]
        public SelectList GenreList { get; set; }

        [Display(Name = "Actors")]
        public MultiSelectList ActorList { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }
    }

    public class ShowAddViewModel
    {
        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required, StringLength(50)]
        public string Genre { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        //Actor Identifiers

        public IEnumerable<int> SelectedActorIds { get; set; }
    }

    public class ShowWithInfoViewModel : ShowBaseViewModel
    {
        public ShowWithInfoViewModel()
        {
            Actors = new List<Actor>();
            Episodes = new List<Episode>();
        }

        public int ActorCount
        {
            get { return Actors.Count; }
        }

        public int EpisodeCount
        {
            get { return Episodes.Count; }
        }
    }
}