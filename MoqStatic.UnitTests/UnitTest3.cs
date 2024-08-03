using System.Reflection;
using HarmonyLib;
using Xunit;

namespace MoqStatic.UnitTests;

public class Test3 {
    public static int Add(int a, int b)
    {
        return a + b;
    }
}

public class UnitTest3 {
    static bool Prefix()
    {
        return false;
    }
    static MethodInfo reentrantMethod;
    static void Add(int a, int b, ref int __result)
    {
        __result = (int)reentrantMethod.Invoke(null, new object[] { 3, 4 });
    }
    
    [Fact]
    public void Main()
    {
        var harmony = new Harmony("com.company.project.product");
        var original = AccessTools.Method(typeof(Test3), "Add");
        var prefix = AccessTools.Method(typeof(UnitTest3), "Prefix");
        var postfix = AccessTools.Method(typeof(UnitTest3), "Add");
        reentrantMethod = harmony.Patch(original);
        harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        var result = Test3.Add(1, 2);
        throw new Exception(message: $"XXX {result} XXX"); // XXX 7 XXX
    }
}