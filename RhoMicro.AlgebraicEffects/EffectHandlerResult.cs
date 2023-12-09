namespace RhoMicro.AlgebraicEffects;
using RhoMicro.CodeAnalysis;

/// <summary>
/// Represents the result of handling an effect.
/// </summary>
/// <typeparam name="T">The type of result to be produced by a handler.</typeparam>
[UnionType(typeof(Unit))]
[UnionType(nameof(T), Alias = "Result")]
[UnionTypeSettings(ToStringSetting = ToStringSetting.Simple)]
public readonly partial struct EffectHandlerResult<T>
{
    /// <summary>
    /// The unit result.
    /// </summary>
    public static readonly EffectHandlerResult<T> Unit = CreateFromUnit(new());

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static Boolean operator ==(EffectHandlerResult<T> left, EffectHandlerResult<T> right) =>
        left.Equals(right);
    public static Boolean operator !=(EffectHandlerResult<T> left, EffectHandlerResult<T> right) =>
        !(left == right);
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
