using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace RainmeterStudio.UI
{
    public class Command : ICommand
    {
        #region Private members

        private Action<object> _execute;
        private Func<object, bool> _canExecute;
        private Action _executeNoParam;
        private Func<bool> _canExecuteNoParam;
        
        #endregion

        #region Public properties

        public string Name { get; set; }
        public string DisplayText { get; set; }
        public string Tooltip { get; set; }
        public ImageSource Icon { get; set; }
        public KeyGesture Shortcut { get; set; }

        public string ShortcutText
        {
            get
            {
                string text = String.Empty;

                if ((Shortcut.Modifiers & ModifierKeys.Windows) != 0)
                    text += "Win+";

                if ((Shortcut.Modifiers & ModifierKeys.Control) != 0)
                    text += "Ctrl+";

                if ((Shortcut.Modifiers & ModifierKeys.Alt) != 0)
                    text += "Alt+";

                if ((Shortcut.Modifiers & ModifierKeys.Shift) != 0)
                    text += "Shift+";

                text += Enum.GetName(typeof(Key), Shortcut.Key);
                return text;
            }
        }

        #endregion

        public event EventHandler CanExecuteChanged;

        public Command(string name = null, Action<object> execute = null, Func<object, bool> canExecute = null)
        {
            Name = name;
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(string name = null, Action execute = null, Func<bool> canExecute = null)
        {
            Name = name;
            _executeNoParam = execute;
            _canExecuteNoParam = canExecute;
        }

        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            else if (_canExecuteNoParam != null)
                return _canExecuteNoParam();

            return true;
        }

        public virtual void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
            else if (_executeNoParam != null)
                _executeNoParam();
        }
    }
}
