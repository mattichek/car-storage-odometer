using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace car_storage_odometer
{
    using car_storage_odometer.Helpers;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Services.Dialogs;
    using System;

    public class CustomMessageBoxViewModel : BindableBase, IDialogAware
    {
        public event Action<IDialogResult> RequestClose;

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _showOkButton;
        public bool ShowOkButton
        {
            get => _showOkButton;
            set => SetProperty(ref _showOkButton, value);
        }

        private bool _showYesNoButtons;
        public bool ShowYesNoButtons
        {
            get => _showYesNoButtons;
            set => SetProperty(ref _showYesNoButtons, value);
        }

        

        public DelegateCommand OkCommand { get; }
        public DelegateCommand YesCommand { get; }
        public DelegateCommand NoCommand { get; }

        public CustomMessageBoxViewModel()
        {
            OkCommand = new DelegateCommand(() => CloseDialog(ButtonResult.OK));
            YesCommand = new DelegateCommand(() => CloseDialog(ButtonResult.Yes));
            NoCommand = new DelegateCommand(() => CloseDialog(ButtonResult.No));
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("message"))
                Message = parameters.GetValue<string>("message");

            if (parameters.ContainsKey("title"))
                Title = parameters.GetValue<string>("title");

            var buttons = parameters.GetValue<CustomMessageBoxButtons?>("buttons") ?? CustomMessageBoxButtons.YesNo;

            ShowOkButton = buttons == CustomMessageBoxButtons.Ok;
            ShowYesNoButtons = buttons == CustomMessageBoxButtons.YesNo;
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        private void CloseDialog(ButtonResult result)
        {
            RequestClose?.Invoke(new DialogResult(result));
        }
    }
}