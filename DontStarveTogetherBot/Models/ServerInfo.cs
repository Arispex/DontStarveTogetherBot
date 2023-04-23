using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DontStarveTogetherBot.Models;

public class ServerInfo
{
        [JsonPropertyName("__addr")]
    public string Addr { get; set; }

    [JsonPropertyName("__rowId")]
    public string RowId { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; }

    [JsonPropertyName("clanonly")]
    public bool ClanOnly { get; set; }

    [JsonPropertyName("platform")]
    public int Platform { get; set; }

    [JsonPropertyName("mods")]
    public bool Mods { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("pvp")]
    public bool Pvp { get; set; }

    [JsonPropertyName("session")]
    public string Session { get; set; }

    [JsonPropertyName("fo")]
    public bool Fo { get; set; }

    [JsonPropertyName("password")]
    public bool Password { get; set; }

    [JsonPropertyName("guid")]
    public string Guid { get; set; }

    [JsonPropertyName("maxconnections")]
    public int MaxConnections { get; set; }

    [JsonPropertyName("dedicated")]
    public bool Dedicated { get; set; }

    [JsonPropertyName("clienthosted")]
    public bool ClientHosted { get; set; }

    [JsonPropertyName("connected")]
    public int Connected { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; }

    [JsonPropertyName("port")]
    public int Port { get; set; }

    [JsonPropertyName("v")]
    public int V { get; set; }

    [JsonPropertyName("tags")]
    public string Tags { get; set; }

    [JsonPropertyName("season")]
    public string Season { get; set; }

    [JsonPropertyName("lanonly")]
    public bool LanOnly { get; set; }

    [JsonPropertyName("intent")]
    public string Intent { get; set; }

    [JsonPropertyName("allownewplayers")]
    public bool AllowNewPlayers { get; set; }

    [JsonPropertyName("serverpaused")]
    public bool ServerPaused { get; set; }

    [JsonPropertyName("steamid")]
    public string SteamId { get; set; }

    [JsonPropertyName("steamroom")]
    public string SteamRoom { get; set; }

    [JsonPropertyName("secondaries")]
    public Dictionary<string, SecondaryInfo> Secondaries { get; set; }

    [JsonPropertyName("data")]
    public string Data { get; set; }

    [JsonPropertyName("worldgen")]
    public string WorldGen { get; set; }

    [JsonPropertyName("players")]
    public string Players { get; set; }

    [JsonPropertyName("mods_info")]
    public List<object> ModsInfo { get; set; }

    [JsonPropertyName("tick")]
    public int Tick { get; set; }

    [JsonPropertyName("clientmodsoff")]
    public bool ClientModsOff { get; set; }

    [JsonPropertyName("nat")]
    public int Nat { get; set; }

    public List<Player> GetPlayers()
    {
        var players = new List<Player>();
        var pattern = @"colour=""(?<colour>[A-Z0-9]+)"".*?eventlevel=(?<eventlevel>\d+).*?name=""(?<name>.*?)"".*?netid=""(?<netid>\d+)"".*?prefab=""(?<prefab>\w+)""";
        var matches = Regex.Matches(Players, pattern, RegexOptions.Singleline);
        foreach (Match match in matches)
        {
            players.Add(new Player()
            {
                Colour = match.Groups["colour"].Value,
                EventLevel = int.Parse(match.Groups["eventlevel"].Value),
                Name = match.Groups["name"].Value,
                NetId = match.Groups["netid"].Value,
                Prefab = match.Groups["prefab"].Value
            });
        }
        return players;
    }

    public string GetSeason()
    {
        return Season;
    }

    public int GetDay()
    {
        return int.Parse(Regex.Match(Data, @"day=(\d+)").Groups[1].Value);
    }

    public int GetDaysElapsedInSeason()
    {
        return int.Parse(Regex.Match(Data, @"dayselapsedinseason=(\d+)").Groups[1].Value);
    }

    public int GetDaysLeftInSeason()
    {
        return int.Parse(Regex.Match(Data, @"daysleftinseason=(\d+)").Groups[1].Value);
    }
}