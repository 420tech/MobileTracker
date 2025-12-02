using System;
using MobileTracker.Models;
using Xunit;

namespace MobileTracker.UnitTests.Models
{
    public class DomainModelTests
    {
        [Fact]
        public void Client_Validate_ThrowsOnMissingName()
        {
            var c = new Client { Name = "" };
            Assert.Throws<ArgumentException>(() => c.Validate());
        }

        [Fact]
        public void Client_Validate_ThrowsOnBadEmail()
        {
            var c = new Client { Name = "Acme", ContactEmail = "not-an-email" };
            Assert.Throws<ArgumentException>(() => c.Validate());
        }

        [Fact]
        public void TimeEntry_Validate_RequiresClientIdAndNonNegativeDuration()
        {
            var t = new TimeEntry { ClientId = "", DurationMinutes = -5 };
            Assert.Throws<ArgumentException>(() => t.Validate());
            t.ClientId = "client-1";
            Assert.Throws<ArgumentOutOfRangeException>(() => t.Validate());
        }

        [Fact]
        public void Invoice_Validate_RequiresClientIdAndNonNegativeAmount()
        {
            var inv = new Invoice { ClientId = "", Amount = -10 };
            Assert.Throws<ArgumentException>(() => inv.Validate());
            inv.ClientId = "c1";
            Assert.Throws<ArgumentOutOfRangeException>(() => inv.Validate());
        }
    }
}
