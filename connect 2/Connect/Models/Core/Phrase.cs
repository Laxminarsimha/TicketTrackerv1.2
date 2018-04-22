using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Connect.Models.Core
{
    public class Phrase
    {
        public int Id { get; set; }
        [MaxLength(500)]
        [Index(IsUnique = true)]
        public string Text { get; set; }
    }
}