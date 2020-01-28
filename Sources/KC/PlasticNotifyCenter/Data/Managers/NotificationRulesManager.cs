using System.Linq;
using System.Threading.Tasks;
using PlasticNotifyCenter.Data.Identity;

namespace PlasticNotifyCenter.Data.Managers
{
    /// <summary>
    /// Manages notification rules
    /// </summary>
    public interface INotificationRulesManager
    {
        /// <summary>
        /// Removes a user from all rules where he is a recipient
        /// </summary>
        /// <param name="user">User to remove</param>
        Task DeleteUserRecipientAsync(User user);

        /// <summary>
        /// Removes a role from all rules where it is a recipient
        /// </summary>
        /// <param name="role">Role to remove</param>
        Task DeleteRoleRecipientAsync(Role role);

        /// <summary>
        /// Returns the number or defined rules
        /// </summary>
        int GetRuleCount();
    }

    /// <summary>
    /// Implementation of INotificationRulesManager
    /// </summary>
    public class NotificationRulesManager : INotificationRulesManager
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;

        public NotificationRulesManager(PncDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Delete Recipients

        /// <summary>
        /// Removes a user from all rules where he is a recipient
        /// </summary>
        /// <param name="user">User to remove</param>
        public async Task DeleteUserRecipientAsync(User user)
        {
            // Find all roles where the user is a recipient
            var rules = _dbContext.Rules
                .ToList()
                .Select(rule => new { Rule = rule, Recipient = rule.Recipients.FirstOrDefault(recipient => recipient.User == user) })
                .Where(rule => rule.Recipient != null)
                .ToList();

            // First remove the user from recipient entry (because foreign-key-constraint)
            rules.ForEach(rule => rule.Recipient.User = null);
            await _dbContext.SaveChangesAsync();

            // Then remove the recipient entries
            rules.ForEach(rule => rule.Rule.Recipients.Remove(rule.Recipient));
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes a role from all rules where it is a recipient
        /// </summary>
        /// <param name="role">Role to remove</param>
        public async Task DeleteRoleRecipientAsync(Role role)
        {
            // Find all roles where the user is a recipient
            var rules = _dbContext.Rules
                .ToList()
                .Select(rule => new { Rule = rule, Recipient = rule.Recipients.FirstOrDefault(recipient => recipient.Role == role) })
                .Where(rule => rule.Recipient != null)
                .ToList();

            // First remove the role from recipient entry (because foreign-key-constraint)
            rules.ForEach(rule => rule.Recipient.Role = null);
            await _dbContext.SaveChangesAsync();

            // Then remove the recipient entries
            rules.ForEach(rule => rule.Rule.Recipients.Remove(rule.Recipient));
            await _dbContext.SaveChangesAsync();
        }

        #endregion

        #region Rule count

        /// <summary>
        /// Returns the number or defined rules
        /// </summary>
        public int GetRuleCount() =>
            _dbContext.Rules.Count();

        #endregion


    }
}