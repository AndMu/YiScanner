using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wikiled.YiScanner.Destinations
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionType
    {
        Execute,
        Rest
    }
}
