using Connect.Models;
using Connect.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Connect.Services
{
    public class SettingsService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public Settings GetSettings()
        {
            return db.Settings.FirstOrDefault();
        }
    }
}