/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 3/18/2025, 11:06:49 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Module;
using ChatServer.Grains;
using Network.Protos;
using Google.Protobuf;

namespace ChatServer.Modules.Login
{
    public class LoginModule(PlayerGrain grain) : IBaseModule
    {
        private readonly PlayerGrain _grain = grain;

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
            await _grain.SendMessageAsync(new SCTodoResp()
            {
                Todo = "hotfix Login"
            });
            return new SCLoginResp()
            {
                RoleId = _grain.RoleId,
                ServerId = 1,
                ErrCode = ErrCode.Ok
            };
        }

        public async Task<IMessage> ReLoginAsync(uint serverSerialId)
        {
            if (_grain.CacheMessageQueue.Count() > 0 && _grain.CacheMessageQueue.Peek().ServerSerialId <= serverSerialId)
            {
                await _grain.SendMessageBatchAsync(_grain.CacheMessageQueue, true);
                return new SCReLoginResp()
                {
                    RoleId = _grain.RoleId,
                    ServerId = 1,
                    ErrCode = ErrCode.Ok
                };
            }
            else
            {
                return new SCReLoginResp()
                {
                    RoleId = _grain.RoleId,
                    ServerId = 1,
                    ErrCode = ErrCode.ReloginFailed
                };
            }
        }
    }

}