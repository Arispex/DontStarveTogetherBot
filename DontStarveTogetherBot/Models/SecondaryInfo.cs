using System.Text.Json.Serialization;

namespace DontStarveTogetherBot.Models;

public class SecondaryInfo
{
    [JsonPropertyName("__addr")]
    public string Addr { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("steamid")]
    public string SteamId { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }
}