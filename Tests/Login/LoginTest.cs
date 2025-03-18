using Network.Message;
using Network.Protos;
using NLog;
using System.Diagnostics;
using System.Net.WebSockets;
using Utils.EnvUtil;
using Utils.LoggerUtil;
using Utils.TimeUtil;

namespace Tests.Login
{
    public class LoginTest
    {

        private void AddNLog()
        {
            var filePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);
            var name = EnvUtils.GetOrDefault("NLOG_CONFIG_NAME", "NLog.config");
            filePath = $"{filePath}/{name}";
            LogManager.LogFactory.Setup().LoadConfigurationFromFile(filePath);
        }

        [Fact]
        public async Task TestLogin()
        {
            AddNLog();
            var req = new CSMessage(10001, 1, 0, new CSLoginReq()
            {
                RoleId = 10001,
                ServerId = 1001,
                Token = "1234567890abcdef==",
            });
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5064/ws"), CancellationToken.None);
            await client.SendAsync(req.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);
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
                    break;
                }
            }
        }

        [Fact]
        public async Task TestRelogin()
        {
            AddNLog();
            var req = new CSMessage(10001, 0, 0, new CSLoginReq()
            {
                RoleId = 10001,
                ServerId = 1001,
                Token = "1234567890abcdef==",
            });
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5064/ws"), CancellationToken.None);
            await client.SendAsync(req.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);

            using var buffer = new RentBuffer(1024 * 8);
            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(buffer.Buffer, CancellationToken.None);
                if (result.EndOfMessage)
                {
                    var msg = SCMessage.Decode([.. buffer.Buffer[..result.Count]]);
                    Loggers.Test.Info($"Receive message from Server RoleId:{msg.RoleId} IsCache:{msg.IsCache} MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");
                    var ping = new CSMessage(
                        msg.RoleId,
                        0,
                        0,
                        new CSPing()
                        {
                            ClientTimeMs = Time.NowMilliseconds()
                        });
                    await client.SendAsync(ping.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);
                    break;
                }
            }

            var relogin = new CSMessage(10001, 0, 1, new CSReLoginReq()
            {
                RoleId = 10001,
                ServerId = 1001,
                Token = "1234567890abcdef==",
            });
            await client.SendAsync(relogin.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);

            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(buffer.Buffer, CancellationToken.None);
                if (result.EndOfMessage)
                {
                    var msg = SCMessage.Decode([.. buffer.Buffer[..result.Count]]);
                    Loggers.Test.Info($"Receive message from Server RoleId:{msg.RoleId} IsCache:{msg.IsCache} MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");

                    if (msg.MsgName == "SCReLoginResp")
                    {
                        break;
                    }
                }
            }
        }
    }
}