namespace DontStarveTogetherBot.Models;

public class Config
{
    public string CqWsAddress = "ws://127.0.0.1:8080";
    public string DstServerIp = "127.0.0.1";
    public int DstServerPort = 10999;
    public string KLeiToken = "exampleToken";
    public List<long> QqGroups = new();
    public string Region = "ap-east-1";
}