namespace Connect.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Connect.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Connect.Models.ApplicationDbContext context)
        {
            List<string> systemRoles = new List<string>() { "Admin", "Respond" };
            foreach (string systemRole in systemRoles)
            {
                var existingRole = from r in context.Roles where r.Name.CompareTo(systemRole) == 0 select r;
                if (existingRole == null)
                {
                    IdentityRole role = new IdentityRole { Name = systemRole };
                    context.Entry(role).State = EntityState.Added;
                }
            }
            context.SaveChanges();
        }
    }
}
