namespace ModularMonolith.Messaging.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MiddlewareTargetAttribute(params Type[] targetMessages) : Attribute
{
    public Type[] TargetMessages { get; } = targetMessages;
}
