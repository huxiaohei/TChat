/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 3/17/2025, 7:57:38 PM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;
using ChatServer.Extensions;
using ChatServer.Grains;
using Google.Protobuf;
using Network.Protos;
using Utils.LoggerUtil;
using Utils.TimeUtil;

namespace ChatServer.Modules.Login
{
    public static class LoginHandler
    {
        [MessageCallback(typeof(CSLoginReq))]
        public static async Task<IMessage?> HandleLoginReq(this PlayerGrain playerGain, ICSMessage message)
        {
            var _ = message.Cast<CSLoginReq>();
            return await playerGain.GetModule<LoginModule>()!.LoginAsync();
        }

        [MessageCallback(typeof(CSReLoginReq))]
        public static async Task<IMessage?> HandleReLoginReq(this PlayerGrain playerGain, ICSMessage message)
        {
            var _ = message.Cast<CSReLoginReq>();
            return await playerGain.GetModule<LoginModule>()!.ReLoginAsync(message.ServerSerialId);
        }

        [MessageCallback(typeof(CSPing))]
        public static async Task<IMessage?> HandlePing(this PlayerGrain playerGain, ICSMessage message)
        {
            var msg = message.Cast<CSPing>();
            Loggers.Chat.Info($"PlayerGrain roleId:{playerGain.RoleId} ping {msg.ClientTimeMs}");
            await playerGain.SendMessageAsync(new SCPong()
            {
                ServerTimeMs = Time.NowMilliseconds()
            });
            return default;
        }
    }
}