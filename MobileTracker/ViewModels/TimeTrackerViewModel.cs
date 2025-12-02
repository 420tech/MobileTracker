using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MobileTracker.Models;
using MobileTracker.Services;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MobileTracker.ViewModels
{
    public partial class TimeTrackerViewModel : ObservableObject
    {
        private readonly IFirebaseDatabaseService _db;
        private readonly ILogger<TimeTrackerViewModel> _logger;

        [ObservableProperty]
        ObservableCollection<TimeEntry> timeEntries = new ObservableCollection<TimeEntry>();

        [ObservableProperty]
        TimeEntry? selectedEntry;

        [ObservableProperty]
        TimeEntry? newEntry = new TimeEntry { StartTime = DateTime.UtcNow };

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

        public TimeTrackerViewModel(IFirebaseDatabaseService db, ILogger<TimeTrackerViewModel>? logger = null)
        {
            _db = db;
            _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<TimeTrackerViewModel>.Instance;
        }

        [RelayCommand]
        public async Task LoadEntriesAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                var dict = await _db.GetAsync<System.Collections.Generic.Dictionary<string, TimeEntry>>("timeEntries");
                TimeEntries.Clear();
                if (dict != null)
                {
                    foreach (var kv in dict)
                    {
                        var c = kv.Value;
                        c.Id = kv.Key;
                        TimeEntries.Add(c);
                    }
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddEntryAsync()
        {
            HasError = false;
            HasSuccessMessage = false;
            if (NewEntry == null) return;
            try
            {
                NewEntry.Validate();
                var key = await _db.PushAsync("timeEntries", NewEntry);
                NewEntry.Id = key;
                TimeEntries.Add(NewEntry);
                SuccessMessage = "Time entry added";
                HasSuccessMessage = true;
                NewEntry = new TimeEntry { StartTime = DateTime.UtcNow };
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        [RelayCommand]
        public async Task UpdateEntryAsync()
        {
            HasError = false;
            HasSuccessMessage = false;
            if (SelectedEntry == null) return;
            try
            {
                SelectedEntry.Validate();
                await _db.SetAsync($"timeEntries/{SelectedEntry.Id}", SelectedEntry);
                SuccessMessage = "Time entry updated";
                HasSuccessMessage = true;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }

        [RelayCommand]
        public async Task DeleteEntryAsync()
        {
            HasError = false;
            HasSuccessMessage = false;
            if (SelectedEntry == null) return;
            try
            {
                await _db.DeleteAsync($"timeEntries/{SelectedEntry.Id}");
                TimeEntries.Remove(SelectedEntry);
                SelectedEntry = null;
                SuccessMessage = "Time entry deleted";
                HasSuccessMessage = true;
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
                HasError = true;
            }
        }
    }
}