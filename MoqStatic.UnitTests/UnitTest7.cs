using System.Reflection;

namespace MoqStatic.UnitTests;

using Moq;
using HarmonyLib;
using Xunit;

public /* static */ class Test8 : ITest8 {
    public static int Add(int a, int b)
    {
        return a + b;
    }

    int ITest8.Add(int a, int b) =>
        (int)Test8Patches.Add.Original.Invoke(null, new object[] { a, b });
}

public interface ITest8
{
    int Add(int a, int b);
}

// Necessary info to generate this class:
// - StaticClassName
// - All method signatures
// public class VirtualTest8
// {
//     public virtual int Add(int a, int b) =>
//         (int)Test8Patches.Add.Original.Invoke(null, new object[] { a, b });
// }

// Necessary info to generate this class:
// - StaticClassName
// - MockBehavior from the annotation
// - CallBase from the annotation
public class Test8MockProvider
{
    internal static Mock<ITest8> Instance;

    public static Mock<ITest8> Get()
    {
        if (Instance == null)
        {
            Instance = new Mock<Test8>(MockBehavior.Loose) { CallBase = true }.As<ITest8>();
            // Redirect all Test8 static method calls to this mock.
            Test8Patches.PatchAll();
        }
        return Instance;
    }
}

// Necessary info to generate this class:
// - StaticClassName
// - All method signatures
public class Test8Patches
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
            var targetMethod = AccessTools.Method(typeof(Test8), nameof(Test8.Add));
            Original = MoqStatic.Harmony.Patch(targetMethod);

            // Redirect the method call to Prefix.
            var prefix = AccessTools.Method(typeof(Add), nameof(Prefix));
            MoqStatic.Harmony.Patch(targetMethod, new HarmonyMethod(prefix));
        }

        // If Prefix is called, then the patch is enabled.
        public static bool Prefix(int a, int b, ref int __result)
        {
            __result = Test8MockProvider.Instance.Object.Add(a, b);
            return false; // Don't run the original.
        }
    }
}

[ProvideStaticMock(typeof(Test8))]
public partial class UnitTest7
{
    [Fact]
    public void Main()
    {
        var original = Test8.Add(1, 2);
        
        Assert.Equal(3, original);
        
        Test8Mock.Setup(it => it.Add(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(10);

        var actual = Test8.Add(1, 2);
        
        Assert.Equal(10, actual);
    }
}

// Necessary info to generate this class:
// - StaticClassName
public partial class UnitTest7
{
    internal Mock<ITest8> Test8Mock = Test8MockProvider.Get();
}