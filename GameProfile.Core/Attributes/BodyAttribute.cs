namespace GameProfile.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class BodyAttribute : HttpRequestItemAttribute
{
    public BodyAttribute()
    {
        IgnoreNullValue = false;
    }
}
