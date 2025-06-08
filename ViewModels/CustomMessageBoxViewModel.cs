using car_storage_odometer.Helpers;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

public class CustomMessageBoxViewModel : BindableBase, IDialogAware
{
    public string Message { get; set; }
    public bool ShowYesNoButtons { get; set; }
    public bool ShowOkButton { get; set; }

    public DelegateCommand YesCommand { get; }
    public DelegateCommand NoCommand { get; }
    public DelegateCommand OkCommand { get; }

    public event Action<IDialogResult> RequestClose;

    public CustomMessageBoxViewModel()
    {
        YesCommand = new DelegateCommand(() => CloseDialog(ButtonResult.Yes));
        NoCommand = new DelegateCommand(() => CloseDialog(ButtonResult.No));
        OkCommand = new DelegateCommand(() => CloseDialog(ButtonResult.OK));
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Message = parameters.GetValue<string>("message");

        var buttons = parameters.GetValue<CustomMessageBoxButtons?>("buttons") ?? CustomMessageBoxButtons.YesNo;

        ShowYesNoButtons = buttons == CustomMessageBoxButtons.YesNo;
        ShowOkButton = buttons == CustomMessageBoxButtons.Ok;

        RaisePropertyChanged(nameof(Message));
        RaisePropertyChanged(nameof(ShowYesNoButtons));
        RaisePropertyChanged(nameof(ShowOkButton));
    }

    private void CloseDialog(ButtonResult result)
    {
        RequestClose?.Invoke(new DialogResult(result));
    }

    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public string Title => "Komunikat";
}
