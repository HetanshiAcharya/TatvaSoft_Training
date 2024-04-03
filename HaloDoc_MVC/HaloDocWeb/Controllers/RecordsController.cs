using Microsoft.AspNetCore.Mvc;

namespace HaloDocWeb.Controllers
{
    public class RecordsController : Controller
    {
        public IActionResult SearchRecords()
        {
            return View();
        }
    }
}
