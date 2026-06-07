using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyVerse.Controllers
{
    [Authorize]
    public class PomodoroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}