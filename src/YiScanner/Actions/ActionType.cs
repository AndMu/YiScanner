using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Wikiled.YiScanner.Actions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ActionType
    {
        Execute,
        Rest
    }
}
