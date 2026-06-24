using System.Reflection;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
namespace Demo.Scripts;

// 必须要加的属性，用于注册Mod。字符串和初始化函数命名一致。
[ModInitializer(nameof(Init))]
public class Entry
{
    public const string ModId = "demo";
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(ModId);
    // 初始化函数
    public static void Init()
    {
        // 打patch（即修改游戏代码的功能）用
        // 传入参数随意，只要不和其他人撞车即可
        // var harmony = new Harmony("sts2.reme.testmod");
        // harmony.PatchAll();
        // 使得tscn可以加载自定义脚本
        // ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        var assembly = Assembly.GetExecutingAssembly();
        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        // 自动注册内容
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);
        Log.Info("Mod initialized!");
    }
}
