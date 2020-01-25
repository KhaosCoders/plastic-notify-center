using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace PlasticNotifyCenter.Authorization
{
    /// <summary>
    /// Build-In roles (groups)
    /// </summary>
    public static class Roles
    {
        /// <summary>
        /// The administrative role
        /// </summary>
        public const string AdminRole = "Administrators";

        /// <summary>
        /// The cooridnator role
        /// </summary>
        public const string CoordinatorRole = "Coordinators";

        /// <summary>
        /// The user role
        /// </summary>
        public const string UserRole = "Users";
    }

    /// <summary>
    /// Requirements of a roles
    /// </summary>
    public static class RoleRequirements
    {
        /// <summary>
        /// Requirement of administrative role
        /// </summary>
        public static AdminRoleRequirement AdminRoleRequirement = new AdminRoleRequirement();

        /// <summary>
        /// Requirement of coordintor role
        /// </summary>
        public static CoordinatorRoleRequirement CoordinatorRoleRequirement = new CoordinatorRoleRequirement();
    }

    /// <summary>
    /// Requirement of administrative role
    /// </summary>
    public class AdminRoleRequirement : IAuthorizationRequirement { }

    /// <summary>
    /// Requirement of coordintor role
    /// </summary>
    public class CoordinatorRoleRequirement : IAuthorizationRequirement { }
}