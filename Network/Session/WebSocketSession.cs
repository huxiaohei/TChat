/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:29:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;
using Abstractions.Network;
using Network.Message;
using System.Net.WebSockets;
using System.Threading.Channels;
using Utils.LoggerUtil;
using Utils.SequenceUtil;

namespace Network.Session
{
    public class WebSocketSession : ISession
    {
        public long SessionId { get; } = 0;
        public long RoleId { get; set; } = 0;
        private ISessionManager _sessionManager;
        private WebSocket _webSocket;
        private IMessageHandler _handler;

        public WebSocketSession(ISessionManager sessionManager, IMessageHandler handler, WebSocket webSocket)
        {
            _sessionManager = sessionManager;
            _handler = handler;
            _webSocket = webSocket;
            SessionId = MemoryUniqueSequence.Next();
        }

        public bool IsConnected
        {
            get => _webSocket.State == WebSocketState.Open;
        }

        public async Task SendMessageAsync(byte[] data)
        {
            await _webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
        }

        public async Task SendMessageAsync(ISCMessage message)
        {
            if (IsConnected)
            {
                await SendMessageAsync(message.Encode());
                Loggers.Network.Info($"Send message to RoleId:{message.RoleId} MsgName:{message.MsgName} ClientSerialId:{message.ClientSerialId} ServerSerialId:{message.ServerSerialId} Message:{message.Message}");
            }
        }

        public async Task CloseAsync()
        {
            if (!IsConnected)
                return;
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        }

        public async Task ReceiveMessageAsync(ChannelWriter<ICSMessage> writer)
        {
            using var buffer = new RentBuffer(1024 * 8);
            var cancellationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                WebSocketReceiveResult receiveResult;
                while (IsConnected)
                {
                    do
                    {
                        receiveResult = await _webSocket.ReceiveAsync(buffer.Buffer, cancellationToken.Token);
                    } while (!receiveResult.EndOfMessage);
                    if (receiveResult.CloseStatus.HasValue)
                    {
                        Loggers.Network.Info($"SessionId:{SessionId} receive message close status:{receiveResult.CloseStatus}");
                        break;
                    }
                    if (receiveResult.Count > 0)
                    {
                        var msg = CSMessage.Decode(buffer.Buffer[..receiveResult.Count].ToArray());
                        Loggers.Network.Info($"Receive message from RoleId:{msg.RoleId} MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");
                        writer.TryWrite(msg);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Loggers.Network.Info($"SessionId:{SessionId} receive message timeout");
            }
            catch (Exception e)
            {
                Loggers.Network.Error($"SessionId:{SessionId} receive message error:{e}");
            }
            finally
            {
                await CloseAsync();
                _sessionManager.RemoveSession(SessionId);
            }
        }

        public async Task ProcessMessageAsync(ChannelReader<ICSMessage> reader)
        {
            try
            {
                var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(1));
                while (IsConnected || reader.Count > 0)
                {
                    var msg = await reader.ReadAsync(cancellationToken.Token);
                    if (msg == null)
                    {
                        cancellationToken.TryReset();
                        continue;
                    }
                    try
                    {
                        if (RoleId == 0)
                        {
                            RoleId = msg.RoleId;
                        }
                        var resp = await _handler.HandleMessage(SessionId, msg);
                        if (resp != null)
                        {
                            await SendMessageAsync(resp);
                        }
                    }
                    catch (Exception e)
                    {
                        Loggers.Network.Error($"Process Message:{msg} Error:{e}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Loggers.Network.Info($"SessionId:{SessionId} process message timeout");
            }
            catch (Exception e)
            {
                Loggers.Network.Error($"SessionId:{SessionId} process message error:{e}");
            }
        }
    }
}