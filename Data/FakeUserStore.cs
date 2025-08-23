using System.Collections.Generic;
using System.Linq;

public static class FakeUserStore
{
    public static List<User> Users = new List<User>
    {
        new User { Username = "admin", Password = "admin123" }
    };

    public static User? Get(string username) =>
        Users.Find(u => u.Username == username);

    public static bool Exists(string username) =>
        Users.Any(u => u.Username == username);

    public static void Add(User user) =>
        Users.Add(user);
}
