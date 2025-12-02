using System.Collections.Generic;
using System.Threading.Tasks;
using MobileTracker.Models;
using MobileTracker.Services;
using MobileTracker.ViewModels;
using Moq;
using Xunit;

namespace MobileTracker.UnitTests.ViewModels
{
    public class ClientViewModelTests
    {
        [Fact]
        public async Task LoadClientsAsync_PopulatesList()
        {
            var mockDb = new Mock<IFirebaseDatabaseService>();
            var data = new Dictionary<string, Client>
            {
                { "k1", new Client { Name = "Acme" } },
                { "k2", new Client { Name = "Beta" } }
            };
            mockDb.Setup(d => d.GetAsync<Dictionary<string, Client>>("clients")).ReturnsAsync(data);

            var vm = new ClientViewModel(mockDb.Object);
            await vm.LoadClientsAsync();

            Assert.Equal(2, vm.Clients.Count);
            Assert.Contains(vm.Clients, c => c.Name == "Acme");
            Assert.Contains(vm.Clients, c => c.Name == "Beta");
        }

        [Fact]
        public async Task AddClientAsync_PushesToDbAndAddsToList()
        {
            var mockDb = new Mock<IFirebaseDatabaseService>();
            mockDb.Setup(d => d.PushAsync("clients", It.IsAny<Client>())).ReturnsAsync("new-key");

            var vm = new ClientViewModel(mockDb.Object) { NewClient = new Client { Name = "NewCo" } };
            await vm.AddClientAsync();

            mockDb.Verify(d => d.PushAsync("clients", It.IsAny<Client>()), Times.Once);
            Assert.Single(vm.Clients);
            Assert.Equal("new-key", vm.Clients[0].Id);
        }

        [Fact]
        public async Task UpdateClientAsync_CallsSetAsync()
        {
            var mockDb = new Mock<IFirebaseDatabaseService>();
            var client = new Client { Id = "key1", Name = "Ct" };
            var vm = new ClientViewModel(mockDb.Object) { SelectedClient = client };

            await vm.UpdateClientAsync();

            mockDb.Verify(d => d.SetAsync($"clients/{client.Id}", client), Times.Once);
        }

        [Fact]
        public async Task DeleteClientAsync_CallsDeleteAndRemoves()
        {
            var mockDb = new Mock<IFirebaseDatabaseService>();
            var client = new Client { Id = "k1", Name = "DelCo" };
            var vm = new ClientViewModel(mockDb.Object);
            vm.Clients.Add(client);
            vm.SelectedClient = client;

            await vm.DeleteClientAsync();

            mockDb.Verify(d => d.DeleteAsync($"clients/{client.Id}"), Times.Once);
            Assert.Empty(vm.Clients);
        }
    }
}
