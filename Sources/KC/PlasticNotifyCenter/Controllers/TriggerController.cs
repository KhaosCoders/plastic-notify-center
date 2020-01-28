using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Controllers.Api;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Data.Managers;
using PlasticNotifyCenter.Filters;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Notifiers;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the /Trigger section
    /// Handles Trigger calls
    /// </summary>
    [NoSetup]
    [AllowAnonymous]
    public class TriggerController : Controller
    {
        #region Dependencies

        private readonly ILogger<TriggerController> _logger;
        private readonly INotificationQueue _notificationQueue;
        private readonly ITriggerHistoryManager _triggerHistoryManager;

        public TriggerController(ILogger<TriggerController> logger,
                                INotificationQueue notificationQueue,
                                ITriggerHistoryManager triggerHistoryManager)
        {
            _logger = logger;
            _notificationQueue = notificationQueue;
            _triggerHistoryManager = triggerHistoryManager;
        }

        #endregion

        #region Handle trigger call

        [HttpPost("/Trigger/Fire/{type}")]
        public async Task<IActionResult> FireAsync([FromRoute] string type)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest();
            }

            // Read POST body to TriggerCall object
            TriggerCall call = null;
            using (var reader = new StreamReader(Request.Body))
            {
                string body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body))
                {
                    return BadRequest();
                }

                try
                {
                    // Parse post body
                    call = TriggerCall.ParseJson(body, type);
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Error while parsing trigger call");
                    return BadRequest();
                }
                catch (InvalidOperationException ex)
                {
                    _logger.LogError(ex, "Error while parsing trigger call");
                    return BadRequest();
                }
            }

            // Handle the rest in the background
            _notificationQueue.QueueTriggerCall(call);

            // Return Success message to Plastic SCM
            return Ok("Trigger ok");
        }

        #endregion

        #region Trigger variables

        [HttpGet("/Trigger/Vars/{type}")]
        public IActionResult Vars([FromRoute] string type)
        {
            // Check parameter
            if (string.IsNullOrWhiteSpace(type))
            {
                return BadRequest();
            }

            // User should be loged-in
            if (User == null)
            {
                return Forbid();
            }

            // Get list of variables for last trigger call
            var vars = _triggerHistoryManager.GetEnvironmentVariables(type).ToArray();
            if ((vars?.Length ?? 0) == 0)
            {
                return Ok(new FailureResponse("No variables recorded!"));
            }
            var varDictionary = vars.ToDictionary(v => v.Variable, v => v.Value);

            // Special INPUT var
            var triggerHistory = _triggerHistoryManager.GetLatestTriggerHistory(type);
            if (!string.IsNullOrWhiteSpace(triggerHistory?.Input))
            {
                varDictionary.Add("Input (Type: string[])", triggerHistory.Input);
            }

            // Return variables with values as key-value-array
            return Ok(new KeyValueArrayResponse(varDictionary));
        }

        #endregion
    }
}