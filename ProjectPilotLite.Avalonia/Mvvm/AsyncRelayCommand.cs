using System.Windows.Input;

namespace ProjectPilotLite.Avalonia.Mvvm;

public class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isRunning;

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? p) => !_isRunning && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? p)
    {
        if (!CanExecute(p)) return;
        _isRunning = true;
        RaiseCanExecuteChanged();
        try { await _execute(); }
        finally { _isRunning = false; RaiseCanExecuteChanged(); }
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
