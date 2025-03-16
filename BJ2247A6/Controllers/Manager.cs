using Antlr.Runtime.Misc;
using AutoMapper;
using BJ2247A5.Data;
using BJ2247A5.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;


// ************************************************************************************
// WEB524 Project Template V2 == 2241-37e1f044-7e7e-493f-bfa7-2be8555fc32c
//
// By submitting this assignment you agree to the following statement.
// I declare that this assignment is my own work in accordance with the Seneca Academic
// Policy. No part of this assignment has been copied manually or electronically from
// any other source (including web sites) or distributed to other students.
// ************************************************************************************

namespace BJ2247A5.Controllers
{
    public class Manager
    {

        // Reference to the data context
        private ApplicationDbContext ds = new ApplicationDbContext();

        // AutoMapper instance
        public IMapper mapper;

        // Request user property...

        // Backing field for the property
        private RequestUser _user;

        // Getter only, no setter
        public RequestUser User
        {
            get
            {
                // On first use, it will be null, so set its value
                if (_user == null)
                {
                    _user = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);
                }
                return _user;
            }
        }

        // Default constructor...
        public Manager()
        {
            // If necessary, add constructor code here

            // Configure the AutoMapper components
            var config = new MapperConfiguration(cfg =>
            {
                // Define the mappings below

                cfg.CreateMap<Models.RegisterViewModel, Models.RegisterViewModelForm>();


                cfg.CreateMap<Genre, GenreBaseViewModel>();

                cfg.CreateMap<Actor, ActorBaseViewModel>();
                cfg.CreateMap<Actor, ActorAddViewModel>();
                cfg.CreateMap<ActorAddViewModel, Actor>();
                cfg.CreateMap<Actor, ActorWithShowInfoViewModel>();

                cfg.CreateMap<Show, ShowBaseViewModel>();
                cfg.CreateMap<ShowAddViewModel, Show>();
                cfg.CreateMap<ShowAddViewModel, ShowAddFormViewModel>();

                cfg.CreateMap<Show, ShowWithInfoViewModel>();

                cfg.CreateMap<Episode, EpisodeBaseViewModel>();
                cfg.CreateMap<EpisodeAddViewModel, Episode>();
                cfg.CreateMap<Episode, EpisodeWithShowNameViewModel>();
                cfg.CreateMap<Episode, EpisodeVideoViewModel>();

                cfg.CreateMap<ActorMediaItem, ActorMediaItemWithContentViewModel>();
                cfg.CreateMap<ActorMediaItem, ActorMediaItemBaseViewModel>();

            });

            mapper = config.CreateMapper();

            // Turn off the Entity Framework (EF) proxy creation features
            // We do NOT want the EF to track changes - we'll do that ourselves
            ds.Configuration.ProxyCreationEnabled = false;

            // Also, turn off lazy loading...
            // We want to retain control over fetching related objects
            ds.Configuration.LazyLoadingEnabled = false;
        }


        // Add your methods below and call them from controllers. Ensure that your methods accept
        // and deliver ONLY view model objects and collections. When working with collections, the
        // return type is almost always IEnumerable<T>.
        //
        // Remember to use the suggested naming convention, for example:
        // ProductGetAll(), ProductGetById(), ProductAdd(), ProductEdit(), and ProductDelete().

        public IEnumerable<GenreBaseViewModel> GenreGetAll()
        {
            var genres = ds.Genres.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Genre>, IEnumerable<GenreBaseViewModel>>(genres);
        }

        public IEnumerable<ActorBaseViewModel> ActorGetAll()
        {
            var actors = ds.Actors.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Actor>, IEnumerable<ActorBaseViewModel>>(actors);
        }

        public IEnumerable<ShowBaseViewModel> ShowGetAll()
        {
            var shows = ds.Shows.OrderBy(a => a.Name);
            return mapper.Map<IEnumerable<Show>, IEnumerable<ShowBaseViewModel>>(shows);
        }

        public IEnumerable<EpisodeWithShowNameViewModel> EpisodeGetAll()
        {
            var episodes = ds.Episodes
                                .Include("Show")
                                .OrderBy(a => a.Show.Name)
                                .ThenBy(a => a.SeasonNumber)
                                .ThenBy(a => a.EpisodeNumber);

            return mapper.Map<IEnumerable<Episode>, IEnumerable<EpisodeWithShowNameViewModel>>(episodes);
        }

        public ActorWithShowInfoViewModel ActorAdd(ActorAddViewModel newItem)
        {
            var user = HttpContext.Current.User.Identity.Name;
            if (user == null) return null;

            var addedItem = ds.Actors.Add(mapper.Map<ActorAddViewModel, Actor>(newItem));
            addedItem.Executive = user;
            ds.SaveChanges();
            return (addedItem == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(addedItem);
        }

        public ActorWithShowInfoViewModel ActorGetById(int id)
        {
            var actor = ds.Actors
                             .Include("Shows")
                             .Include("ActorMediaItems")
                             .SingleOrDefault(a => a.Id == id);

            return (actor == null) ? null : mapper.Map<Actor, ActorWithShowInfoViewModel>(actor);
        }

        public ActorMediaItemWithContentViewModel ActorMediaItemGetById(int id)
        {
            var mediaItem = ds.ActorMediaItems.SingleOrDefault(p => p.Id == id);

            return (mediaItem == null) ? null : mapper.Map<ActorMediaItem, ActorMediaItemWithContentViewModel>(mediaItem);
        }

        public ShowWithInfoViewModel ShowAdd(ShowAddViewModel newItem)
        {
            var user = HttpContext.Current.User.Identity.Name;
            if (user == null) return null;

            var addedItem = ds.Shows.Add(mapper.Map<ShowAddViewModel, Show>(newItem));
            var actorList = new List<Actor>();

            foreach (var actorId in newItem.SelectedActorIds)
            {
                var actor = ds.Actors.Find(actorId);
                if (actor == null) return null;
                actorList.Add(actor);
            }
            addedItem.Actors = actorList;
            addedItem.Coordinator = user;
            ds.SaveChanges();
            return (addedItem == null) ? null : mapper.Map<Show, ShowWithInfoViewModel>(addedItem);

        }

        public ShowWithInfoViewModel ShowGetById(int id)
        {
            var show = ds.Shows
                .Include("Actors")
                .Include("Episodes")
                .SingleOrDefault(a => a.Id == id);

            return (show == null) ? null : mapper.Map<Show, ShowWithInfoViewModel>(show);
        }

        public EpisodeWithShowNameViewModel EpisodeAdd(EpisodeAddViewModel newItem)
        {
            var user = HttpContext.Current.User.Identity.Name;
            var show = ds.Shows.Find(newItem.Id);
            if (show == null || user == null) return null;

            byte[] videoBytes = new byte[newItem.VideoUpload.ContentLength];
            newItem.VideoUpload.InputStream.Read(videoBytes, 0, newItem.VideoUpload.ContentLength);

            var addedItem = ds.Episodes.Add(mapper.Map<EpisodeAddViewModel, Episode>(newItem));
            
            addedItem.Show = show;
            addedItem.Clerk = user;
            addedItem.Video = videoBytes;
            addedItem.VideoContentType = newItem.VideoUpload.ContentType;

            ds.SaveChanges();
            return (addedItem == null) ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(addedItem);
            
        }

        public EpisodeWithShowNameViewModel EpisodeGetById(int id)
        {
            var episode = ds.Episodes
                .Include("Show")
                .SingleOrDefault(a => a.Id == id);

            return (episode == null) ? null : mapper.Map<Episode, EpisodeWithShowNameViewModel>(episode);
        }

        public EpisodeVideoViewModel EpisodeVideoGetById(int id)
        {
            var episode = ds.Episodes.Find(id);

            return (episode == null) ? null : mapper.Map<Episode, EpisodeVideoViewModel>(episode);
        }

        public ActorBaseViewModel ActorMediaItemAdd(ActorMediaItemAddViewModel newItem)
        {
            var actor = ds.Actors.Find(newItem.ActorId);

            if (actor == null) return null;

            var addedItem = new ActorMediaItem();
            ds.ActorMediaItems.Add(addedItem);

            byte[] contentBytes = new byte[newItem.ContentUpload.ContentLength];
            newItem.ContentUpload.InputStream.Read(contentBytes, 0, newItem.ContentUpload.ContentLength);

            addedItem.Content = contentBytes;
            addedItem.ContentType = newItem.ContentUpload.ContentType;
            addedItem.Actor = actor;
            addedItem.Caption = newItem.Caption;
            
            ds.SaveChanges();

            return (addedItem == null) ? null : mapper.Map<Actor, ActorBaseViewModel>(actor);
            
        }

        // *** Add your methods ABOVE this line **

        #region Role Claims

        public List<string> RoleClaimGetAllStrings()
        {
            return ds.RoleClaims.OrderBy(r => r.Name).Select(r => r.Name).ToList();
        }

        #endregion

        #region Load Data Methods

        // Add some programmatically-generated objects to the data store
        // Write a method for each entity and remember to check for existing
        // data first.  You will call this/these method(s) from a controller action.
        public bool LoadRoles()
        {
            // User name
            var user = HttpContext.Current.User.Identity.Name;

            // Monitor the progress
            bool done = false;

            // *** Role claims ***
            if (ds.RoleClaims.Count() == 0)
            {
                ds.RoleClaims.Add(new RoleClaim() { Name = "Administrator" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Executive" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Coordinator" });
                ds.RoleClaims.Add(new RoleClaim() { Name = "Clerk" });

                ds.SaveChanges();
                done = true;
            }

            return done;
        }


        public bool LoadGenres()
        {

            if (ds.Genres.Count() > 0) return false;

            ds.Genres.Add(new Genre { Name = "Action" });
            ds.Genres.Add(new Genre { Name = "Adventure" });
            ds.Genres.Add(new Genre { Name = "Comedy" });
            ds.Genres.Add(new Genre { Name = "Drama" });
            ds.Genres.Add(new Genre { Name = "Fantasy" });
            ds.Genres.Add(new Genre { Name = "Historical" });
            ds.Genres.Add(new Genre { Name = "Horror" });
            ds.Genres.Add(new Genre { Name = "Mystery" });
            ds.Genres.Add(new Genre { Name = "Romance" });
            ds.Genres.Add(new Genre { Name = "Science Fiction" });

            ds.SaveChanges();
            return true;

        }

        public bool LoadActors()
        {

            var user = HttpContext.Current.User.Identity.Name;

            if (ds.Actors.Count() > 0) return false;

            ds.Actors.Add(new Actor
            {
                Name = "Oscar Isaac",
                AlternateName = "Issac",
                BirthDate = new DateTime(1979, 3, 9),
                Height = 1.74,
                ImageUrl = "https://resizing.flixster.com/uPxAfB-tzCzwXl7zok9pkZw0u6w=/fit-in/352x330/v2/https://resizing.flixster.com/-XZAfHZM39UwaGJIFWKAE8fS0ak=/v3/t/v9/AllPhotos/493617/493617_v9_bb.jpg",
                Executive = user
            });

            ds.Actors.Add(new Actor
            {
                Name = "Kevin Hart",
                AlternateName = "Lil Kev",
                BirthDate = new DateTime(1979, 7, 6),
                Height = 1.63,
                ImageUrl = "https://media.newyorker.com/photos/5c09b6cd52c7422cbbac087b/master/pass/Schulman-KevinHart.jpg",
                Executive = user
            });

            ds.Actors.Add(new Actor
            {
                Name = "Pedro Pascal",
                AlternateName = "Pascal",
                BirthDate = new DateTime(1975, 4, 2),
                Height = 1.80,
                ImageUrl = "https://cdn.britannica.com/41/240741-050-D4777963/Pedro-Pascal-attends-premiere-The-Last-of-US-January-2023.jpg",
                Executive = user
            });

            ds.SaveChanges();
            return true;
        }

        public bool LoadShows()
        {

            var user = HttpContext.Current.User.Identity.Name;

            if (ds.Shows.Count() > 0) return false;

            //var oscar = ds.Actors.SingleOrDefault(a => a.Name == "Oscar Issac");
            var pedro = ds.Actors.SingleOrDefault(a => a.Name == "Pedro Pascal");

            ds.Shows.Add(new Show
            {
                Actors = new Actor[] { pedro },
                Name = "The Mandalorian",
                Genre = "Science Fiction",
                ImageUrl = "https://m.media-amazon.com/images/M/MV5BNjgxZGM0OWUtZGY1MS00MWRmLTk2N2ItYjQyZTI1OThlZDliXkEyXkFqcGc@._V1_FMjpg_UX1000_.jpg",
                Coordinator = user,
                ReleaseDate = new DateTime(2019, 11, 12)
            });

            ds.Shows.Add(new Show
            {
                Actors = new Actor[] { pedro },
                Name = "The Last of Us",
                Genre = "Horror",
                ImageUrl = "https://upload.wikimedia.org/wikipedia/en/3/3e/The_Last_of_Us_season_1_Blu-ray.png",
                Coordinator = user,
                ReleaseDate = new DateTime(2023, 1, 15)
            });


            ds.SaveChanges();
            return true;
        } 

        public bool LoadEpisodes()
        {
            if (ds.Episodes.Count() > 0) return false;

            var user = HttpContext.Current.User.Identity.Name;

            var lastOfUs = ds.Shows.SingleOrDefault(s => s.Name == "The Last of Us");
            var madalorian = ds.Shows.SingleOrDefault(s => s.Name == "The Mandalorian");

            ds.Episodes.Add(new Episode
            {
                Name = "When You're Lost in the Darkness",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                Genre = "Horror",
                AirDate = new DateTime(2023, 1, 15),
                ImageUrl = "https://assets-prd.ignimgs.com/2022/12/22/12908211-1671745643746.jpg",
                Clerk = user,
                Show = lastOfUs
            });

            ds.Episodes.Add(new Episode
            {
                Name = "Infected",
                SeasonNumber = 1,
                EpisodeNumber = 2,
                Genre = "Horror",
                AirDate = new DateTime(2023, 1, 22),
                ImageUrl = "https://www.gameshub.com/wp-content/uploads/sites/5/2023/01/the-last-of-us-hbo-tv-series-episode-2.jpg",
                Clerk = user,
                Show = lastOfUs
            });

            ds.Episodes.Add(new Episode
            {
                Name = "Long, Long Time",
                SeasonNumber = 1,
                EpisodeNumber = 3,
                Genre = "Horror",
                AirDate = new DateTime(2023, 1, 29),
                ImageUrl = "https://cdn.mos.cms.futurecdn.net/8FxHbYZ3rfatwyMLHdTN4K.jpg",
                Clerk = user,
                Show = lastOfUs
            });

            ds.Episodes.Add(new Episode
            {
                Name = "The Mandalorian",
                SeasonNumber = 1,
                EpisodeNumber = 1,
                Genre = "Science Fiction",
                AirDate = new DateTime(2019, 11, 12),
                ImageUrl = "https://millermedianow.org/wp-content/uploads/2019/12/88A71D42-1B98-40F4-96C0-0F227E46C8E3-900x699.jpeg",
                Clerk = user,
                Show = madalorian
            });

            ds.Episodes.Add(new Episode
            {
                Name = "The Child",
                SeasonNumber = 1,
                EpisodeNumber = 2,
                Genre = "Science Fiction",
                AirDate = new DateTime(2019, 11, 15),
                ImageUrl = "https://ew.com/thmb/hDSuTYA3PHEzkmGBv7V1AScxPcU=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/huc-067228-2000-4d0814a383584238890beabff9382f3c.jpg",
                Clerk = user,
                Show = madalorian
            });

            ds.Episodes.Add(new Episode
            {
                Name = "The Sin",
                SeasonNumber = 1,
                EpisodeNumber = 3,
                Genre = "Science Fiction",
                AirDate = new DateTime(2019, 11, 22),
                ImageUrl = "https://m.media-amazon.com/images/M/MV5BNzkzMjU2OTItNmNlNS00N2UyLWFiMDktMzgyZjRmMGM3ZWRkXkEyXkFqcGc@._V1_.jpg",
                Clerk = user,
                Show = madalorian
            });

            ds.SaveChanges();
            return true;


        }

        public bool removeDatabaseData()
        {
            try
            {
                foreach (var e in ds.Episodes)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (var s in ds.Shows)
                {
                    ds.Entry(s).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (var a in ds.Actors)
                {
                    ds.Entry(a).State = System.Data.Entity.EntityState.Deleted;
                }

                foreach (var g in ds.Genres)
                {
                    ds.Entry(g).State = System.Data.Entity.EntityState.Deleted;
                }

                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public bool RemoveData()
        {
            try
            {
                foreach (var e in ds.RoleClaims)
                {
                    ds.Entry(e).State = System.Data.Entity.EntityState.Deleted;
                }

                // Remove additional entities as needed.

                ds.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool RemoveDatabase()
        {
            try
            {
                return ds.Database.Delete();
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

    #endregion

    #region RequestUser Class

    // This "RequestUser" class includes many convenient members that make it
    // easier work with the authenticated user and render user account info.
    // Study the properties and methods, and think about how you could use this class.

    // How to use...
    // In the Manager class, declare a new property named User:
    //    public RequestUser User { get; private set; }

    // Then in the constructor of the Manager class, initialize its value:
    //    User = new RequestUser(HttpContext.Current.User as ClaimsPrincipal);

    public class RequestUser
    {
        // Constructor, pass in the security principal
        public RequestUser(ClaimsPrincipal user)
        {
            if (HttpContext.Current.Request.IsAuthenticated)
            {
                Principal = user;

                // Extract the role claims
                RoleClaims = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

                // User name
                Name = user.Identity.Name;

                // Extract the given name(s); if null or empty, then set an initial value
                string gn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.GivenName).Value;
                if (string.IsNullOrEmpty(gn)) { gn = "(empty given name)"; }
                GivenName = gn;

                // Extract the surname; if null or empty, then set an initial value
                string sn = user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Surname).Value;
                if (string.IsNullOrEmpty(sn)) { sn = "(empty surname)"; }
                Surname = sn;

                IsAuthenticated = true;
                // You can change the string value in your app to match your app domain logic
                IsAdmin = user.HasClaim(ClaimTypes.Role, "Admin") ? true : false;
            }
            else
            {
                RoleClaims = new List<string>();
                Name = "anonymous";
                GivenName = "Unauthenticated";
                Surname = "Anonymous";
                IsAuthenticated = false;
                IsAdmin = false;
            }

            // Compose the nicely-formatted full names
            NamesFirstLast = $"{GivenName} {Surname}";
            NamesLastFirst = $"{Surname}, {GivenName}";
        }

        // Public properties
        public ClaimsPrincipal Principal { get; private set; }

        public IEnumerable<string> RoleClaims { get; private set; }

        public string Name { get; set; }

        public string GivenName { get; private set; }

        public string Surname { get; private set; }

        public string NamesFirstLast { get; private set; }

        public string NamesLastFirst { get; private set; }

        public bool IsAuthenticated { get; private set; }

        public bool IsAdmin { get; private set; }

        public bool HasRoleClaim(string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(ClaimTypes.Role, value) ? true : false;
        }

        public bool HasClaim(string type, string value)
        {
            if (!IsAuthenticated) { return false; }
            return Principal.HasClaim(type, value) ? true : false;
        }
    }

    #endregion

}