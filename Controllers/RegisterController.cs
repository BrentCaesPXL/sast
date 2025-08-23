using Microsoft.AspNetCore.Mvc;
using QRCoder;
using OtpNet;

public class RegisterController : Controller
{
    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    public IActionResult Index(string username, string password)
    {
        if (FakeUserStore.Exists(username))
        {
            ViewBag.Error = "Username already exists.";
            return View();
        }

        var newUser = new User
        {
            Username = username,
            Password = password
        };

        FakeUserStore.Add(newUser);

        HttpContext.Session.SetString("Username", newUser.Username);

        return RedirectToAction("SetupMfa");
    }

    [HttpGet]
    public IActionResult SetupMfa()
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null) return RedirectToAction("Index");

        var user = FakeUserStore.Get(username);

        if (string.IsNullOrEmpty(user.MfaSecret))
        {
            var secret = KeyGeneration.GenerateRandomKey(20);
            user.MfaSecret = Base32Encoding.ToString(secret);
        }

        string otpUrl = $"otpauth://totp/{user.Username}?secret={user.MfaSecret}&issuer=Proactive Privacy PoC";
        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpUrl, QRCodeGenerator.ECCLevel.Q);
        Base64QRCode qrCode = new(qrCodeData);
        ViewBag.QRCodeImage = qrCode.GetGraphic(20);

        return View("SetupMfa");
    }

    [HttpPost]
    public IActionResult SetupMfa(string code)
    {
        var username = HttpContext.Session.GetString("Username");
        if (username == null) return RedirectToAction("Index");

        var user = FakeUserStore.Get(username);
        var totp = new Totp(Base32Encoding.ToBytes(user.MfaSecret));
        if (totp.VerifyTotp(code, out _))
        {
            user.IsMfaEnabled = true;
            TempData["Success"] = "MFA set up!";
            return RedirectToAction("Success", "Account");
        }

        ViewBag.Error = "Wrong code. Try again.";
        return RedirectToAction("SetupMfa");
    }
}
