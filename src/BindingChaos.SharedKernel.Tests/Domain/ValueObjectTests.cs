using BindingChaos.SharedKernel.Domain;
using FluentAssertions;

namespace BindingChaos.SharedKernel.Tests.Domain;

public class ValueObjectTests
{
    [Theory]
    [InlineData("test", 42, "test", 42, true)]
    [InlineData("test", 42, "different", 42, false)]
    [InlineData("test", 42, "test", 24, false)]
    public void Equality_ShouldCompareByValue(string value1String, int value1Int, string value2String, int value2Int, bool shouldBeEqual)
    {
        var value1 = new TestValueObject(value1String, value1Int);
        var value2 = new TestValueObject(value2String, value2Int);

        value1.Equals(value2).Should().Be(shouldBeEqual);
        (value1 == value2).Should().Be(shouldBeEqual);
        (value1 != value2).Should().Be(!shouldBeEqual);

        if (shouldBeEqual)
        {
            value1.GetHashCode().Should().Be(value2.GetHashCode());
        }
    }

    [Fact]
    public void Equality_WithNull_ShouldReturnFalse()
    {
        var value = new TestValueObject("test", 42);
        TestValueObject? nullValue = null;

        value.Equals(null).Should().BeFalse();
        (value == null).Should().BeFalse();
        (value != null).Should().BeTrue();
        nullValue.Should().BeNull();
    }

    [Fact]
    public void Equality_WithDifferentTypes_ShouldReturnFalse()
    {
        var value = new TestValueObject("test", 42);
        var otherValue = new OtherTestValueObject("test", 42);

        value.Equals(otherValue).Should().BeFalse();
        (value == otherValue).Should().BeFalse();
    }

    [Fact]
    public void Equality_WithComplexComponents_ShouldWorkCorrectly()
    {
        var value1 = new ComplexTestValueObject("test", 42, new[] { "a", "b", "c" });
        var value2 = new ComplexTestValueObject("test", 42, new[] { "a", "b", "c" });
        var value3 = new ComplexTestValueObject("test", 42, new[] { "a", "b", "d" });

        value1.Should().Be(value2);
        value1.Should().NotBe(value3);
    }

    [Fact]
    public void GetHashCode_WithNullComponents_ShouldHandleNulls()
    {
        var value1 = new TestValueObjectWithNulls(null, 42);
        var value2 = new TestValueObjectWithNulls(null, 42);

        value1.GetHashCode().Should().Be(value2.GetHashCode());
        value1.Should().Be(value2);
    }

    [Fact]
    public void ToString_ShouldReturnTypeName_ByDefault()
    {
        var value = new TestValueObject("test", 42);

        var result = value.ToString();

        result.Should().Be("TestValueObject(test, 42)");
    }

    private class TestValueObject : ValueObject
    {
        public string StringValue { get; }
        public int IntValue { get; }

        public TestValueObject(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
        }
    }

    private class OtherTestValueObject : ValueObject
    {
        public string StringValue { get; }
        public int IntValue { get; }

        public OtherTestValueObject(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
        }
    }

    private class TestValueObjectWithNulls : ValueObject
    {
        public string? StringValue { get; }
        public int IntValue { get; }

        public TestValueObjectWithNulls(string? stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue!;
            yield return IntValue;
        }
    }

    private class ComplexTestValueObject : ValueObject
    {
        public string StringValue { get; }
        public int IntValue { get; }
        public string[] ArrayValue { get; }

        public ComplexTestValueObject(string stringValue, int intValue, string[] arrayValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
            ArrayValue = arrayValue;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StringValue;
            yield return IntValue;
            foreach (var item in ArrayValue)
            {
                yield return item;
            }
        }
    }
}