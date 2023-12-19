namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Maps the value of the result using a mapping function,
    /// or does nothing if the result is an error.  
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapping) =>
        hasValue
            ? new(mapping(value!))
            : new(Error);

    /// <summary>
    /// Maps the value of the result to a new result using a mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns></returns>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> mapping) =>
        hasValue
            ? mapping(value!)
            : new(Error);

    /// <inheritdoc cref="Map{TNew}"/> 
    public Result<TNew> Select<TNew>(Func<T, TNew> mapping) =>
        Map(mapping);

    /// <inheritdoc cref="Bind{TNew}"/>
    public Result<TNew> SelectMany<TNew>(Func<T, Result<TNew>> mapping) =>
        Bind(mapping);

    /// <summary>
    /// Maps the value of the result to an intermediate result using a mapping function,
    /// then applies another mapping function onto the values of the original and intermediate results,
    /// or does nothing if either of the results is an error.
    /// </summary>
    /// <remarks>
    /// The expression <c>r.SelectMany(x => f(x), (x, y) => g(x, y))</c> can be written as
    /// <c>r.Bind(x => f(x).Map(y => g(x, y)))</c>.
    /// </remarks>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TOther">The type of the intermediate value.</typeparam>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>The result of</returns>
    public Result<TNew> SelectMany<TOther, TNew>(
        Func<T, Result<TOther>> other,
        Func<T, TOther, TNew> mapping)
    {
        if (!hasValue) return new(Error);
        var a = value!;

        var br = other(a);
        if (!br.hasValue) return new(br.Error);
        var b = br.value!;

        return new(mapping(a, b));
    }

    /// <summary>
    /// Tries to map the value of the result using a mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    public Result<TNew> TryMap<TNew>(Func<T, TNew> mapping)
    {
        try
        {
            return Map(mapping);
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }

    /// <summary>
    /// Tries to map the value of the result to a new result using a mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns></returns>
    public Result<TNew> TryBind<TNew>(Func<T, Result<TNew>> mapping)
    {
        try
        {
            return Bind(mapping);
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }
}
