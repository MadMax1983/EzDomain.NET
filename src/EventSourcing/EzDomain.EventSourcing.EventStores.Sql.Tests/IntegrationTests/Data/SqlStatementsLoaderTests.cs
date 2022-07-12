using EzDomain.EventSourcing.EventStores.Sql.Data;
using FluentAssertions;
using NUnit.Framework;

namespace EzDomain.EventSourcing.EventStores.Sql.Tests.IntegrationTests.Data;

[TestFixture]
public sealed class SqlStatementsLoaderTests
{
    [Test]
    public void WHEN__THEN_()
    {
        // Arrange
        var loader = new SqlStatementsLoader();

        // Act
        loader.LoadScripts();

        // Assert
        loader["GetByAggregateRootId"].Should().NotBeNullOrWhiteSpace();
        loader["Save"].Should().NotBeNullOrWhiteSpace();
    }
}