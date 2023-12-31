using Rascal.Errors;

namespace Rascal.Tests;

public class ErrorTests
{
    [Fact]
    public void ImplicitConversion_FromString_ReturnsStringError()
    {
        Error error = "error";
        
        error.ShouldBeOfType<StringError>();
        error.Message.ShouldBe("error");
    }

    [Fact]
    public void ImplicitConversion_FromException_ReturnsExceptionError()
    {
        Error error = new TestException();

        var exceptionError = error.ShouldBeOfType<ExceptionError>();
        exceptionError.Exception.ShouldBeOfType<TestException>();
    }
}
