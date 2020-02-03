using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Data
{
    /// <summary>
    /// Data context for this app
    /// </summary>
    public class PncDbContext : IdentityDbContext<User, Role, string,
                                IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
                                IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        #region DbSets

        /// <summary>
        /// Gets a single (first) settings entry
        /// </summary>
        public DbSet<AppSettings> AppSettings { get; set; }

        /// <summary>
        /// Gets a set of TriggerHistory entries
        /// </summary>
        public DbSet<TriggerHistory> TriggerHistory { get; set; }

        /// <summary>
        /// Gets a set of TriggerVariable entries
        /// </summary>
        public DbSet<TriggerVariable> TriggerVariables { get; set; }

        /// <summary>
        /// Gets a set of NotifierData entries (based on BaseNotifierData)
        /// </summary>
        public DbSet<BaseNotifierData> Notifiers { get; set; }

        /// <summary>
        /// Gets a set of NotificationRule entries
        /// </summary>
        public DbSet<NotificationRule> Rules { get; set; }

        /// <summary>
        /// Gets a set of RuleNotifier entries
        /// </summary>
        public DbSet<RuleNotifier> RuleNotifiers { get; set; }

        /// <summary>
        /// Gets a set of NotificationRecipient entries
        /// </summary>
        /// <value></value>
        public DbSet<NotificationRecipient> NotificationRecipients { get; set; }

        /// <summary>
        /// Gets a set of NotificationHistory entries
        /// </summary>
        public DbSet<NotificationHistory> NotificationHistory { get; set; }

        #endregion

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="options">Options</param>
        /// <remarks>
        /// Always use Dependency-Injection to get a data context
        /// <remarks>
        public PncDbContext(DbContextOptions<PncDbContext> options)
            : base(options)
        {
        }

        #region Model-Builder

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Find all notifier data classes, derived from BaseNotifierData
            // and add them to the model
            Console.WriteLine("Looking for NotifierData types...");
            var notifierDataBaseType = typeof(BaseNotifierData);
            notifierDataBaseType.Assembly
                .GetTypes()
                .Where(type => notifierDataBaseType.IsAssignableFrom(type))
                .Where(type => type.GetCustomAttributes(false).Any(a => a is NotifierAttribute))
                .ToList()
                .ForEach(type =>
                {
                    Console.WriteLine($"Found NotifierData type: {type.FullName}");
                    builder.Entity(type);
                });

            base.OnModelCreating(builder);

            // Setup navigational user / role properties
            builder.Entity<User>(b =>
            {
                // User -> Roles
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                // Role -> Users
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });
        }

        #endregion
    }
}
