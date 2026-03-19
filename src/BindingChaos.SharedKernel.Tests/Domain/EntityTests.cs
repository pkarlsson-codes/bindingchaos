using BindingChaos.SharedKernel.Domain;

namespace BindingChaos.SharedKernel.Tests.Domain;

public class EntityTests
{
    [Fact]
    public void Equality_ShouldBeBasedOnId()
    {
        var id1 = TestEntityId.Create("test-1");
        var id2 = TestEntityId.Create("test-2");
        var entity1 = new TestEntity(id1, "test");
        var entity2 = new TestEntity(id1, "different"); // Same ID, different content
        var entity3 = new TestEntity(id2, "test");

        Assert.Equal(entity1, entity2); // Same ID = equal
        Assert.True(entity1 == entity2);
        Assert.False(entity1 != entity2);

        Assert.NotEqual(entity1, entity3); // Different ID = not equal
        Assert.False(entity1 == entity3);
        Assert.True(entity1 != entity3);
    }

    [Fact]
    public void Equality_WithNull_ShouldReturnFalse()
    {
        var id = TestEntityId.Create("test-1");
        var entity = new TestEntity(id, "test");

        Assert.False(entity.Equals(null));
        Assert.False(entity == null);
        Assert.True(entity != null);
    }

    [Fact]
    public void GetHashCode_ShouldBeBasedOnId()
    {
        var id1 = TestEntityId.Create("test-1");
        var id2 = TestEntityId.Create("test-2");
        var entity1 = new TestEntity(id1, "test");
        var entity2 = new TestEntity(id1, "different");
        var entity3 = new TestEntity(id2, "test");

        Assert.Equal(entity1.GetHashCode(), entity2.GetHashCode());
        Assert.NotEqual(entity1.GetHashCode(), entity3.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnEntityTypeAndId()
    {
        var id = TestEntityId.Create("test-1");
        var entity = new TestEntity(id, "test");

        var result = entity.ToString();

        Assert.Equal("TestEntity[test-1]", result);
    }

    [Fact]
    public void TransientEntity_ShouldNotEqualAnyOtherEntity()
    {
        var transientEntity = new TestEntity("test"); // No ID
        var normalEntity = new TestEntity(TestEntityId.Create("test-1"), "test");

        Assert.False(transientEntity.Equals(normalEntity));
        Assert.False(normalEntity.Equals(transientEntity));
        Assert.False(transientEntity.Equals(transientEntity)); // Transient entities don't equal themselves
    }

    private class TestEntity : Entity<TestEntityId>
    {
        public string Name { get; }

        public TestEntity(TestEntityId id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public TestEntity(string name)
            : base() // Transient entity
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public sealed class TestEntityId : EntityId<TestEntityId>
    {
        public TestEntityId(string value) : base(value, "test") { }
    }
}