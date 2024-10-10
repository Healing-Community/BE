namespace PRH_UserService_API.Middleware
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousRefreshTokenAttribute : Attribute { }
}
