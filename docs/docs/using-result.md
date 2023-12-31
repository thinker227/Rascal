# Using `Result<T>`

## Introduction

The most fundamental type Rascal provides is the [`Result<T>`](/api/Rascal.Result-1.html) struct. `Result<T>` can be summarized in a myriad of ways, but in the most simplest of terms, **`Result<T>` represents either some value *or* an error**. This kind of value-or-error pattern (commonly and quite aptly referred to as the "result pattern") is applicable in a wide variety of situations, practically any time there is some kind of operation which might fail. In C# we typically use either the `Try*` pattern or exceptions to represent this, but the `Try*` pattern is somewhat verbose and ugly, and exceptions have performance penalties and are hard to catch and handle properly (and most importantly are usually not documented very well). The result pattern and the `Result<T>` type aim to solve these issues by *1)* documenting the possibility of failure on the *type* level, and *2)* forcing you to handle the possibility of an error occuring. Neither the `Try*` pattern nor exceptions do either of these things very well.

### An example

So, how do you use `Result<T>`? In the simplest case, imagine a method which takes the ID of a user and returns the user with that ID:

```cs
public async Task<User?> GetUser(int userId)
{
    var user = await db.Users.FirstOrDefaultAsync(user => user.Id == userId);

    return user;
}
```

There are two things to note about this method: *1)* we're returning `User?` through the task, which doesn't communicate intent particularly well, and *2)* `FirstOrDefaultAsync` might throw an exception. Let's rewrite this to use `Result<T>` with proper handling for both of these situations:

```cs
public Task<Result<User>> GetUser(int userId) => TryAsync(async () =>
{
    var user = await db.Users.FirstOrDefaultAsync(user => user.Id == userId);

    if (user is null) return Err<User>(new NotFoundError($"Could not find a user with id {userId}."));

    return user;
});
```

Now, suddenly this code can handle `FirstOrDefaultAsync` throwing an exception, and the case where no user with the ID `userId` can be found.

Of course, this is a somewhat contrived example of when using `Result<T>` would be useful, but it gets the point across.

### WHAT DO I DO WITH THIS???

So you have a `Result<T>` instance now, great! Now what can you do with this?? As a good programmer, you look through the intellisense context menu on what this type has to offer, but fail to see anything resembling a `Value` property. This is because `Result<T>` does not contain a `Value` property. Instead, if you're given a `Result<T>`, you have two options on what to do with it: continue working with it, molding it like a lump of clay until the value inside it resembles what you want it to, or try *unwrapping* the value to see if it exists, then do whatever you want with that value. The former of these two options is usually the better and less cumbersome one, by not requiring you to constantly have to check whether the result actually contains anything useful, rather letting you happily pretend it does until reality comes crashing down and you realize it doesn't (or it actually does!).

Following the two above paths to functional Nirvana, you can group the methods `Result<T>` provides in two: [mapping](#mapping) and [unwrapping](#unwrapping).

## Mapping

*Mapping* in the context of `Result<T>` is akin to reaching inside a shut box and doing something with the contents of that box. If the box contains something, when you finally open the box you'll see the fruits of your labor, otherwise if the box was empty from the start or you somehow managed to destory the thing inside while you were working with it, you'll just have an empty box in the end. This is in essence what mapping is.

The first method you might find in `Result<T>` on this topic is the aptly named [`Map`](/api/Rascal.Result-1.html#Rascal_Result_1_Map__1_System_Func__0___0__) method, which full signature is

```cs
public Result<TNew> Map<TNew>(Func<T, TNew> mapping)
```

Remember `T` here is some kind of value. We don't actually know if that value exists, but `Map` will let us pretend it does. The one parameter the method takes is a function which takes a `T` and returns a `TNew`, which can be absolutely whatever we want. If we had a result returned by `ParseR<int>` (a `Result<int>`), we could perhaps try turn it into a string:

```cs
Result<int> result = ParseR<int>(Console.ReadLine()!);

Result<string> newResult = result.Map(int value => value.ToString());
```

This isn't particularly useful in a real-world case, but it demonstrates what `Map` does. Now if we imagine `result` was actually an error, `Map` wouldn't have a value to give us, so instead the error is just passed along to the next step.

## Unwrapping
