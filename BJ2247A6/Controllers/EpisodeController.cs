using BJ2247A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJ2247A5.Controllers
{
    public class EpisodeController : Controller
    {
        private Manager m = new Manager();

        // GET: Show
        public ActionResult Index()
        {
            return View(m.EpisodeGetAll());
        }

        // GET: Episode/Details/5
        public ActionResult Details(int? id)
        {
            var episode = m.EpisodeGetById(id.GetValueOrDefault());

            if (episode == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(episode);
            }
        }


        // GET: Episode/Videos/5
        [Route("Episodes/{id}/Video")]
        public ActionResult GetVideo(int? id)
        {
            var episodeVideo = m.EpisodeVideoGetById(id.GetValueOrDefault());

            if (episodeVideo == null)
            {
                return HttpNotFound();
            }
            else
            {

                return File(episodeVideo.Video, episodeVideo.VideoContentType);
            }
        }


    }
}
