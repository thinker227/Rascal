using System.Text.Json;

namespace Rascal.Json.Tests;

public class ResultConverterTests
{
    [Fact]
    public void SerializesOk()
    {
        var result = Ok(2);
        var options = new JsonSerializerOptions().AddResultConverters();
        
        var json = JsonSerializer.Serialize(result, options);

        json.ShouldBe("""
        {"ok":2}
        """);
    }

    [Fact]
    public void SerializesErr()
    {
        var result = Err<int>("error");
        var options = new JsonSerializerOptions().AddResultConverters();
        
        var json = JsonSerializer.Serialize(result, options);

        json.ShouldBe("""
        {"err":"error"}
        """);
    }

    [Fact]
    public void DeserializesOk()
    {
        var json = """
        {"ok":2}
        """;
        var options = new JsonSerializerOptions().AddResultConverters();

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeTrue();
        result.value.ShouldBe(2);
    }

    [Fact]
    public void DeserializesErr()
    {
        var json = """
        {"err":"error"}
        """;
        var options = new JsonSerializerOptions().AddResultConverters();

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeFalse();
        result.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void DeserializesOkPreferred()
    {
        var json = """
        {"err":"error","ok":2}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o with
        {
            PropertyPreference = ResultPropertyPreference.PreferOk
        });

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeTrue();
        result.value.ShouldBe(2);
    }
    
    [Fact]
    public void DeserializesErrPreferred()
    {
        var json = """
        {"ok":2,"err":"error"}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o with
        {
            PropertyPreference = ResultPropertyPreference.PreferErr
        });

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeFalse();
        result.error?.Message.ShouldBe("error");
    }
    
    [Fact]
    public void DeserializesOkFirst()
    {
        var json = """
        {"ok":2,"err":"error"}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o with
        {
            PropertyPreference = ResultPropertyPreference.First
        });

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeTrue();
        result.value.ShouldBe(2);
    }
    
    [Fact]
    public void DeserializesErrFirst()
    {
        var json = """
        {"err":"error","ok":2}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o with
        {
            PropertyPreference = ResultPropertyPreference.First
        });

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);
        
        result.IsOk.ShouldBeFalse();
        result.error?.Message.ShouldBe("error");
    }

    [Fact]
    public void SerializesUsingOkPropertyName()
    {
        var result = Ok(2);
        var options = new JsonSerializerOptions().AddResultConverters(o => o
            .WithOkPropertyName("foo"));

        var json = JsonSerializer.Serialize(result, options);

        json.ShouldBe("""
        {"foo":2}
        """);
    }

    [Fact]
    public void SerializesUsingErrorPropertyName()
    {
        var result = Err<int>("error");
        var options = new JsonSerializerOptions().AddResultConverters(o => o
            .WithErrorPropertyName("foo"));

        var json = JsonSerializer.Serialize(result, options);

        json.ShouldBe("""
        {"foo":"error"}
        """);
    }

    [Fact]
    public void DeserializesUsingOkPropertyName()
    {
        var json = """
        {"foo":2}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o
            .WithOkPropertyName("foo"));

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);

        result.IsOk.ShouldBeTrue();
        result.value.ShouldBe(2);
    }

    [Fact]
    public void DeserializesUsingErrorPropertyName()
    {
        var json = """
        {"foo":"error"}
        """;
        var options = new JsonSerializerOptions().AddResultConverters(o => o
            .WithErrorPropertyName("foo"));

        var result = JsonSerializer.Deserialize<Result<int>>(json, options);

        result.IsOk.ShouldBeFalse();
        result.error?.Message.ShouldBe("error");
    }
}
