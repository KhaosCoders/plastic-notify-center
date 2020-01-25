using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Controllers.Api;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Filters;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Mail;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Data.Identity;
using PlasticNotifyCenter.Utils;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Setup section
    /// </summary>
    [NoSetup]
    [AllowAnonymous]
    public class SetupController : Controller
    {
        #region Dependencies

        private readonly PncDbContext _dbContext;
        private readonly ILogger<SetupController> _logger;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;


        public SetupController(ILogger<SetupController> logger,
                               PncDbContext dbContect,
                               IMailService mailService,
                               UserManager<User> userManager,
                               RoleManager<Role> roleManager)
        {
            _dbContext = dbContect;
            _logger = logger;
            _mailService = mailService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        #region Index

        public IActionResult Index()
        {
            // Block request when setup was completed before
            if (_dbContext.AppSettings.FirstOrDefault() != null)
            {
                return BadRequest();
            }
            return View(new SetupViewModel());
        }

        #endregion

        #region Plastic CLI

        /*
         * Methods used to localize and configure Plastic SCM cm cli. But it's not needed...
         *
        public async Task<IActionResult> ConnectToServerAsync([FromQuery] string CMPath, [FromQuery] string Host, [FromQuery] int Port, [FromQuery] string WorkingMode, [FromQuery] string User, [FromQuery] string Password)
        {
            if (string.IsNullOrWhiteSpace(Host) || Port <= 0 || string.IsNullOrWhiteSpace(WorkingMode))
            {
                return BadRequest();
            }

            if (await new PlasticCLI(CMPath).SetupClient(Host, Port, WorkingMode, User, Password))
            {
                return Ok(new SuccessResponse());
            }
            return Ok(new ErrorRespose(new Exception("Connection to Plastic SCM Server not possible or user has not enough rights to read licence information.")));
        }
        public async Task<IActionResult> TestCMPathAsync([FromQuery] string CMPath)
        {
            if (string.IsNullOrWhiteSpace(CMPath))
            {
                return BadRequest();
            }
            try
            {
                string cmVersion = await new PlasticCLI(CMPath).GetVersionAsync();
                return Ok(new StringValueRespose(cmVersion));
            }
            catch (CLIException ex)
            {
                return Ok(new ErrorRespose(ex));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Unexpected exception while getting CM version with CMPath {0}", CMPath);
                return BadRequest();
            }
        }
        */

        #endregion

        #region Trigger test

        /// <summary>
        /// Returns a Json-Object string with information about trigger invoke counts
        /// </summary>
        public async Task<IActionResult> TriggerCountsAsync()
        {
            // Group trigger call history by type and count them
            var triggerCallCount = _dbContext.TriggerHistory
                                        .GroupBy((entry) => entry.Trigger)
                                        .Select((group) => new { Trigger = group.Key, Count = group.Count() })
                                        .ToArray();

            // Convert result to Json
            string message = await JsonHelper.StringifyAsync(triggerCallCount);

            return Ok(message);
        }

        #endregion

        #region Validate Password

        [HttpPost]
        public async Task<IActionResult> ValidatePWAsync([FromForm] string password)
        {
            foreach (var validator in _userManager.PasswordValidators)
            {
                // Check all validators for errors
                var result = await validator.ValidateAsync(_userManager, null, password);
                if (!result.Succeeded)
                {
                    return Ok(new FailureResponse(String.Join(" ", result.Errors.Select(e => e.Description).ToArray())));
                }
            }
            // No errors
            return Ok(new SuccessResponse());
        }

        #endregion

        #region Smtp Test Mail

        [HttpPost]
        public async Task<IActionResult> SmtpTestAsync()
        {
            // Read POST body to SmtpMailTest object
            SmtpConfiguration smtpTest;
            using (var reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }
                smtpTest = await SmtpConfiguration.ParseJsonAsync(body);
            }

            // Validate data
            if (smtpTest == null || !smtpTest.ValidateTest())
            {
                return Ok(new FailureResponse("SMTP test data not valid"));
            }

            try
            {
                // Perform test
                if (!_mailService.SendTestMail(smtpTest))
                {
                    return Ok(new FailureResponse("SMTP test failed"));
                }

                // Return success
                return Ok(new SuccessResponse());
            }
            catch (SmtpException ex)
            {
                // Failed by exception
                return Ok(new ErrorRespose(ex));
            }
        }

        #endregion

        #region Save setup

        [HttpPost]
        public async Task<IActionResult> CompleteSetupAsync()
        {
            // Block request when setup was completed before
            if (_dbContext.AppSettings.FirstOrDefault() != null)
            {
                return BadRequest();
            }

            // Read POST body to SmtpMailTest object
            InitialConfiguration config;
            using (var reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }
                config = await InitialConfiguration.ParseJsonAsync(body);
            }

            // Validate data
            if (config == null || !config.Validate())
            {
                return Ok(new FailureResponse("Configuration data not valid"));
            }

            // Create admin user
            var adminUser = new User(config.AdminUsername)
            {
                Email = config.AdminEmail,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(adminUser, config.AdminPw);
            if (!result.Succeeded)
            {
                return Ok(new FailureResponse(string.Join(' ', result.Errors.Select(err => err.Description))));
            }

            // Create roles
            await AddBuildInUserRole(Roles.AdminRole);
            await AddBuildInUserRole(Roles.CoordinatorRole);
            await AddBuildInUserRole(Roles.UserRole);

            // Add admin role to admin user
            await _userManager.AddToRoleAsync(adminUser, Roles.AdminRole);
            // Also add user role
            await _userManager.AddToRoleAsync(adminUser, Roles.UserRole);

            // Store basic app settings
            _dbContext.AppSettings.Add(new AppSettings(config.BaseUrl));

            // Store SMTP notifier
            _dbContext.Notifiers.Add(SmtpNotifierData.CreateFrom(config.Smtp));

            // Save
            await _dbContext.SaveChangesAsync();

            return Ok(new SuccessResponse());
        }

        /// <summary>
        /// Adds a role if it doesn't exist
        /// </summary>
        /// <param name="roleName">Name of role</param>
        private async Task AddBuildInUserRole(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new Role(roleName)
                {
                    IsBuildIn = true
                };
                await _roleManager.CreateAsync(role);
            }
        }

        #endregion
    }
}