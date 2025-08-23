public class User
{
    public string Username { get; set; }
    public string Password { get; set; } // Voor PoC plaintext (NIET in productie!)
    public string MfaSecret { get; set; }
    public bool IsMfaEnabled { get; set; } = false;

}
