using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlasticNotifyCenter.Data;
using PlasticNotifyCenter.Models;
using PlasticNotifyCenter.Services;

namespace PlasticNotifyCenter.Controllers
{
    /// <summary>
    /// Controller for the default landing site
    /// </summary>
    public class HomeController : Controller
    {
        #region Dependencies

        private readonly ILogger<HomeController> _logger;
        private readonly PncDbContext _dbContext;
        private readonly INotifierDefinitionService _notifierIconService;

        public HomeController(ILogger<HomeController> logger, PncDbContext dbContect, INotifierDefinitionService notifierIconService)
        {
            _logger = logger;
            _dbContext = dbContect;
            _notifierIconService = notifierIconService;
        }

        #endregion

        public IActionResult Index()
        {
            StatsViewModel model = new StatsViewModel();

            // Trigger stats
            model.TriggerStats = _dbContext.TriggerHistory
                                           .GroupBy(o => o.Trigger)
                                           .Select(g => new TriggerStats()
                                           {
                                               Name = g.Key,
                                               Count = g.Count()
                                           })
                                           .ToArray();
            // Rule stats
            model.RuleCount = _dbContext.Rules.Count();

            // Notifier Stats
            model.NotificationStats = _dbContext.NotificationHistory
                                            .GroupBy(o => o.NotifierName)
                                            .Select(g => new NotificationStats()
                                            {
                                                Notifier = g.Key,
                                                Icon = _notifierIconService.GetIcon(g.Key),
                                                SuccessCount = g.Sum(e => e.SuccessCount),
                                                FailedCount = g.Sum(e => e.FailedCount)
                                            })
                                            .ToArray();

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Show an error page with information about the original request
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
