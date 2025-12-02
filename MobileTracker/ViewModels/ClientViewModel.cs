using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Models;
using MobileTracker.Services;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;


namespace MobileTracker.ViewModels
{
    public partial class ClientViewModel : ObservableObject
    {
        private readonly IFirebaseDatabaseService _db;
        private readonly ILogger<ClientViewModel> _logger;

        [ObservableProperty]
        ObservableCollection<Client> clients = new ObservableCollection<Client>();

        [ObservableProperty]
        Client? selectedClient;

        [ObservableProperty]
        Client? newClient = new Client();

        [ObservableProperty]
        bool isBusy;

        public ClientViewModel(IFirebaseDatabaseService db, ILogger<ClientViewModel>? logger = null)
        {
            _db = db;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<ClientViewModel>.Instance;
        }

        [RelayCommand]
        public async Task LoadClientsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var dict = await _db.GetAsync<System.Collections.Generic.Dictionary<string, Client>>("clients");
                Clients.Clear();
                if (dict != null)
                {
                    foreach (var kv in dict)
                    {
                        var c = kv.Value;
                        c.Id = kv.Key;
                        Clients.Add(c);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddClientAsync()
        {
            if (NewClient == null) return;
            NewClient.Validate();
            var key = await _db.PushAsync("clients", NewClient);
            NewClient.Id = key;
            Clients.Add(NewClient);
            NewClient = new Client();
        }

        [RelayCommand]
        public async Task UpdateClientAsync()
        {
            if (SelectedClient == null) return;
            SelectedClient.Validate();
            await _db.SetAsync($"clients/{SelectedClient.Id}", SelectedClient);
        }

        [RelayCommand]
        public async Task DeleteClientAsync()
        {
            if (SelectedClient == null) return;
            await _db.DeleteAsync($"clients/{SelectedClient.Id}");
            Clients.Remove(SelectedClient);
            SelectedClient = null;
        }
    }
}