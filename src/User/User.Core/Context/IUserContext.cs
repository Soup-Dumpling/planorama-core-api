namespace Planorama.User.Core.Context
{
    public interface IUserContext
    {
        string UserName { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
        bool IsLoggedIn();
        bool IsInRole(string roleName);
    }
}
