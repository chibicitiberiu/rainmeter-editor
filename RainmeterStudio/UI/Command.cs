using System;
using System.Windows.Input;
using System.Windows.Media;
using RainmeterStudio.Business;
using RainmeterStudio.Core.Utils;

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

        /// <summary>
        /// Gets or sets the name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the display text of the command
        /// </summary>
        public string DisplayText
        {
            get
            {
                return ResourceProvider.GetString("Command_" + Name + "_DisplayText");
            }
        }

        /// <summary>
        /// Gets or sets the tooltip
        /// </summary>
        public string ToolTip
        {
            get
            {
                return ResourceProvider.GetString("Command_" + Name + "_ToolTip");
            }
        }

        /// <summary>
        /// Gets or sets the command's icon
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                return ResourceProvider.GetImage("Command_" + Name + "_Icon");
            }
        }

        /// <summary>
        /// Gets or sets the keyboard shortcut of this command
        /// </summary>
        public KeyGesture Shortcut
        {
            get
            {
                string str = SettingsProvider.GetSetting<string>("Command_" + Name + "_Shortcut");
                return InputHelper.GetKeyGesture(str);
            }
        }

        /// <summary>
        /// Gets the text representation of the keyboard shortcut
        /// </summary>
        public string ShortcutText
        {
            get
            {
                return SettingsProvider.GetSetting<string>("Command_" + Name + "_Shortcut");
            }
        }

        #endregion

        /// <summary>
        /// Event triggered when the command execution status changes
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Triggers the can execute changed event
        /// </summary>
        public void NotifyCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        /// <summary>
        /// Initializes this command
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="execute">Callback function to execute when the command is triggered</param>
        /// <param name="canExecute">Function that can be queried if the command can execute</param>
        public Command(string name, Action<object> execute, Func<object, bool> canExecute = null)
        {
            Name = name;
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Initializes this command
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="execute">Callback function to execute when the command is triggered</param>
        /// <param name="canExecute">Function that can be queried if the command can execute</param>
        public Command(string name, Action execute, Func<bool> canExecute = null)
        {
            Name = name;
            _executeNoParam = execute;
            _canExecuteNoParam = canExecute;
        }

        /// <summary>
        /// Function that can be queried if the command can be executed
        /// </summary>
        /// <param name="parameter">Command parameter</param>
        /// <returns>True if the function can be executed</returns>
        public virtual bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute(parameter);
            else if (_canExecuteNoParam != null)
                return _canExecuteNoParam();

            return true;
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="parameter">Command parameter</param>
        public virtual void Execute(object parameter)
        {
            if (_execute != null)
                _execute(parameter);
            else if (_executeNoParam != null)
                _executeNoParam();
        }
    }

    public static partial class UIElementExtensions
    {
        /// <summary>
        /// Adds a keyboard shortcut to an UI element
        /// </summary>
        /// <param name="uiElement">UI element</param>
        /// <param name="command">Command</param>
        public static void AddKeyBinding(this System.Windows.UIElement uiElement, Command command)
        {
            if (command.Shortcut != null)
                uiElement.InputBindings.Add(new KeyBinding(command, command.Shortcut));
        }
    }
}
