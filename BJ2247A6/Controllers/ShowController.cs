using BJ2247A5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJ2247A5.Controllers
{
    public class ShowController : Controller
    {
        private Manager m = new Manager();

        // GET: Show
        public ActionResult Index()
        {
            return View(m.ShowGetAll());
        }

        // GET: Show/Details/5
        public ActionResult Details(int? id)
        {
            var show = m.ShowGetById(id.GetValueOrDefault());

            if (show == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(show);
            }
        }

        // GET: Episodes/{id}/AddEpisode
        [Route("Shows/{id}/AddEpisode")]
        [Authorize(Roles = "Clerk")]
        public ActionResult AddEpisode(int id)
        {
            var show = m.ShowGetById(id);
            var genres = m.GenreGetAll();
            var firstGenre = genres.ElementAt(0).Id;

            if (show == null) return HttpNotFound();

            var form = new EpisodeAddFormViewModel();
            form.Id = show.Id;
            form.ShowName = show.Name;

            form.GenreList = new SelectList(m.GenreGetAll(), "Name", "Name", selectedValue: firstGenre);
            return View(form);
        }

        // POST: Episodes/{id}/AddEpisode
        [HttpPost]
        [Authorize(Roles = "Clerk")]
        [Route("Shows/{id}/AddEpisode")]
        [ValidateInput(false)]
        public ActionResult AddEpisode(EpisodeAddViewModel newItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }
            var addedItem = m.EpisodeAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", "Episode", new { id = addedItem.Id });
            }
        }
    }

}
