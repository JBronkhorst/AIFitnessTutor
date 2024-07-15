using AIFitnessTutor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AIFitnessTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<PersonalInfo> PersonInfos { get; set; }
        public DbSet<ActivityPlan> ActivityPlans { get; set; }
        public DbSet<NutritionPlan> NutritionPlans { get; set; }
        public DbSet<BMI> BMIs { get; set; }
    }
}
