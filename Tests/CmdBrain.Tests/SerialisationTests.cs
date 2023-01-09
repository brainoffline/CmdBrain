using Newtonsoft.Json;
using No8.CmdBrain.Tests.Helpers;

namespace No8.CmdBrain.Tests;

[TestClass]
public class SerialisationTests
{
    [Fact(Skip = "System.Text.Json does not support derived types")]
    public void Test1()
    {
        var c = new Container
        {
            List = new List<Base> { new Base1 { Str = "str1", Integer = 1 } },
            Array = new Base[] { new Base1 { Str = "str2", Integer = 2 } }
        };
        var json = System.Text.Json.JsonSerializer.Serialize<object>(c);

        Assert.Equal(
            "{\"List\":[{\"Integer\":1,\"Str\":\"str1\"}],\"Array\":[{\"Integer\":2,\"Str\":\"str2\"}]}",
            json);
    }

    [Fact]
    public void Test2()
    {
        var c = new Container
        {
            List = new List<Base> { new Base1 { Str = "str1", Integer = 1 } },
            Array = new Base[] { new Base1 { Str = "str2", Integer = 2 } }
        };
        var json = JsonConvert.SerializeObject(c);

        Assert.Equal(
                        "{\"List\":[{\"Integer\":1,\"Str\":\"str1\"}],\"Array\":[{\"Integer\":2,\"Str\":\"str2\"}]}",
                        json);
    }

}

public class Base
{
    public string? Str { get; set; }
}

public class Base1 : Base
{
    public int Integer { get; set; }
}

public class Container
{
    public List<Base>? List { get; set; }
    public Base[]? Array { get; set; }
}
