using Bogus;
using FluentAssertions;
using Flunt.Notifications;
using Flunt.Validations;
using Octo.Core.Primitives;

namespace Octo.Core.UnitTests.Primitives;

public sealed class ResultTests
{
    private readonly Faker _faker = new("pt_BR");

    [Fact]
    public void Success_WithValidValue_ShouldBeValidAndExposeValue()
    {
        var expectedName = _faker.Person.FullName;

        var result = Result.Success(expectedName);

        result.IsValid.Should().BeTrue();
        result.Notifications.Should().BeEmpty();
        result.Value.Should().Be(expectedName);
    }

    [Fact]
    public void Failure_WithInvalidNotifiable_ShouldBeInvalidAndPropagateNotifications()
    {
        var validations = new Contract<Notification>()
            .Requires();
        validations.AddNotification("Name", "Name is required.");

        var result = Result.Failure<string>(validations);

        result.IsValid.Should().BeFalse();
        result.Notifications.Should().ContainSingle(n => n.Key == "Name");
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Success_WithInvalidNotifiableValue_ShouldCaptureNotificationsInsteadOfSettingValue()
    {
        var invalidEntity = new DummyNotifiable(isValid: false);

        var result = Result.Success(invalidEntity);

        result.IsValid.Should().BeFalse();
        result.Notifications.Should().NotBeEmpty();
        result.Value.Should().BeNull();
    }

    private sealed class DummyNotifiable : Notifiable<Notification>
    {
        public DummyNotifiable(bool isValid)
        {
            if (!isValid)
            {
                AddNotification("Dummy", "Invalid state.");
            }
        }
    }
}