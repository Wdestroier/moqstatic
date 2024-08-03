using Moq;
using Xunit;

namespace MoqStatic.UnitTests;

public class ProductService
{
    public virtual int GetFirstId() => 1;
    
    public static int GetSecondId() => 2;
}

public class UnitTest5
{
    [Fact]
    public void MockVirtualMethod()
    {
        var mockProductRepository = new Mock<ProductService>();
        mockProductRepository.Setup(it => it.GetFirstId()).Returns(5);
        
        var actualId = mockProductRepository.Object.GetFirstId();

        Assert.Equal(5, actualId);
    }
    
    [Fact]
    public void MockStaticMethod()
    {
        var mockProductRepository = new Mock<ProductService>();
        mockProductRepository.Setup(it => ProductService.GetSecondId()).Returns(5);
        
        var actualId = mockProductRepository.Object.GetFirstId();

        Assert.Equal(5, actualId);
        // InheritanceTestB.StaticMethod();
    }
}

class InheritanceTestA
{
    public static void StaticMethod()
    {
    }
}

class InheritanceTestB : InheritanceTestA
{
    public void StaticMethod()
    {
    }
}