using BindingChaos.SharedKernel.Specifications;
using System.Linq.Expressions;

namespace BindingChaos.SharedKernel.Tests.Specifications;

public class SpecificationTests
{
    [Fact]
    public void All_ReturnsIdentitySpecification()
    {

        var spec = Specification<TestEntity>.All;


        Assert.NotNull(spec);
        Assert.True(spec.IsSatisfiedBy(new TestEntity("test", 42)));
    }

    [Fact]
    public void IsSatisfiedBy_WithValidEntity_ReturnsTrue()
    {

        var spec = new TestSpecification("test", 42);
        var entity = new TestEntity("test", 42);


        var result = spec.IsSatisfiedBy(entity);


        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithInvalidEntity_ReturnsFalse()
    {

        var spec = new TestSpecification("test", 42);
        var entity = new TestEntity("different", 42);


        var result = spec.IsSatisfiedBy(entity);


        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_WithNullEntity_ThrowsArgumentNullException()
    {

        var spec = new TestSpecification("test", 42);


        Assert.Throws<ArgumentNullException>(() => spec.IsSatisfiedBy(null!));
    }

    [Fact]
    public void And_WithBothTrue_ReturnsTrue()
    {

        var spec1 = new TestSpecification("test", 42);
        var spec2 = new TestSpecification("test", 42);
        var entity = new TestEntity("test", 42);


        var combined = spec1.And(spec2);
        var result = combined.IsSatisfiedBy(entity);


        Assert.True(result);
    }

    [Fact]
    public void And_WithOneFalse_ReturnsFalse()
    {

        var spec1 = new TestSpecification("test", 42);
        var spec2 = new TestSpecification("different", 42);
        var entity = new TestEntity("test", 42);


        var combined = spec1.And(spec2);
        var result = combined.IsSatisfiedBy(entity);


        Assert.False(result);
    }

    [Fact]
    public void And_WithNullSpecification_ThrowsArgumentNullException()
    {

        var spec = new TestSpecification("test", 42);


        Assert.Throws<ArgumentNullException>(() => spec.And(null!));
    }

    [Fact]
    public void And_WithAllSpecification_ReturnsOtherSpecification()
    {

        var spec = new TestSpecification("test", 42);


        var result = spec.And(Specification<TestEntity>.All);


        Assert.Same(spec, result);
    }

    [Fact]
    public void And_WithAllSpecification_ReturnsThisSpecification()
    {

        var spec = new TestSpecification("test", 42);


        var result = Specification<TestEntity>.All.And(spec);


        Assert.Same(spec, result);
    }

    [Fact]
    public void Or_WithOneTrue_ReturnsTrue()
    {

        var spec1 = new TestSpecification("test", 42);
        var spec2 = new TestSpecification("different", 42);
        var entity = new TestEntity("test", 42);


        var combined = spec1.Or(spec2);
        var result = combined.IsSatisfiedBy(entity);


        Assert.True(result);
    }

    [Fact]
    public void Or_WithBothFalse_ReturnsFalse()
    {

        var spec1 = new TestSpecification("test", 42);
        var spec2 = new TestSpecification("different", 42);
        var entity = new TestEntity("other", 42);


        var combined = spec1.Or(spec2);
        var result = combined.IsSatisfiedBy(entity);


        Assert.False(result);
    }

    [Fact]
    public void Or_WithNullSpecification_ThrowsArgumentNullException()
    {

        var spec = new TestSpecification("test", 42);


        Assert.Throws<ArgumentNullException>(() => spec.Or(null!));
    }

    [Fact]
    public void Or_WithAllSpecification_ReturnsAllSpecification()
    {

        var spec = new TestSpecification("test", 42);


        var result = spec.Or(Specification<TestEntity>.All);


        Assert.Same(Specification<TestEntity>.All, result);
    }

    [Fact]
    public void Not_WithTrueSpecification_ReturnsFalse()
    {

        var spec = new TestSpecification("test", 42);
        var entity = new TestEntity("test", 42);


        var negated = spec.Not();
        var result = negated.IsSatisfiedBy(entity);


        Assert.False(result);
    }

    [Fact]
    public void Not_WithFalseSpecification_ReturnsTrue()
    {

        var spec = new TestSpecification("test", 42);
        var entity = new TestEntity("different", 42);


        var negated = spec.Not();
        var result = negated.IsSatisfiedBy(entity);


        Assert.True(result);
    }

    [Fact]
    public void ToExpression_ReturnsValidExpression()
    {

        var spec = new TestSpecification("test", 42);


        var expression = spec.ToExpression();


        Assert.NotNull(expression);
        Assert.Equal(typeof(Func<TestEntity, bool>), expression.Type);
    }



    [Fact]
    public void Specification_WithNullEntityProperty_ShouldHandleGracefully()
    {

        var spec = new NullableTestSpecification("test");
        var entityWithNull = new NullableTestEntity(null, 42);
        var entityWithValue = new NullableTestEntity("test", 42);


        Assert.False(spec.IsSatisfiedBy(entityWithNull), "Entity with null property should not match");
        Assert.True(spec.IsSatisfiedBy(entityWithValue), "Entity with matching value should match");
    }

    [Fact]
    public void Specification_WithInvalidExpression_ShouldThrowAppropriateException()
    {

        var spec = new InvalidExpressionSpecification();


        var exception = Assert.Throws<InvalidOperationException>(() => spec.IsSatisfiedBy(new TestEntity("test", 42)));
        Assert.Contains("Invalid expression", exception.Message);
    }

    [Fact]
    public void Specification_WithEmptyStringValues_ShouldHandleCorrectly()
    {

        var spec = new TestSpecification("", 0);
        var entityWithEmpty = new TestEntity("", 0);
        var entityWithValues = new TestEntity("test", 42);


        Assert.True(spec.IsSatisfiedBy(entityWithEmpty), "Entity with empty values should match");
        Assert.False(spec.IsSatisfiedBy(entityWithValues), "Entity with non-empty values should not match");
    }

    [Fact]
    public void Specification_WithBoundaryValues_ShouldHandleCorrectly()
    {

        var spec = new BoundaryTestSpecification(int.MaxValue, int.MinValue);
        var entityWithMax = new BoundaryTestEntity("test", int.MaxValue);
        var entityWithMin = new BoundaryTestEntity("test", int.MinValue);
        var entityWithNormal = new BoundaryTestEntity("test", 0);


        Assert.True(spec.IsSatisfiedBy(entityWithMax), "Entity with max value should match");
        Assert.True(spec.IsSatisfiedBy(entityWithMin), "Entity with min value should match");
        Assert.False(spec.IsSatisfiedBy(entityWithNormal), "Entity with normal value should not match");
    }



    private class TestEntity
    {
        public string Name { get; }
        public int Value { get; }

        public TestEntity(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    private class TestSpecification : Specification<TestEntity>
    {
        private readonly string _expectedName;
        private readonly int _expectedValue;

        public TestSpecification(string expectedName, int expectedValue)
        {
            _expectedName = expectedName;
            _expectedValue = expectedValue;
        }

        public override Expression<Func<TestEntity, bool>> ToExpression()
        {
            return entity => entity.Name == _expectedName && entity.Value == _expectedValue;
        }
    }

    private class NullableTestEntity
    {
        public string? Name { get; }
        public int Value { get; }

        public NullableTestEntity(string? name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    private class NullableTestSpecification : Specification<NullableTestEntity>
    {
        private readonly string _expectedName;

        public NullableTestSpecification(string expectedName)
        {
            _expectedName = expectedName;
        }

        public override Expression<Func<NullableTestEntity, bool>> ToExpression()
        {
            return entity => entity.Name != null && entity.Name == _expectedName;
        }
    }

    private class InvalidExpressionSpecification : Specification<TestEntity>
    {
        public override Expression<Func<TestEntity, bool>> ToExpression()
        {
            throw new InvalidOperationException("Invalid expression");
        }
    }

    private class BoundaryTestEntity
    {
        public string Name { get; }
        public int Value { get; }

        public BoundaryTestEntity(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }

    private class BoundaryTestSpecification : Specification<BoundaryTestEntity>
    {
        private readonly int _maxValue;
        private readonly int _minValue;

        public BoundaryTestSpecification(int maxValue, int minValue)
        {
            _maxValue = maxValue;
            _minValue = minValue;
        }

        public override Expression<Func<BoundaryTestEntity, bool>> ToExpression()
        {
            return entity => entity.Value == _maxValue || entity.Value == _minValue;
        }
    }
}