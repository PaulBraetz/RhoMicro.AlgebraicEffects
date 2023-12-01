namespace RhoMicro.AlgebraicEffects;
/// <summary>
/// Thrown if an effect is performed but cannot be handled.
/// </summary>
/// <typeparam name="T">The type of effect to handle.</typeparam>
public sealed class UnhandledEffectException<T>() : UnhandledEffectException();
/// <summary>
/// Thrown if an effect is performed but cannot be handled.
/// </summary>
public class UnhandledEffectException() : Exception("Effect was not handled appropriately.");