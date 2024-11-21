namespace PRH_ChatService_API.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymous : Attribute
{
}