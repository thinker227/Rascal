namespace Rascal;

public readonly partial struct Result<T>
{
    /// <summary>
    /// Maps the value of the result using a mapping function,
    /// or does nothing if the result is an error.  
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A new result containing either the mapped value
    /// or the error of the original result.</returns>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapping) =>
        HasValue
            ? new(mapping(value))
            : new(Error);

    /// <summary>
    /// Maps the value of the result to a new result using a mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A result which is either the mapped result
    /// or a new result containing the error of the original result.</returns>
    public Result<TNew> Then<TNew>(Func<T, Result<TNew>> mapping) =>
        HasValue
            ? mapping(value)
            : new(Error);

    /// <inheritdoc cref="Map{TNew}"/> 
    public Result<TNew> Select<TNew>(Func<T, TNew> mapping) =>
        Map(mapping);

    /// <inheritdoc cref="Then{TNew}"/>
    public Result<TNew> SelectMany<TNew>(Func<T, Result<TNew>> mapping) =>
        Then(mapping);

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
    /// <returns>A result constructed by applying <paramref name="mapping"/>
    /// onto the value contained in the result returned by <paramref name="other"/>
    /// as well as the value in the original result,
    /// or a new result containing the error of the original or intermediate results.</returns>
    public Result<TNew> SelectMany<TOther, TNew>(
        Func<T, Result<TOther>> other,
        Func<T, TOther, TNew> mapping)
    {
        if (!HasValue) return new(Error);
        var a = value;

        var br = other(a);
        if (!br.HasValue) return new(br.Error);
        var b = br.value;

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
    /// <returns>A new result containing either the mapped value,
    /// the exception thrown by <paramref name="mapping"/> wrapped in an <see cref="ExceptionError"/>,
    /// or the error of the original result.</returns>
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
    /// <returns>A result which is either the mapped result,
    /// the exception thrown by <paramref name="mapping"/> wrapped in an <see cref="ExceptionError"/>,
    /// or a new result containing the error of the original result.</returns>
    public Result<TNew> ThenTry<TNew>(Func<T, Result<TNew>> mapping)
    {
        try
        {
            return Then(mapping);
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }

    /// <summary>
    /// Maps the error in the result if it contains one
    /// and returns a new result containing the mapped error.
    /// Otherwise, returns the current result.
    /// </summary>
    /// <param name="mapping">The function used to map the error.</param>
    /// <returns>A result which contains either the mapped error
    /// or the value of the original result.</returns>
    public Result<T> MapError(Func<Error, Error> mapping) =>
        !HasValue
            ? new(mapping(Error))
            : this;
}
