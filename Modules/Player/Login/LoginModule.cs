/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 3/18/2025, 11:06:49 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Network.Protos;
using Google.Protobuf;
using Abstractions.Module;
using Abstractions.Grains;
using Utils.LoggerUtil;

namespace Modules.Player.Login
{
    public class LoginModule(IPlayer player) : IBaseModule
    {
        private readonly IPlayer _player = player;

        public async Task InitAsync()
        {
            await Task.CompletedTask;
        }

        public async Task DestroyAsync()
        {
            await Task.CompletedTask;
        }

        public async Task<IMessage> LoginAsync()
        {
            Loggers.Chat.Info("hello world");
            await _player.SendMessageAsync(new SCTodoResp()
            {
                Todo = "hotfix Login"
            });
            return new SCLoginResp()
            {
                RoleId = _player.RoleId,
                ServerId = 1,
                ErrCode = ErrCode.Ok
            };
        }

        public async Task<IMessage> ReLoginAsync(uint serverSerialId)
        {
            //if (_player.CacheMessageQueue.Count() > 0 && _player.CacheMessageQueue.Peek().ServerSerialId <= serverSerialId)
            //{
            //    await _player.SendMessageBatchAsync(_player.CacheMessageQueue, true);
            //    return new SCReLoginResp()
            //    {
            //        RoleId = _player.RoleId,
            //        ServerId = 1,
            //        ErrCode = ErrCode.Ok
            //    };
            //}
            //else
            //{
            //    return new SCReLoginResp()
            //    {
            //        RoleId = _player.RoleId,
            //        ServerId = 1,
            //        ErrCode = ErrCode.ReloginFailed
            //    };
            //}
            return new SCReLoginResp()
            {
                RoleId = _player.RoleId,
                ServerId = 1,
                ErrCode = ErrCode.ReloginFailed
            };
        }
    }
}