using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Connect.Models.Core
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        public string question { get; set; }
        public DateTime dateCreated { get; set; }
        [Required]
        public virtual ApplicationUser createdBy { get; set; }

        public string answer { get; set; }
        public virtual ApplicationUser answeredBy { get; set; }
        public DateTime? dateAnswered { get; set; }

        public bool Important { get; set; }
        public int Rating { get; set; }

        public virtual ICollection<Like> Likes { get; set; }
    }
}