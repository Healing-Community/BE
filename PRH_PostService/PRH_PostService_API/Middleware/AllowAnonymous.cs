namespace PRH_PostService_API.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymous : Attribute
{
}