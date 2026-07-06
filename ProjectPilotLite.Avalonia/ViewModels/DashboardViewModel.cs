using System.Windows.Input;
using ProjectPilotLite.Avalonia.Mvvm;
using ProjectPilotLite.Avalonia.Services;
using ProjectPilotLite.Core.Dtos;

namespace ProjectPilotLite.Avalonia.ViewModels;

public class DashboardViewModel : ObservableObject
{
    private readonly IApiClient _api;
    private DashboardDto _stats = new();
    private bool _isBusy;
    private string? _errorMessage;

    public DashboardViewModel(IApiClient api)
    {
        _api = api;
        RefreshCommand = new AsyncRelayCommand(LoadAsync);
    }

    public DashboardDto Stats
    {
        get => _stats;
        private set => SetProperty(ref _stats, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetProperty(ref _isBusy, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        private set { if (SetProperty(ref _errorMessage, value)) OnPropertyChanged(nameof(HasError)); }
    }

    public bool HasError => !string.IsNullOrEmpty(_errorMessage);

    public ICommand RefreshCommand { get; }

    public async Task LoadAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        try
        {
            Stats = await _api.GetDashboardAsync();
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
