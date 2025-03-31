/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 3/26/2025, 11:42:09 AM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Utils.LoggerUtil;
using Abstractions.Module;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace ChatServer.Extensions
{
    public static class HotFixEngine
    {
        private static ScriptOptions GetScriptOptions()
        {
            return ScriptOptions.Default
                .AddReferences(typeof(IBaseModule).Assembly)
                .AddReferences(typeof(object).Assembly)
                .AddImports("System", "ChatServer");
        }

        public static async Task<IBaseModule?> LoadModuleFromScriptAsync(string scriptPath)
        {
            try
            {
                var code = await File.ReadAllTextAsync(scriptPath);
                var script = CSharpScript.Create<IBaseModule>(
                    code,
                    options: GetScriptOptions(),
                    globalsType: null
                );

                var compiledScript = script.Compile();
                if (compiledScript.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    Loggers.Chat.Error($"LoadModuleFromScriptAsync error:{compiledScript}");
                    return null;
                }
                return await script.RunAsync().ContinueWith(t => t.Result.ReturnValue);
            }
            catch (Exception ex)
            {
                Loggers.Chat.Error(ex, $"LoadModuleFromScriptAsync error:{ex.Message}");
                return null;
            }
        }

    }
}