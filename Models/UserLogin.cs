public class UserLogin
{
    public string UserName { get; set; }
    
    public string Email { get; set; }

    public string Password { get; set; }

    public UserLogin(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }

}
