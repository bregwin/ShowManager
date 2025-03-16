using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BJ2247A5.Data
{
    public class ActorMediaItem
    {
        public int Id { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public string Caption { get; set; }

        public Actor Actor { get; set; }
    }
}