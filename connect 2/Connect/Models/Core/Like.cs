using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Connect.Models.Core
{
    public class Like
    {
        public int Id { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Question Question { get; set; }

        [Index("QUES_USER", Order = 1, IsUnique = true)]
        public string UserId { get; set; }
        [Index("QUES_USER", Order = 2, IsUnique = true)]
        public int QuestionId { get; set; }
    }
}