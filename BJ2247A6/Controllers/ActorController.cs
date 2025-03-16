using BJ2247A5.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJ2247A5.Controllers
{
    public class ActorController : Controller
    {
        private Manager m = new Manager();

        // GET: Actor
        public ActionResult Index()
        {
            return View(m.ActorGetAll());
        }

        // GET: Actor/Details/5
        public ActionResult Details(int? id)
        {
            var actor = m.ActorGetById(id.GetValueOrDefault());
            if (actor == null)
            {
                return HttpNotFound();
            }

            actor.Photos = actor.ActorMediaItems.Where(p => p.ContentType.StartsWith("image/")).OrderBy(p => p.Caption);
            actor.Documents = actor.ActorMediaItems.Where(p => p.ContentType.StartsWith("application/pdf")).OrderBy(p => p.Caption);
            actor.AudioClips = actor.ActorMediaItems.Where(p => p.ContentType.StartsWith("audio/")).OrderBy(p => p.Caption);
            actor.VideoClips = actor.ActorMediaItems.Where(p => p.ContentType.StartsWith("video/")).OrderBy(p => p.Caption);
            return View(actor);
        }

        // GET: Actor/Create
        [Authorize(Roles = "Executive")]
        public ActionResult Create()
        {
            var model = new ActorAddViewModel();
            model.BirthDate = DateTime.Now;
            return View(model);
        }

        // POST: Actor/Create
        [HttpPost]
        [Authorize(Roles = "Executive")]
        [ValidateInput(false)]
        public ActionResult Create(ActorAddViewModel newItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            var addedItem = m.ActorAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            return RedirectToAction("Details", new { id = addedItem.Id });
        }

        //get actor from id, get genres, put genres into form select, put alls actors into multiselect, put current actor for preselection, show page

        // GET: Actors/{id}/AddShow
        [Route("Actors/{id}/AddShow")]
        [Authorize(Roles = "Coordinator")]
        public ActionResult AddShow(int id)
        {
            var form = new ShowAddFormViewModel();
            var actor = m.ActorGetById(id);
            var genres = m.GenreGetAll();

            var firstGenre = genres.ElementAt(0).Id;
            if (actor == null) return HttpNotFound();
            
            form.ActorId = actor.Id;
            form.ActorName = actor.Name;

            var selectedActors = new List<int> { actor.Id };
            form.GenreList = new SelectList(m.GenreGetAll(), "Name", "Name", selectedValue: firstGenre);
            form.ActorList = new MultiSelectList(items: m.ActorGetAll(), selectedValues: selectedActors, dataValueField: "Id", dataTextField: "Name");
              
            return View(form);
        }

        // POST: Actors/{id}/AddShow
        [HttpPost]
        [Route("Actors/{id}/AddShow")]
        [Authorize(Roles = "Coordinator")]
        [ValidateInput(false)]
        public ActionResult AddShow(ShowAddViewModel show)
        {
            if (!ModelState.IsValid)
            {
                return View(show);
            }
            
            var addedItem = m.ShowAdd(show);

            if (addedItem == null) return View(show);
            return RedirectToAction("Details", "Show", new { id = addedItem.Id });
        }

        // GET: Actors/{id}/AddMedia
        [Route("Actors/{id}/AddMedia")]
        [Authorize(Roles = "Executive")]
        public ActionResult AddMedia(int? id)
        {
            var actor = m.ActorGetById(id.GetValueOrDefault());

            if (actor == null) return HttpNotFound();

            var form = new ActorMediaItemAddFormViewModel();
            form.ActorId = actor.Id;
            form.ActorName = actor.Name;
            return View(form);
            
        }

        // POST: Actors/{id}/AddMedia
        [HttpPost]
        [Route("Actors/{id}/AddMedia")]
        [Authorize(Roles = "Executive")]
        public ActionResult AddMedia(int? id, ActorMediaItemAddViewModel newItem)
        {
            if (!ModelState.IsValid)
            {
                return View(newItem);
            }

            var addedItem = m.ActorMediaItemAdd(newItem);

            if (addedItem == null)
            {
                return View(newItem);
            }
            else
            {
                return RedirectToAction("Details", new { id = addedItem.Id });
            }
        }

        [Route("Actors/MediaItem/{id}/Download")]
        public ActionResult DownloadMediaItem(int? id)
        {
            var mediaItem = m.ActorMediaItemGetById(id.GetValueOrDefault());

            if (mediaItem == null )
            {
                return HttpNotFound();
            }
            else
            {
                string extension;
                RegistryKey key;
                object value;

                // Open the Registry, attempt to locate the key
                key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mediaItem.ContentType, false);
                // Attempt to read the value of the key
                value = (key == null) ? null : key.GetValue("Extension", null);
                // Build/create the file extension string
                extension = (value == null) ? string.Empty : value.ToString();

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = $"mediaitem-{id}{extension}",
                    Inline = false
                };
                Response.AppendHeader("Content-Disposition", cd.ToString());

                return File(mediaItem.Content, mediaItem.ContentType);
            }
        }

        [Route("Actors/MediaItem/{id}")]
        public ActionResult GetMediaItem(int? id)
        {
            var mediaItem = m.ActorMediaItemGetById(id.GetValueOrDefault());

            if (mediaItem == null)
            {
                return HttpNotFound();
            }
            else
            {
                return File(mediaItem.Content, mediaItem.ContentType);
            }
        }

    }
}
