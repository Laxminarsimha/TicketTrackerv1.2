using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Connect.Models.Core.ViewModels
{
    public class QuestionViewModel
    {
        [Required(ErrorMessage = "Question is mandatory")]
        public string question { get; set; }
    }
}