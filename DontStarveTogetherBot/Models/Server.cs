using System.IO.Compression;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using DontStarveTogetherBot.Exceptions;
using Microsoft.VisualBasic.CompilerServices;
using YamlDotNet.Serialization;

namespace DontStarveTogetherBot.Models;

public class Server
{
    public string Ip { get; set; }
    public int Port { get; set; }
    
    public Server(string ip, int port)
    {
        this.Ip = ip;
        this.Port = port;
    }

    public async ValueTask<string> GetRowIdAsync()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.yaml");
        var config = new Deserializer().Deserialize<Config>(await File.ReadAllTextAsync(configPath));

        var url = $"https://lobby-v2-cdn.klei.com/{config.Region}-Steam.json.gz";
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        var stream = new GZipStream(await response.Content.ReadAsStreamAsync(), CompressionMode.Decompress);
        var serverList = await JsonSerializer.DeserializeAsync<ServerList>(stream, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var server = serverList.ServerInfos.FirstOrDefault(x => x.Addr == this.Ip && x.Port == this.Port);
        if (server == null)
        {
            throw new ServerNotFoundException("Server not found.");
        }

        return server.RowId;
    }

    public async ValueTask<ServerInfo> GetServerInfoAsync()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "config.yaml");
        var config = new Deserializer().Deserialize<Config>(await File.ReadAllTextAsync(configPath));
        
        var rowId = await this.GetRowIdAsync();
        var url = "https://lobby-v2-ap-east-1.klei.com/lobby/read";
        var query = new Dictionary<string, object>
        {
            ["__gameId"] = "DontStarveTogether",
            ["__token"] = config.KLeiToken,
            ["query"] = new Dictionary<string, string>()
            {
                {"__rowid", rowId}
            }
        };
        var jsonString = JsonSerializer.Serialize(query);
        var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
        var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, content);
        // var result = Regex.Replace(await response.Content.ReadAsStringAsync(), @"\s+", "");
        var serverList = await JsonSerializer.DeserializeAsync<ServerList>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        return serverList.ServerInfos[0];
    }
}