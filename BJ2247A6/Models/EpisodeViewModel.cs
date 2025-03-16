using BJ2247A5.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJ2247A5.Models
{

    public class EpisodeWithShowNameViewModel : EpisodeVideoViewModel
    {
        public string ShowName { get; set; }
    }

    public class EpisodeAddFormViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        [Required]
        public int SeasonNumber { get; set; }

        [Required]
        public int EpisodeNumber { get; set; }
        
        //List for selection
        [Display(Name = "Genre")]
        public SelectList GenreList { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AirDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        public string ImageUrl { get; set; }

        public Show Show { get; set; }

        public string ShowName { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        [Required]
        [Display(Name = "Upload Episode")]
        [DataType(DataType.Upload)]
        public string VideoUpload { get; set; }

    }

    public class EpisodeBaseViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Season")]
        public int SeasonNumber { get; set; }

        [Required]
        [Display(Name = "Episode")]
        public int EpisodeNumber { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Genre")]
        public string Genre { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date Aired")]
        public DateTime AirDate { get; set; } = DateTime.Now;

        [Required, StringLength(250)]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [Required, StringLength(250)]
        [Display(Name = "Clerk")]
        public string Clerk { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        public Show Show { get; set; }

        [Display(Name = "Video")]
        public string VideoUrl
        {
            get
            {
                return $"/Episodes/Video/{Id}";
            }
        }
    }


    public class EpisodeWithDetailViewModel : EpisodeBaseViewModel
    {
        public EpisodeWithDetailViewModel()
        {
            Show = new Show();
        }
    }

    public class EpisodeAddViewModel
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

        public Show Show { get; set; }

        [DataType(DataType.MultilineText)]
        public string Premise { get; set; }

        [Required]
        public HttpPostedFileBase VideoUpload { get; set; }
    }

    public class EpisodeVideoViewModel : EpisodeBaseViewModel
    {
        public string VideoContentType { get; set; }
        public byte[] Video { get; set; }
    }


}