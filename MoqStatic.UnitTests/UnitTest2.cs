using System.Reflection;
using HarmonyLib;
using Moq;
using Xunit;

namespace MoqStatic.UnitTests;

public partial class UnitTest2
{
    [Fact]
    public void FirstMethodIsNotCalledBySecondMethod()
    {
        StaticClassMock
            .Setup(it => it.FirstStaticMethod(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(5);
        
        var result = StaticClass.SecondStaticMethod(3, 3);
        
        Assert.NotEqual(9, result);
        Assert.Equal(5, result);
    }
}

public static class StaticClass
{
    public static int FirstStaticMethod(int first, int second) => first + second;

    public static int SecondStaticMethod(int first, int second)
    {
        var result = 0;
            
        for (var i = 0; i < second; i++)
            result = FirstStaticMethod(result, first);
            
        return result;
    }
}

public interface IStaticClass
{
    int FirstStaticMethod(int first, int second);
    int SecondStaticMethod(int first, int second);
}

public sealed class StaticClassImpl : IStaticClass
{
    private static Mock<IStaticClass> _mock;
    public static Mock<IStaticClass> Mock
    {
        get
        {
            if (_mock == null)
            {
                _mock = new Mock<IStaticClass>(MockBehavior.Default);
                StaticClassPatches.ApplyAll();
            }

            return _mock;
        }
    }

    public int FirstStaticMethod(int first, int second) =>
        Mock.Object.FirstStaticMethod(first, second);

    public int SecondStaticMethod(int a, int b) =>
        Mock.Object.SecondStaticMethod(a, b);
}

public class StaticClassPatches
{
    public static MethodInfo OriginalFirstStaticMethod;

    public static void ApplyAll()
    {
        var harmony = new Harmony("com.example.patch");
        
        harmony.PatchAll();
    }
}

[HarmonyPatch(typeof(UnitTest1.StaticClass))]
public class StaticClassPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StaticClass.FirstStaticMethod))]
    public static bool FirstStaticMethodPrefix(int first, int second)
    {
        var shouldSkip = !StaticClassImpl.Mock.CallBase;

        return shouldSkip;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StaticClass.FirstStaticMethod))]
    public static int FirstStaticMethodPostfix(ref int __result)
    {
        return __result;
    }
}

public partial class UnitTest2
{
    internal Mock<IStaticClass> StaticClassMock = StaticClassImpl.Mock;
}