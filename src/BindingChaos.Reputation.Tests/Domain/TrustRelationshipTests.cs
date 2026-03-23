using BindingChaos.Reputation.Domain.TrustRelationships;
using BindingChaos.SharedKernel.Domain;
using BindingChaos.SharedKernel.Domain.Exceptions;
using BindingChaos.SharedKernel.Domain.Services;
using BindingChaos.SharedKernel.Infrastructure.Services;
using FluentAssertions;

namespace BindingChaos.Reputation.Tests.Domain;

public class TrustRelationshipTests
{
    private static readonly ParticipantId Alice = new("participant-alice");
    private static readonly ParticipantId Bob = new("participant-bob");
    private static readonly DateTimeOffset FixedTime = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithValidIds_SetsFieldsCorrectly()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(FixedTime));
        try
        {
            var relationship = TrustRelationship.Create(Alice, Bob);

            relationship.TrusterId.Should().Be(Alice);
            relationship.TrusteeId.Should().Be(Bob);
            relationship.CreatedAt.Should().Be(FixedTime);
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public void Create_WhenTrusterEqualsTrustee_ThrowsInvariantViolationException()
    {
        var act = () => TrustRelationship.Create(Alice, Alice);

        act.Should().Throw<InvariantViolationException>();
    }

    [Fact]
    public void Equality_SameCompositeKey_DifferentCreatedAt_AreEqual()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(FixedTime));
        try
        {
            var r1 = TrustRelationship.Create(Alice, Bob);
            TimeProviderContext.SetCurrent(new ControllableTimeProvider(FixedTime.AddDays(1)));
            var r2 = TrustRelationship.Create(Alice, Bob);

            r1.Should().Be(r2);
            (r1 == r2).Should().BeTrue();
            (r1 != r2).Should().BeFalse();
            r1.GetHashCode().Should().Be(r2.GetHashCode());
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public void Equality_DifferentCompositeKey_AreNotEqual()
    {
        TimeProviderContext.SetCurrent(new ControllableTimeProvider(FixedTime));
        try
        {
            var r1 = TrustRelationship.Create(Alice, Bob);
            var r2 = TrustRelationship.Create(Bob, Alice);

            r1.Should().NotBe(r2);
            (r1 == r2).Should().BeFalse();
            (r1 != r2).Should().BeTrue();
        }
        finally
        {
            TimeProviderContext.Reset();
        }
    }

    [Fact]
    public void Create_WhenTrusterAndTrusteeHaveSameValue_ThrowsInvariantViolationException()
    {
        var id1 = new ParticipantId("participant-alice");
        var id2 = new ParticipantId("participant-alice");

        var act = () => TrustRelationship.Create(id1, id2);

        act.Should().Throw<InvariantViolationException>();
    }
}
