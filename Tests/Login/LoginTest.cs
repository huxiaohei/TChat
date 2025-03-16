using Network.Message;
using Network.Protos;
using System.Net.WebSockets;
using Utils.LoggerUtil;
using Utils.TimeUtil;

namespace Tests.Login
{
    public class LoginTest
    {
        [Fact]
        public async Task TestLogin()
        {
            var clients = new List<ClientWebSocket>();
            for (int i = 0; i < 10; i++)
            {
                var req = new CSMessage(10001 + i, 0, 0, new CSLoginReq()
                {
                    Uid = 10001 + i,
                    ServerId = 1001,
                    Token = "1234567890abcdef==",
                });
                var client = new ClientWebSocket();
                await client.ConnectAsync(new Uri("ws://localhost:5064/ws"), CancellationToken.None);
                await client.SendAsync(req.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);
                clients.Add(client);
            }

            await Task.WhenAll(clients.Select(async client =>
            {
                using var buffer = new RentBuffer(1024 * 8);
                while (client.State == WebSocketState.Open)
                {
                    var result = await client.ReceiveAsync(buffer.Buffer, CancellationToken.None);
                    if (result.EndOfMessage)
                    {
                        var msg = SCMessage.Decode([.. buffer.Buffer[..result.Count]]);
                        Loggers.Test.Info($"Receive message from Server, RoleId:{msg.RoleId} MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");

                        var ping = new CSMessage(
                            msg.RoleId,
                            msg.ClientSerialId + 1,
                            msg.ServerSerialId,
                            new CSPing()
                            {
                                ClientTimeMs = Time.NowMilliseconds()
                            });
                        await client.SendAsync(ping.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);
                    }

                }
            }));
        }
    }
}