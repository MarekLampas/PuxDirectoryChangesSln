using Microsoft.AspNetCore.Mvc;
using PuxDirectoryChanges.Helpers;
using PuxDirectoryChanges.Models;
using System.Diagnostics;

namespace PuxDirectoryChanges.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StatesHolder _statesHolder;

        public HomeController(ILogger<HomeController> logger, StatesHolder statesHolder)
        {
            _logger = logger;
            _statesHolder = statesHolder;
        }

        /// <summary>
        /// Landing page
        /// View for inserting a directory path
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            if (_statesHolder.IsDefinedDirectoryPath())
            {
                _statesHolder.CleanUpStatesHolder();
            }

            _logger.LogInformation("Index page displayed.");

            return View(new DirectoryModel());
        }

        /// <summary>
        /// Main page
        /// View for tracking changes in defined directory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IActionResult Changes(DirectoryModel model)
        {
            if (_statesHolder.IsDefinedDirectoryPath())
            {
                model.DirectoryPath = _statesHolder.DirectoryPath;
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(nameof(Index), model);
                }
            }

            if (!Directory.Exists(model.DirectoryPath))
            {
                ModelState.AddModelError("DirectoryPath", "Path does not exist.");

                _logger.LogInformation($"Entered direvory path do not exists: {model.DirectoryPath}");

                return View(nameof(Index), model);
            }

            if (!_statesHolder.SetupDirectoryState(model.DirectoryPath))
            {
                ModelState.AddModelError("DirectoryPath", "Something went wrong. Please try again.");

                if (_statesHolder.IsDefinedDirectoryPath())
                {
                    _statesHolder.CleanUpStatesHolder();
                }

                return View(nameof(Index), model);
            }

            _logger.LogInformation("Changes page displayed.");

            return View(new ChangesModel() { FileStates = _statesHolder.GetFileStatesToDisplay() });
        }

        /// <summary>
        /// About page
        /// View containing app description
        /// </summary>
        /// <returns></returns>
        public IActionResult About()
        {
            _logger.LogInformation("About page displayed.");

            return View();
        }

        /// <summary>
        /// Error page
        /// View that ocures when an unhandled exeption is thrown
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogInformation("Error page displayed.");

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}