using Moq;

namespace MoqStatic.UnitTests;

using System.Reflection;
using HarmonyLib;
using Xunit;

public static class Test4
{
    public static int Add(int a, int b)
    {
        return a + b;
    }
}

public class ITest4
{
    public virtual int Add(int a, int b)
    {
        return Test4Patches.Add.InvokeOriginal(a, b);
    }
}

public class Test4Patches
{
    public static Mock<ITest4> Mock;

    public class Add // MethodNameMethodIndex, where MethodIndex = "" if 0
    {
        public static MethodInfo? Original;

        public static bool Prefix(int a, int b, ref int __result)
        {
            // If the patch is called, then it is enabled.

            __result = Mock.Object.Add(a, b);

            return false; // Don't run the original.
        }

        public static int InvokeOriginal(int a, int b)
        {
            return (int)Original!.Invoke(null, new object[] { a, b });
        }
    }
}

public class UnitTest6
{
    [Fact]
    public void Main()
    {
        var harmony = new Harmony("com.github.wdestroier.moqstatic");
        
        // Save an original copy of each method.
        var targetMethod = AccessTools.Method(typeof(Test4), nameof(Test4.Add));
        var originalMethod = harmony.Patch(targetMethod);
        Test4Patches.Add.Original = originalMethod;

        // Check if the behavior wasn't affected.
        var resultX = Test4.Add(1, 2);
        Assert.Equal(3, resultX);

        // Setup mock.
        Test4Patches.Mock = new Mock<ITest4>(MockBehavior.Loose)
        {
            CallBase = true
        };
        var mock = Test4Patches.Mock;

        // Redirect all method calls to Prefix.
        var prefix = AccessTools.Method(typeof(Test4Patches.Add), nameof(Test4Patches.Add.Prefix));
        // var postfix = AccessTools.Method(typeof(Test4Patches.Add), nameof(Test4Patches.Add.Postfix));
        harmony.Patch(targetMethod, new HarmonyMethod(prefix));

        // Test if the base method is called (CallBase = true).
        var resultB = Test4.Add(1, 2);
        Assert.Equal(3, resultB);

        // Test if the Harmony patch is working.
        mock.CallBase = false;
        var resultC = Test4.Add(1, 2); // The result will be 0, because the Prefix patch will call the mock.
        Assert.Equal(0, resultC);

        mock.Setup(it => it.Add(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(10);
        
        // Test if Harmony is calling the mock.
        var resultD = Test4.Add(1, 2);
        Assert.Equal(10, resultD);
        
        harmony.UnpatchAll();
    }
}