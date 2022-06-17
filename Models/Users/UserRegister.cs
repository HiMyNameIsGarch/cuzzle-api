public class UserRegister
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public UserRegister(string username, string email, string password)
    {
        UserName = username;
        Email = email;
        Password = password;
    }

}
