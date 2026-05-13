namespace GameProfile.Core.Attributes;

public abstract class HttpRequestItemAttribute : Attribute
{
    public string Name { get; set; }
    public bool IgnoreNullValue { get; protected init; } = true;
}
