using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace gop.Extensions;

/// <summary>
/// JSON Extensions
/// </summary>
public static class JsonExtensions
{
    private static readonly CamelCaseNamingStrategy NamingStrategy = new();
    private static readonly StringEnumConverter EnumConverter = new(NamingStrategy);
    private static readonly PrivateSetterContractResolver ContractResolver = new() { NamingStrategy = NamingStrategy };
    private static readonly Lazy<JsonSerializerSettings> LazySettings = new(() => new JsonSerializerSettings().Configure(), isThreadSafe: true);

    /// <summary>
    /// Deserializes JSON to the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to deserialize to.</typeparam>
    /// <param name="value">The object to deserialize.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static T FromJson<T>(this string value) =>
        value != null ? JsonConvert.DeserializeObject<T>(value, LazySettings.Value) : default;

    /// <summary>
    /// Serializes the specified object into a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string ToJson<T>(this T value) =>
        value != null ? JsonConvert.SerializeObject(value, LazySettings.Value) : default;

    public static JsonSerializerSettings Configure(this JsonSerializerSettings settings)
    {
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
        settings.NullValueHandling = NullValueHandling.Ignore;
        settings.Formatting = Formatting.None;
        settings.ContractResolver = ContractResolver;
        settings.Converters.Add(EnumConverter);
        return settings;
    }
}