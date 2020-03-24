using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using XeroNetStandardApp.Models;

namespace XeroNetStandardApp.Controllers
{
  public class HomeController : Controller
  {
    public IActionResult Index()
    {
      bool firstTimeConnection = false;

      if (TokenUtilities.TokenExists())
      {
        firstTimeConnection = true;
      }

      return View(firstTimeConnection);
    }
    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}


