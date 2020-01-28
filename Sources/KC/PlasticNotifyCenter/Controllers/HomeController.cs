using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data.Managers;
using PlasticNotifyCenter.Models;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the default landing site
    /// </summary>
    public class HomeController : Controller
    {
        #region Dependencies

        private readonly ILogger<HomeController> _logger;
        private readonly ITriggerHistoryManager _triggerHistoryManager;
        private readonly INotificationRulesManager _notificationRulesManager;
        private readonly INotificationHistoryManager _notificationHistoryManager;

        public HomeController(ILogger<HomeController> logger,
                            ITriggerHistoryManager triggerHistoryManager,
                            INotificationRulesManager notificationRulesManager,
                            INotificationHistoryManager notificationHistoryManager)
        {
            _logger = logger;
            _triggerHistoryManager = triggerHistoryManager;
            _notificationRulesManager = notificationRulesManager;
            _notificationHistoryManager = notificationHistoryManager;
        }

        #endregion

        #region Start page (Index)

        [HttpGet("/Index")]
        public IActionResult Index()
        {
            // Show view
            return View(new StatsViewModel(
                _triggerHistoryManager.GetTriggerStats().ToArray(),
                _notificationRulesManager.GetRuleCount(),
                _notificationHistoryManager.GetNotificationStats().ToArray()
            ));
        }

        #endregion

        #region Error page

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Show an error page with information about the original request
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
