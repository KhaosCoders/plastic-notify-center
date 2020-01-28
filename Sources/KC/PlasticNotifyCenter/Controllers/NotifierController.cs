using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Authorization;
using PlasticNotifyCenter.Controllers.Api;
using PlasticNotifyCenter.Data.Managers;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Notifier section
    /// </summary>
    public class NotifierController : Controller
    {
        #region Dependencies

        private readonly ILogger<NotifierController> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly INotifierDefinitionService _notifierDefinitionService;
        private readonly INotifierManager _notifierManager;

        public NotifierController(ILogger<NotifierController> logger,
                                  IAuthorizationService authorizationService,
                                  INotifierDefinitionService notifierIconService,
                                  INotifierManager notifierManager)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _notifierDefinitionService = notifierIconService;
            _notifierManager = notifierManager;
        }

        #endregion

        [HttpGet("/Notifier/{id}")]
        public async Task<IActionResult> IndexAsync([FromRoute] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            NotifierViewModel model = new NotifierViewModel();

            // All supported types of notifiers
            model.NotifierTypes = _notifierDefinitionService.GetAllNotifierTypes();

            // List all created notifiers
            model.Notifiers = _notifierManager.GetOrderedNotifiers().ToArray();

            if (!string.IsNullOrWhiteSpace(id))
            {
                // Select a specific notifier
                var notifier = _notifierManager.GetNotifierById(id);
                if (notifier == null)
                {
                    return NotFound();
                }
                model.SelectedNotifier = notifier;
            }
            else if (model.Notifiers.Length > 0)
            {
                // Select the first notfier (or none)
                model.SelectedNotifier = _notifierManager.GetFirstNotifier();
            }

            return View(model);
        }

        [HttpPost("/Notifier/{id}")]
        public async Task<IActionResult> SaveNotifierAsync([FromRoute] string id)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Read POST body
            string body = string.Empty;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }
            }

            // Try to save notifier changes
            try
            {
                await _notifierManager.ChangeNotifierAsync(id, body);
            }
            catch (KeyNotFoundException kex)
            {
                return Ok(new FailureResponse(kex.Message));
            }

            // Return success response
            return Ok(new SuccessResponse());
        }

        [HttpPost("/Notifier/New")]
        public async Task<IActionResult> NewAsync([FromForm] string typedId)
        {
            // Check authorization
            if (!(await User.IsAdminAsync(_authorizationService)))
            {
                return Unauthorized();
            }

            // Check parameter
            if (string.IsNullOrWhiteSpace(typedId))
            {
                return BadRequest();
            }

            // Creates a new notifier of given type
            try
            {
                string newId = await _notifierManager.NewNotifierAsync(typedId);

                // Redirect to notifiers view (with new notifier selected)
                return new RedirectToActionResult("Index", "Notifier", new { @Id = newId });
            }
            catch (TypeAccessException tex)
            {
                return NotFound(tex.Message);
            }
        }

        [HttpDelete("/Notifier/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] string id)
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

            try
            {
                // Try to delete notifier
                await _notifierManager.DeleteNotifierByIdAsync(id);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(kex.Message);
            }

            // Redirect to notifiers view
            return RedirectToAction("Index");
        }
    }
}