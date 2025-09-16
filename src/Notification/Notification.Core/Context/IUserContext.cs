namespace Planorama.Notification.Core.Context
{
    public interface IUserContext
    {
        string UserName { get; }
        bool IsLoggedIn();
        bool IsInRole(string roleName);
    }
}
