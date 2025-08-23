using Microsoft.AspNetCore.Mvc;
using QRCoder;
using OtpNet;
using System.Text;
using System.Diagnostics;

public class LoginController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string username, string password)
    {
        var user = FakeUserStore.Get(username);

        if (user != null && user.Password == password)
        {
            TempData["Username"] = user.Username;
            TempData.Keep("Username");

            if (user.IsMfaEnabled)
            {
                return RedirectToAction("Mfa");
            }
            else
            {
                return RedirectToAction("SetupMfa", "Register");
            }
        }

        ViewBag.Error = "Invalid credentials";
        return View();
    }

    [HttpGet]
    public IActionResult Mfa()
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost]
    public IActionResult Mfa(string code)
    {
        var username = HttpContext.Session.GetString("Username");

        if (string.IsNullOrEmpty(username))
            return RedirectToAction("Failure", "Account");

        var user = FakeUserStore.Get(username);
        if (user == null || string.IsNullOrEmpty(user.MfaSecret))
            return RedirectToAction("Failure", "Account");

        var totp = new Totp(Base32Encoding.ToBytes(user.MfaSecret));
        bool isValid = totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));

        if (isValid)
        {
            return RedirectToAction("Success", "Account");
        }

        ViewBag.Error = "Invalid MFA-code";
        TempData["Username"] = username;
        return View();
    }
}