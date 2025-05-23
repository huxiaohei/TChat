/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 3/17/2025, 7:57:38 PM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Utils.TimeUtil;
using Network.Protos;
using Google.Protobuf;
using Utils.LoggerUtil;
using Network.Extensions;
using Abstractions.Grains;
using Abstractions.Message;

namespace Modules.Player.Login
{
    public static class LoginHandler
    {
        [MessageCallback(typeof(CSLoginReq))]
        public static async Task<IMessage?> HandleLoginReq(this IPlayer player, ICSMessage message)
        {
            var _ = message.Cast<CSLoginReq>();
            return await player.GetModule<LoginModule>()!.LoginAsync();
        }

        [MessageCallback(typeof(CSReLoginReq))]
        public static async Task<IMessage?> HandleReLoginReq(this IPlayer player, ICSMessage message)
        {
            var _ = message.Cast<CSReLoginReq>();
            return await player.GetModule<LoginModule>()!.ReLoginAsync(message.ServerSerialId);
        }

        [MessageCallback(typeof(CSPing))]
        public static async Task<IMessage?> HandlePing(this IPlayer player, ICSMessage message)
        {
            var msg = message.Cast<CSPing>();
            Loggers.Chat.Info($"Player roleId:{player.RoleId} ping {msg.ClientTimeMs}");
            await player.SendMessageAsync(new SCPong()
            {
                ServerTimeMs = Time.NowMilliseconds()
            });
            return default;
        }
    }
}