using BlastPlanning.Domain.Aggregates.BlastPlan;
using BlastPlanning.Domain.Events;
using BlastPlanning.Domain.Exceptions;
using BlastPlanning.Domain.ValueObjects;
using FluentAssertions;

namespace BlastPlanning.Domain.Tests.Aggregates;

public sealed class BlastPlanTests
{
    [Fact]
    public void Create_Should_Raise_BlastPlanCreated_Event()
    {
        var blastPlanId = BlastPlanId.New();
        var occurredUtc = DateTimeOffset.UtcNow;

        var blastPlan = BlastPlan.Create(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            occurredUtc);

        blastPlan.UncommittedEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<BlastPlanCreated>()
            .Which
            .Should()
            .BeEquivalentTo(new BlastPlanCreated(
                blastPlanId,
                "Test Blast Plan",
                "site-001",
                occurredUtc));
    }

    [Fact]
    public void Approve_Should_Raise_BlastPlanApproved_Event()
    {
        var blastPlanId = BlastPlanId.New();
        var createdUtc = DateTimeOffset.UtcNow.AddMinutes(-5);
        var approvedUtc = DateTimeOffset.UtcNow;

        var blastPlan = BlastPlan.Create(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            createdUtc);

        blastPlan.ClearUncommittedEvents();

        blastPlan.Approve(
            "user-001",
            approvedUtc);

        blastPlan.UncommittedEvents
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeOfType<BlastPlanApproved>()
            .Which
            .Should()
            .BeEquivalentTo(new BlastPlanApproved(
                blastPlanId,
                "user-001",
                approvedUtc,
                approvedUtc));
    }

    [Fact]
    public void Approve_When_AlreadyApproved_Should_Throw_InvalidBlastPlanStateException()
    {
        var blastPlanId = BlastPlanId.New();

        var blastPlan = BlastPlan.Create(
            blastPlanId,
            "Test Blast Plan",
            "site-001",
            DateTimeOffset.UtcNow.AddMinutes(-10));

        blastPlan.ClearUncommittedEvents();

        blastPlan.Approve(
            "user-001",
            DateTimeOffset.UtcNow.AddMinutes(-5));

        blastPlan.ClearUncommittedEvents();

        var act = () => blastPlan.Approve(
            "user-002",
            DateTimeOffset.UtcNow);

        act.Should()
            .Throw<InvalidBlastPlanStateException>()
            .WithMessage("Only draft blast plans can be approved. Current status is 'Approved'.");
    }

    [Fact]
    public void FromHistory_Should_Rehydrate_BlastPlan_State()
    {
        var blastPlanId = BlastPlanId.New();
        var createdUtc = DateTimeOffset.UtcNow.AddMinutes(-10);
        var approvedUtc = DateTimeOffset.UtcNow.AddMinutes(-5);

        IDomainEvent[] history =
        [
            new BlastPlanCreated(
                blastPlanId,
                "Test Blast Plan",
                "site-001",
                createdUtc),

            new BlastPlanApproved(
                blastPlanId,
                "user-001",
                approvedUtc,
                approvedUtc)
        ];

        var blastPlan = BlastPlan.FromHistory(history);

        blastPlan.Id.Should().Be(blastPlanId);
        blastPlan.Name.Should().Be("Test Blast Plan");
        blastPlan.SiteId.Should().Be("site-001");
        blastPlan.Status.Should().Be(BlastPlanStatus.Approved);
        blastPlan.Version.Should().Be(1);
        blastPlan.UncommittedEvents.Should().BeEmpty();
    }
}