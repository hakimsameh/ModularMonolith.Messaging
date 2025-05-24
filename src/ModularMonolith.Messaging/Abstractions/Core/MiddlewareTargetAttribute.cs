namespace ModularMonolith.Messaging.Abstractions.Core;

/// <summary>
/// Attribute that specifies which message types a middleware targets.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class MiddlewareTargetAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MiddlewareTargetAttribute"/> class.
    /// </summary>
    /// <param name="targetMessages">The message types the middleware targets.</param>
    public MiddlewareTargetAttribute(params Type[] targetMessages)
    {
        TargetMessages = targetMessages;
    }

    /// <summary>
    /// Gets the message types the middleware targets.
    /// </summary>
    public Type[] TargetMessages { get; }
}
