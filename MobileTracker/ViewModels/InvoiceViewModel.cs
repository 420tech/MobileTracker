using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Models;
using MobileTracker.Services;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MobileTracker.ViewModels
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly IFirebaseDatabaseService _db;
        private readonly ILogger<InvoiceViewModel> _logger;

        [ObservableProperty]
        ObservableCollection<Invoice> invoices = new ObservableCollection<Invoice>();

        [ObservableProperty]
        Invoice? selectedInvoice;

        [ObservableProperty]
        Invoice? newInvoice = new Invoice();

        [ObservableProperty]
        bool isBusy;

        public InvoiceViewModel(IFirebaseDatabaseService db, ILogger<InvoiceViewModel>? logger = null)
        {
            _db = db;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<InvoiceViewModel>.Instance;
        }

        [RelayCommand]
        public async Task LoadInvoicesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var dict = await _db.GetAsync<System.Collections.Generic.Dictionary<string, Invoice>>("invoices");
                Invoices.Clear();
                if (dict != null)
                {
                    foreach (var kv in dict)
                    {
                        var c = kv.Value;
                        c.Id = kv.Key;
                        Invoices.Add(c);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddInvoiceAsync()
        {
            if (NewInvoice == null) return;
            NewInvoice.Validate();
            var key = await _db.PushAsync("invoices", NewInvoice);
            NewInvoice.Id = key;
            Invoices.Add(NewInvoice);
            NewInvoice = new Invoice();
        }

        [RelayCommand]
        public async Task UpdateInvoiceAsync()
        {
            if (SelectedInvoice == null) return;
            SelectedInvoice.Validate();
            await _db.SetAsync($"invoices/{SelectedInvoice.Id}", SelectedInvoice);
        }

        [RelayCommand]
        public async Task DeleteInvoiceAsync()
        {
            if (SelectedInvoice == null) return;
            await _db.DeleteAsync($"invoices/{SelectedInvoice.Id}");
            Invoices.Remove(SelectedInvoice);
            SelectedInvoice = null;
        }
    }
}