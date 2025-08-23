using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mfa.Models;
public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Success()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Logout()
    {
        //TempData.Clear();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Error501()
    {
        Response.StatusCode = 501;
        return View("501");
    }

    [HttpGet]
    public IActionResult Failure()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Failure2()
    {
        return View();
    }
}