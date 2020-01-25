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

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller of /Admin section
    /// </summary>
    public class AdminController : Controller
    {
        #region Dependencies

        private readonly ILogger<AdminController> _logger;
        private readonly PncDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAuthorizationService _authorizationService;

        public AdminController(ILogger<AdminController> logger,
                               PncDbContext dbContect,
                               UserManager<User> userManager,
                               RoleManager<Role> roleManager,
                               IAuthorizationService authorizationService)
        {
            _dbContext = dbContect;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _authorizationService = authorizationService;
        }

        #endregion

        #region Index

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();
            return View(appSettings);
        }

        [HttpPost]
        public async Task<IActionResult> IndexAsync([FromForm] string baseUrl, [FromForm] string allowRegistration)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();

            // Update record
            appSettings.BaseUrl = baseUrl;
            appSettings.AllowRegistration = allowRegistration?.Equals("on", System.StringComparison.CurrentCultureIgnoreCase) ?? false;

            // Save
            await _dbContext.SaveChangesAsync();

            return View(appSettings);
        }

        #endregion

        #region Users

        [HttpGet]
        public async Task<IActionResult> UsersAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Show all users in view
            return View(new UsersViewModel()
            {
                Users = _userManager.Users.ToArray()
            });
        }

        [HttpGet]
        public async Task<IActionResult> AddUserAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Show edit user form with new user template
            return View("edit_user", new EditUserViewModel()
            {
                User = new User(),
                IsNewUser = true
            });
        }

        [HttpGet]
        public async Task<IActionResult> UserAsync(string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
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
            return View("edit_user", new EditUserViewModel() { User = user });
        }

        [HttpPost]
        public async Task<IActionResult> UserAsync(string id,
                                        [FromForm] string name,
                                        [FromForm] string email,
                                        [FromForm] string password)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // New user or edited?
            User user;

            if (string.IsNullOrWhiteSpace(id))
            {
                // New user
                if (string.IsNullOrWhiteSpace(password))
                {
                    // New users need a password
                    return BadRequest();
                }
                user = new User(name);
            }
            else
            {
                // Find user by ID
                user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
            }

            // Set properties
            user.UserName = name;
            user.Email = email;
            user.EmailConfirmed = true;

            if (string.IsNullOrWhiteSpace(id))
            {
                // Add new user
                if (!(await _userManager.CreateAsync(user, password)).Succeeded)
                {
                    return BadRequest();
                }
                // All users get the User role
                await _userManager.AddToRoleAsync(user, Roles.UserRole);
            }
            else
            {
                // Change password?
                if (!string.IsNullOrWhiteSpace(password))
                {
                    // validate password
                    if (_userManager.PasswordValidators.Any(v => !v.ValidateAsync(_userManager, user, password).Result.Succeeded))
                    {
                        return BadRequest();
                    }
                    // hash password
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);
                }

                // save user changes
                if (!(await _userManager.UpdateAsync(user)).Succeeded)
                {
                    return BadRequest();
                }
            }

            // Return to users view
            return RedirectToAction("Users", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DelUserAsync([FromForm] string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
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

            // Delete user
            await _userManager.DeleteAsync(user);

            // Return to users view
            return RedirectToAction("Users", "Admin");
        }

        #endregion

        #region Groups

        [HttpGet]
        public async Task<IActionResult> GroupsAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Show all roles in the groups view
            return View(new GroupsViewModel()
            {
                Roles = _roleManager.Roles.ToArray()
            });
        }

        [HttpGet]
        public async Task<IActionResult> AddGroupAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // Show the edit group form with a new group template
            return View("edit_group", new EditGroupViewModel()
            {
                Role = new Role(),
                IsNewRole = true,
                UsersInRole = new User[0],
                UserNotInRole = _userManager.Users.ToArray()
            });
        }

        [HttpGet]
        public async Task<IActionResult> GroupAsync(string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
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
            var usersInRole = _userManager.Users
                                    .AsEnumerable()
                                    .Where(u => _userManager.IsInRoleAsync(u, role.Name).Result)
                                    .ToArray();
            // Users without this role
            var usersNotinRole = _userManager.Users
                                    .AsEnumerable()
                                    .Except(usersInRole)
                                    .ToArray();

            // Show edit group form
            return View("edit_group", new EditGroupViewModel()
            {
                Role = role,
                UsersInRole = usersInRole,
                UserNotInRole = usersNotinRole
            });
        }

        [HttpPost]
        public async Task<IActionResult> GroupAsync(string id,
                                                    [FromForm] string name,
                                                    [FromForm] string[] users)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // New role or edited?
            Role role;

            if (string.IsNullOrWhiteSpace(id))
            {
                // New role
                role = new Role(name);
            }
            else
            {
                // Find role by ID
                role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }
            }

            // Set properties
            role.Name = name;

            if (string.IsNullOrWhiteSpace(id))
            {
                // Add new role
                if (!(await _roleManager.CreateAsync(role)).Succeeded)
                {
                    return BadRequest();
                }
            }
            else
            {
                // Save role
                if (!(await _roleManager.UpdateAsync(role)).Succeeded)
                {
                    return BadRequest();
                }
            }

            // User assignments
            // Cycle through all users
            foreach (var user in _userManager.Users)
            {
                // Has user the role currently?
                var inRole = await _userManager.IsInRoleAsync(user, role.Name);
                if (inRole && !users.Contains(user.Id))
                {
                    // User should not have the role anymore
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else if (!inRole && users.Contains(user.Id))
                {
                    // User should have the role now
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
            }

            // Return to the groups view
            return RedirectToAction("Groups", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> DelGroupAsync([FromForm] string id)
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
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

            // Build-In roles can't be deleted
            if (role.IsBuildIn)
            {
                return Forbid();
            }

            // Delete role
            await _roleManager.DeleteAsync(role);

            // Return to the groups view
            return RedirectToAction("Groups", "Admin");
        }

        #endregion

        #region LDAP

        [HttpGet]
        public async Task<IActionResult> LDAPAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // LDAP settings are part of AppSettings
            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();
            return View(appSettings);
        }

        #endregion

        #region Triggers

        [HttpGet]
        public async Task<IActionResult> TriggersAsync()
        {
            // Check authorization
            if (!(await _authorizationService.AuthorizeAsync(User, null, RoleRequirements.AdminRoleRequirement)).Succeeded)
            {
                return Unauthorized();
            }

            // AppSettings has only one record
            var appSettings = _dbContext.AppSettings.First();
            return View(appSettings);
        }

        #endregion
    }
}