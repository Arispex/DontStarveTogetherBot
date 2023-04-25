using DontStarveTogetherBot.Exceptions;
using DontStarveTogetherBot.Models;
using EleCho.GoCqHttpSdk;
using EleCho.GoCqHttpSdk.Message;
using YamlDotNet.Serialization;
using YukariToolBox.LightLog;

Log.LogConfiguration.EnableConsoleOutput().SetLogLevel(LogLevel.Info);
Log.Info("System", "正在加载配置文件...");
var configPath = Path.Combine(AppContext.BaseDirectory, "config.yaml");
if (!File.Exists(configPath))
{
    await File.WriteAllTextAsync(configPath, new Serializer().Serialize(new Config()));
    Log.Error("System", "配置文件不存在，已自动生成，请修改后重启程序。");
    Console.ReadKey();
    return;
}
var config = new Deserializer().Deserialize<Config>(await File.ReadAllTextAsync(configPath));
Log.Info("System", "配置文件加载完成。");

Log.Info("System", "正在连接到 go-cqhttp...");
var session = new CqWsSession(new CqWsSessionOptions()
{
    BaseUri = new Uri(config.CqWsAddress),
    UseApiEndPoint = true,
    UseEventEndPoint = true
});
await session.StartAsync();
Log.Info("System", "连接成功。");

session.UseGroupMessage(async (context, next) =>
{
    // 判断是否为白名单群
    if (!config.QqGroups.Contains(context.GroupId))
    {
        return;
    }
    if (context.RawMessage == "在线")
    {
        var server = new Server(config.DstServerIp, config.DstServerPort);
        List<Player> players;
        try
        {
            players = (await server.GetServerInfoAsync()).GetPlayers();
        }
        catch (ServerNotFoundException e)
        {
            await session.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器。"));
            return;
        }
        if (!players.Any())
        {
            await session.SendGroupMessageAsync(context.GroupId, new CqMessage("当前没有玩家在线。"));
            return;
        }
        await session.SendGroupMessageAsync(context.GroupId, new CqMessage("当前在线玩家：\n" + string.Join(" ", players.Select(x => $"[{x.Name}({x.Prefab})]"))));
        return;
    }

    if (context.RawMessage == "世界信息")
    {
        var server = new Server(config.DstServerIp, config.DstServerPort);
        ServerInfo serverInfo;
        try
        {
            serverInfo = await server.GetServerInfoAsync();
        }
        catch (ServerNotFoundException e)
        {
            await session.SendGroupMessageAsync(context.GroupId, new CqMessage("无法连接至服务器。"));
            return;
        }

        var message = $"房间名称：{serverInfo.Name}\n天数：{serverInfo.GetDay()}\n季节：{serverInfo.GetSeason()}\n这个季节已经过去了 {serverInfo.GetDaysElapsedInSeason()} 天\n距离下个季节还有 {serverInfo.GetDaysLeftInSeason()} 天";
        await session.SendGroupMessageAsync(context.GroupId, new CqMessage(message));
        return;
    }

    if (context.RawMessage == "直连代码")
    {
        await session.SendGroupMessageAsync(context.GroupId, new CqMessage($"c_connect(\"{config.DstServerIp}\", {config.DstServerPort})"));
        return;
    }
});

await session.WaitForShutdownAsync();