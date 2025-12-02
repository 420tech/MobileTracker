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

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool hasSuccessMessage;

        [ObservableProperty]
        private string? successMessage;

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
            HasError = false;
            HasSuccessMessage = false;
            if (NewClient == null) return;
            try
            {
                NewClient.Validate();
                var key = await _db.PushAsync("clients", NewClient);
                NewClient.Id = key;
                Clients.Add(NewClient);
                SuccessMessage = "Client added";
                HasSuccessMessage = true;
                NewClient = new Client();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        [RelayCommand]
        public async Task UpdateClientAsync()
        {
            HasError = false;
            HasSuccessMessage = false;
            if (SelectedClient == null) return;
            try
            {
                SelectedClient.Validate();
                await _db.SetAsync($"clients/{SelectedClient.Id}", SelectedClient);
                SuccessMessage = "Client updated";
                HasSuccessMessage = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        [RelayCommand]
        public async Task DeleteClientAsync()
        {
            HasError = false;
            HasSuccessMessage = false;
            if (SelectedClient == null) return;
            try
            {
                await _db.DeleteAsync($"clients/{SelectedClient.Id}");
                Clients.Remove(SelectedClient);
                SelectedClient = null;
                SuccessMessage = "Client deleted";
                HasSuccessMessage = true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }
    }
}