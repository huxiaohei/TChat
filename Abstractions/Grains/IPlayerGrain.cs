/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:58
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Module;
using Abstractions.Message;

namespace Abstractions.Grains
{
    [Alias("Abstractions.Grains.IPlayerGrain")]
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        [Alias("PingAsync")]
        Task<bool> PingAsync();

        [Alias("ProcessMessageAsync")]
        Task<ISCMessage?> ProcessMessageAsync([Immutable] SiloAddress siloAddress, long sessionId, [Immutable] ICSMessage message);

        [Alias("HotfixModuleAsync")]
        Task<bool> HotfixModuleAsync(string hotfixAssemblyPath);
    }
}