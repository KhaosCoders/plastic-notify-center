using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;
using PlasticNotifyCenter.Controllers.Api;
using System;
using PlasticNotifyCenter.Data.Managers;
using System.Collections.Generic;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller of /Admin section
    /// </summary>
    public class AdminController : Controller
    {
        #region Dependencies

        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILdapService _ldapService;
        private readonly IAppSettingsManager _appSettingsManager;
        private readonly INotificationRulesManager _rulesManager;

        public AdminController(ILogger<AdminController> logger,
                               UserManager<User> userManager,
                               RoleManager<Role> roleManager,
                               IAuthorizationService authorizationService,
                               ILdapService ldapService,
                               IAppSettingsManager appSettingsManager,
                               INotificationRulesManager rulesManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
            _ldapService = ldapService;
            _appSettingsManager = appSettingsManager;
            _rulesManager = rulesManager;
        }

        #endregion

        #region Index

        [HttpGet("/Admin/Index")]
        public async Task<IActionResult> IndexAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // AppSettings has only one record
            return View(_appSettingsManager.AppSettings);
        }

        [HttpPost("/Admin/Index")]
        public async Task<IActionResult> SaveIndexAsync([FromForm] string baseUrl, [FromForm] string allowRegistration)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Change settings
            await _appSettingsManager.SaveSettingsAsync(
                baseUrl,
                allowRegistration?.Equals("on", System.StringComparison.CurrentCultureIgnoreCase) ?? false);

            // Show same page
            return RedirectToAction("Index");
        }

        #endregion

        #region Users

        [HttpGet("/Admin/Users")]
        public async Task<IActionResult> UsersAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Show all users in view
            return View(new UsersViewModel(_userManager.GetOrderedUsers().ToArray()));
        }

        [HttpGet("/Admin/AddUser")]
        public async Task<IActionResult> AddUserAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Show edit user form with new user template
            return View("edit_user", new EditUserViewModel(new User(), true));
        }

        [HttpGet("/Admin/User/{id}")]
        public async Task<IActionResult> UserAsync([FromRoute] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find user by ID
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Show edit user form
            return View("edit_user", new EditUserViewModel(user, false));
        }

        [HttpPost("/Admin/User/{id}")]
        public async Task<IActionResult> SaveUserAsync(
                                        [FromRoute] string id,
                                        [FromForm] string name,
                                        [FromForm] string email,
                                        [FromForm] string password)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            try
            {
                // Try to save changed or added user
                await _userManager.SaveUserAsync(id, name, password, email);
            }
            catch (ArgumentException aex)
            {
                return BadRequest(aex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return BadRequest(oex.Message);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }

            // Return to users view
            return RedirectToAction("Users", "Admin");
        }

        [HttpDelete("/Admin/User")]
        public async Task<IActionResult> DeleteUserAsync([FromForm] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Try to delete user and recipients
            try
            {
                await _userManager.DeleteByIdAsync(id, _rulesManager);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return BadRequest(oex.Message);
            }

            // Return to users view
            return RedirectToAction("Users", "Admin");
        }

        #endregion

        #region Groups

        [HttpGet("/Admin/Groups")]
        public async Task<IActionResult> GroupsAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Show all roles in the groups view
            return View(new GroupsViewModel(_roleManager.GetOrderedRoles().ToArray()));
        }

        [HttpGet("/Admin/AddGroup")]
        public async Task<IActionResult> AddGroupAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Show the edit group form with a new group template
            return View("edit_group", new EditGroupViewModel(
                new Role(),
                true,
                // New role: no users assigned jet, all other active users free to assign
                new User[0],
                _userManager.GetOrderedUsers().ToArray()
            ));
        }

        [HttpGet("/Admin/Group/{id}")]
        public async Task<IActionResult> GroupAsync([FromRoute] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Find role by ID
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // User with this role
            var usersInRole = _userManager.GetOrderedUsersOfRole(role).ToArray();

            // Users without this role
            var usersNotInRole = _userManager
                                    .GetOrderedUsers()
                                    .Except(usersInRole)
                                    .ToArray();

            // Show edit group form
            return View("edit_group", new EditGroupViewModel(
                role,
                false,
                usersInRole,
                usersNotInRole
            ));
        }

        [HttpPost("/Admin/Group/{id}")]
        public async Task<IActionResult> SaveGroupAsync(
                                            [FromRoute] string id,
                                            [FromForm] string name,
                                            [FromForm] string[] users)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            try
            {
                // Try to save changed or added role (group)
                await _roleManager.SaveRoleAsync(_userManager, id, name, users);
            }
            catch (InvalidOperationException oex)
            {
                return BadRequest(oex.Message);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }

            // Return to the groups view
            return RedirectToAction("Groups", "Admin");
        }

        [HttpDelete("/Admin/Group")]
        public async Task<IActionResult> DeleteGroupAsync([FromForm] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            // Try to delete role and recipients
            try
            {
                await _roleManager.DeleteByIdAsync(id, _rulesManager);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }
            catch (InvalidOperationException oex)
            {
                return BadRequest(oex.Message);
            }

            // Return to the groups view
            return RedirectToAction("Groups", "Admin");
        }

        #endregion

        #region LDAP

        [HttpGet("/Admin/LDAP")]
        public async Task<IActionResult> LDAPAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // LDAP settings are part of AppSettings
            // AppSettings has only one record
            return View(_appSettingsManager.AppSettings);
        }


        [HttpPost("/Admin/LDAP")]
        public async Task<IActionResult> SaveLDAPAsync([FromForm] LdapSettings ldapConfig)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Update config
            await _appSettingsManager.ChangeLdapConfig(ldapConfig);

            // Reload view
            return RedirectToAction("LDAP");
        }

        [HttpPost("/Admin/TestLDAP")]
        public async Task<IActionResult> TestLDAPAsync([FromForm] LdapSettings ldapConfig)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            _logger.LogDebug("Testing LDAP for config: {0}", ldapConfig);

            int userCount = 0;
            int groupCount = 0;
            try
            {
                // Combine host, port and protocol to a single string
                string ldapStr = _ldapService.GetLdapStr(ldapConfig);

                // Try to connect
                if (!_ldapService.TestConnection(ldapStr))
                {
                    _logger.LogWarning("LDAP connection test failed");
                    return Ok(new FailureResponse("Connection test failed: Check hostname and credientials"));
                }

                // Count entries
                userCount = _ldapService.GetUserCount(ldapConfig);
                groupCount = _ldapService.GetGroupCount(ldapConfig);

                // Test user attributes
                if (userCount > 0
                    && !_ldapService.TestUserAttributes(ldapConfig))
                {
                    _logger.LogWarning("User attribute test failed");
                    return Ok(new FailureResponse("User attributes not found"));
                }

                // Test group attributes
                if (groupCount > 0
                    && !_ldapService.TestGroupAttrbutes(ldapConfig))
                {
                    _logger.LogWarning("Group attribute test failed");
                    return Ok(new FailureResponse("Group attributes not found"));
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error while LDAP test");
                return Ok(new ErrorRespose(e));
            }

            // Return success message
            _logger.LogDebug("LDAP test successfull. Found {0} users and {1} groups.", userCount, groupCount);
            return Ok(new StringValueRespose($"Found {userCount} users and {groupCount} groups"));
        }

        #endregion

        #region Triggers

        [HttpGet("/Admin/Triggers")]
        public async Task<IActionResult> TriggersAsync()
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Show triggers view
            return View(_appSettingsManager.AppSettings);
        }

        #endregion
    }
}