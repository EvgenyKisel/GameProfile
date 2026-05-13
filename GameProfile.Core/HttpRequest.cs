using System.Reflection;
using System.Text;
using GameProfile.Core.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GameProfile.Core;

public abstract class HttpRequest
{
    public override string ToString()
    {
        var builder = new StringBuilder();
        Append(builder, "Parameters", GetUrlParameters());
        Append(builder, "Headers", GetHeaders());

        var body = GetBody();
        if (!string.IsNullOrEmpty(body))
        {
            if (builder.Length > 0) builder.AppendLine();
            builder.AppendLine("Body");
            builder.AppendLine(body);
        }
        return builder.ToString();
    }

    internal Dictionary<string, string> GetUrlParameters() => Collect<UrlParameterAttribute>();

    internal Dictionary<string, string> GetHeaders() => Collect<HeaderAttribute>();

    internal string GetBody()
    {
        var bodyProps = Collect<BodyAttribute>();
        if (bodyProps.Count == 0) return null;

        return JsonConvert.SerializeObject(this, Formatting.None, new JsonSerializerSettings
        {
            ContractResolver = new BodyOnlyContractResolver(GetType(), bodyProps.Keys),
            NullValueHandling = NullValueHandling.Ignore
        });
    }

    private Dictionary<string, string> Collect<TAttr>() where TAttr : HttpRequestItemAttribute
    {
        var result = new Dictionary<string, string>();
        foreach (var prop in GetType().GetProperties())
        {
            var attr = prop.GetCustomAttribute<TAttr>();
            if (attr is null) continue;

            var value = prop.GetValue(this);
            if (attr.IgnoreNullValue && value is null) continue;

            result[attr.Name ?? prop.Name] = value?.ToString();
        }
        return result;
    }

    private static void Append(StringBuilder builder, string title, Dictionary<string, string> values)
    {
        if (values is null || values.Count == 0) return;
        if (builder.Length > 0) builder.AppendLine();
        builder.AppendLine(title);
        builder.AppendLine(JsonConvert.SerializeObject(values));
    }

    private sealed class BodyOnlyContractResolver(Type owner, IEnumerable<string> bodyPropNames)
        : DefaultContractResolver
    {
        private readonly HashSet<string> _bodyPropNames = new(bodyPropNames);

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var all = base.CreateProperties(type, memberSerialization);
            if (type != owner) return all;

            var filtered = new List<JsonProperty>();
            foreach (var prop in all)
            {
                if (_bodyPropNames.Contains(prop.PropertyName))
                {
                    filtered.Add(prop);
                    continue;
                }

                var assignedName = type.GetProperty(prop.PropertyName)
                    ?.GetCustomAttribute<HttpRequestItemAttribute>()?.Name ?? prop.PropertyName;
                if (_bodyPropNames.Contains(assignedName))
                {
                    prop.PropertyName = assignedName;
                    filtered.Add(prop);
                }
            }
            return filtered;
        }
    }
}
