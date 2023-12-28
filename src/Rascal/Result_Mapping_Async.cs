namespace Rascal;

public readonly partial struct Result<T>
{
    // ValueTask is not available in .NET Standard 2.0.
#if NETCOREAPP

    /// <summary>
    /// Maps the value of the result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A <see cref="ValueTask{T}"/> which either completes asynchronously
    /// by invoking the mapping function on the value of the result and constructing a new result
    /// containing the mapped value, or completes synchronously by returning a new result
    /// containing the error of the original result.</returns>
    [Pure]
    public ValueTask<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapping)
    {
        if (!IsOk) return new(new Result<TNew>(Error));

        var task = mapping(value!);
        return new(CreateResult(task));

        static async Task<Result<TNew>> CreateResult(Task<TNew> task)
        {
            var value = await task;
            return new(value);
        }
    }
    
    /// <summary>
    /// Maps the value of the result to a new result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A <see cref="ValueTask{T}"/> which either completes asynchronously
    /// by invoking the mapping function on the value of the result, 
    /// or completes synchronously by returning a new result
    /// containing the error of the original result.</returns>
    [Pure]
    public ValueTask<Result<TNew>> ThenAsync<TNew>(Func<T, Task<Result<TNew>>> mapping)
    {
        if (!IsOk) return new(new Result<TNew>(Error));

        var task = mapping(value!);
        return new(task);
    }
    
    /// <summary>
    /// Maps the value of the result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A <see cref="ValueTask{T}"/> which either completes asynchronously
    /// by invoking the mapping function on the value of the result and constructing a new result
    /// containing the mapped value,
    /// returning any exception thrown by <paramref name="mapping"/>
    /// wrapped in an <see cref="ExceptionError"/>,
    /// or completes synchronously by returning a new result containing the error of the original result.</returns>
    [Pure]
    public ValueTask<Result<TNew>> TryMapAsync<TNew>(Func<T, Task<TNew>> mapping)
    {
        if (!IsOk) return new(new Result<TNew>(Error));

        try
        {
            var task = mapping(value!);
            return new(CreateResult(task));
        }
        catch (Exception e)
        {
            return new(new Result<TNew>(new ExceptionError(e)));
        }

        static async Task<Result<TNew>> CreateResult(Task<TNew> task)
        {
            try
            {
                var value = await task;
                return new(value);
            }
            catch (Exception e)
            {
                return new(new ExceptionError(e));
            }
        }
    }
    
    /// <summary>
    /// Maps the value of the result to a new result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A <see cref="ValueTask{T}"/> which either completes asynchronously
    /// by invoking the mapping function on the value of the result,
    /// returning any exception thrown by <paramref name="mapping"/>
    /// wrapped in an <see cref="ExceptionError"/>,
    /// or completes synchronously by returning a new result
    /// containing the error of the original result.</returns>
    [Pure]
    public ValueTask<Result<TNew>> ThenTryAsync<TNew>(Func<T, Task<Result<TNew>>> mapping)
    {
        if (!IsOk) return new(new Result<TNew>(Error));

        try
        {
            var task = mapping(value!);
            return new(task);
        }
        catch (Exception e)
        {
            return new(new Result<TNew>(new ExceptionError(e)));
        }
    }

#else
    
    /// <summary>
    /// Maps the value of the result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A new result containing either the mapped value
    /// or the error of the original result.</returns>
    [Pure]
    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapping) =>
        IsOk
            ? new(await mapping(value!))
            : new(Error);

    /// <summary>
    /// Maps the value of the result to a new result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A result which is either the mapped result
    /// or a new result containing the error of the original result.</returns>
    [Pure]
    public async Task<Result<TNew>> ThenAsync<TNew>(Func<T, Task<Result<TNew>>> mapping) =>
        IsOk
            ? await mapping(value!)
            : new(Error);

    /// <summary>
    /// Tries to map the value of the result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A new result containing either the mapped value,
    /// the exception thrown by <paramref name="mapping"/> wrapped in an <see cref="ExceptionError"/>,
    /// or the error of the original result.</returns>
    [Pure]
    public async Task<Result<TNew>> TryMapAsync<TNew>(Func<T, Task<TNew>> mapping)
    {
        if (!IsOk) return new(Error);

        try
        {
            var val = await mapping(value!);
            return new(val);
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }

    /// <summary>
    /// Tries to map the value of the result to a new result using an asynchronous mapping function,
    /// or does nothing if the result is an error.
    /// If the mapping function throws an exception, the exception will be returned wrapped in an
    /// <see cref="ExceptionError"/>.
    /// </summary>
    /// <param name="mapping">The function used to map the value to a new result.</param>
    /// <typeparam name="TNew">The type of the new value.</typeparam>
    /// <returns>A result which is either the mapped result,
    /// the exception thrown by <paramref name="mapping"/> wrapped in an <see cref="ExceptionError"/>,
    /// or a new result containing the error of the original result.</returns>
    [Pure]
    public async Task<Result<TNew>> ThenTryAsync<TNew>(Func<T, Task<Result<TNew>>> mapping)
    {
        if (!IsOk) return new Result<TNew>(Error);

        try
        {
            return await mapping(value!);
        }
        catch (Exception e)
        {
            return new(new ExceptionError(e));
        }
    }

#endif
}
