using System.Text.Json.Serialization;

namespace DontStarveTogetherBot.Models;

public class ServerList
{
    [JsonPropertyName("GET")] public List<ServerInfo> ServerInfos { get; set; }
}