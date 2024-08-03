using System.Reflection;

namespace MoqStatic.UnitTests;

using Moq;
using HarmonyLib;
using Xunit;

public static class Test7 {
    public static int Add(int a, int b)
    {
        return a + b;
    }
}

// Necessary info to generate this class:
// - StaticClassName
// - All method signatures
public class VirtualTest7
{
    // System.ArgumentException
    //     MethodInfo must be a runtime MethodInfo object. (Parameter 'method')
    //     at System.Delegate.CreateDelegate(Type type, Object firstArgument, MethodInfo method, Boolean throwOnBindFailure)
    // at MoqStatic.UnitTests.Test8Patches.Add.Patch() in 
    public virtual int Add(int a, int b) =>
        (int)Test7Patches.Add.Original.Invoke(null, new object[] { a, b });
}

// Necessary info to generate this class:
// - StaticClassName
// - MockBehavior from the annotation
// - CallBase from the annotation
public class Test7MockProvider
{
    internal static Mock<VirtualTest7> Instance;

    public static Mock<VirtualTest7> Get()
    {
        if (Instance == null)
        {
            Instance = new Mock<VirtualTest7>(MockBehavior.Loose) { CallBase = true };
            // Redirect all Test7 static method calls to this mock.
            Test7Patches.PatchAll();
        }
        return Instance;
    }
}

// Necessary info to generate this class:
// - StaticClassName
// - All method signatures
public class Test7Patches
{
    // Redirect all method calls to Prefix.
    public static void PatchAll()
    {
        Add.Patch();
    }

    // Necessary info to generate this class:
    // - Method signature
    // - StaticClassName
    // MethodNameMethodIndex, where MethodIndex = "" if 0
    public class Add
    {
        public static MethodInfo Original { get; private set; }

        public static void Patch()
        {
            // Save an original copy of the method.
            var targetMethod = AccessTools.Method(typeof(Test7), nameof(Test7.Add));
            Original = MoqStatic.Harmony.Patch(targetMethod);

            // Redirect the method call to Prefix.
            var prefix = AccessTools.Method(typeof(Add), nameof(Prefix));
            MoqStatic.Harmony.Patch(targetMethod, new HarmonyMethod(prefix));
        }

        // If Prefix is called, then the patch is enabled.
        public static bool Prefix(int a, int b, ref int __result)
        {
            __result = Test7MockProvider.Instance.Object.Add(a, b);
            return false; // Don't run the original.
        }
    }
}

[ProvideStaticMock(typeof(Test7))]
public partial class UnitTest8
{
    [Fact]
    public void Main()
    {
        Test7Mock.Setup(it => it.Add(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(10);

        var actual = Test7.Add(1, 2);
        
        Assert.Equal(10, actual);
    }
}

// Necessary info to generate this class:
// - StaticClassName
public partial class UnitTest8
{
    internal Mock<VirtualTest7> Test7Mock = Test7MockProvider.Get();
}