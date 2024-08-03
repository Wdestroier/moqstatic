using Moq;

namespace MoqStatic.UnitTests;

using Xunit;
using HarmonyLib;

public class UnitTest1
{
    private Harmony Harmony = new("com.github.wdestroier.moqstatic");
    
    public static class StaticClass
    {
    }

    [Fact]
    public void StaticClassMock()
    {
        var mock = new ProvideStaticMockAttribute(typeof(StaticClass));
        
        Assert.NotNull(mock);
    }
    
    public interface IInstanceInterface
    {
        int Property { get; set; }
    }
    
    public interface IStaticInterface
    {
        static int Property { get; set; }
    }

    [Fact]
    public void MockInstanceProperty()
    {
        var mock = new Mock<IInstanceInterface>();

        mock.Setup(x => x.Property).Returns(42);

        int value = mock.Object.Property;

        Assert.Equal(42, value);
    }
    
    public class ConcreteInstanceClass
    {
        public virtual int Add(int a, int b) => a + b;

        public virtual int Multiply(int a, int b)
        {
            var result = 0;
            
            for (var i = 0; i < b; i++)
                result = Add(result, a);
            
            return result;
        }
    }
    
    public class ConcreteStaticClass
    {
        public static int Add(int a, int b) => a + b;

        public static int Multiply(int a, int b)
        {
            var result = 0;
            
            for (var i = 0; i < b; i++)
                result = Add(result, a);
            
            return result;
        }
    }
    
    public class ConcreteHybridClass
    {
        public static int Add(int a, int b) => a + b;

        public virtual int Multiply(int a, int b)
        {
            var result = 0;
            
            for (var i = 0; i < b; i++)
                result = Add(result, a);
            
            return result;
        }
    }

    [Fact]
    public void MockConcreteInstanceClassAndCallBase()
    {
        var mockCalculator = new Mock<ConcreteInstanceClass> { CallBase = true };
        mockCalculator
            .Setup(it => it.Add(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(100);

        // 100, the value in the mock.
        int addResult = mockCalculator.Object.Add(5, 3);
        // 100, instead of returning the default value of 0, calls the Add method, because
        // CallBase is true.
        int multiplyResult = mockCalculator.Object.Multiply(5, 3);
        
        Assert.Equal(100, multiplyResult);
    }
    
    // [Fact]
    // public void MockConcreteHybridClassAndCallBase()
    // {
    //     var mockCalculator = new Mock<ConcreteHybridClass> { CallBase = true };
    //     mockCalculator
    //         .Setup(it => it.Add(It.IsAny<int>(), It.IsAny<int>()))
    //         .Returns(100);
    //
    //     int multiplyResult = mockCalculator.Object.Multiply(5, 3);
    //     
    //     Assert.Equal(100, multiplyResult);
    // }
}