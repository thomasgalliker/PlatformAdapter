
using FluentAssertions;


using Xunit;

namespace PlatformAdapter.Tests
{
    [Collection("PlatformAdapter")]
    public class PlatformAdapterTests
    {
        [Fact]
        public void ShouldGetStaticPlatformAdapter()
        {
            // Act
            var adapter = PlatformAdapter.Current;

            // Assert
            adapter.Should().BeOfType<ProbingAdapterResolver>();
        }
    }
}
