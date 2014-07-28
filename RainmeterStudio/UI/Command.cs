using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using RainmeterStudio.UI.Controller;

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

        #region Display text property
        private string _displayText = null;

        /// <summary>
        /// Gets or sets the display text of the command
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_displayText == null)
                    return Resources.Strings.ResourceManager.GetString(Name + "_DisplayText");

                return _displayText;
            }
            set
            {
                _displayText = value;
            }
        }

        #endregion

        #region ToolTip property
        private string _toolTip = null;

        /// <summary>
        /// Gets or sets the tooltip
        /// </summary>
        public string ToolTip
        {
            get
            {
                if (_toolTip == null)
                    return Resources.Strings.ResourceManager.GetString(Name + "_ToolTip");

                return _toolTip;
            }
            set
            {
                _toolTip = value;
            }
        }
        #endregion

        #region Icon property
        private ImageSource _icon = null;

        /// <summary>
        /// Gets or sets the command's icon
        /// </summary>
        public ImageSource Icon
        {
            get
            {
                if (_icon == null)
                    return IconProvider.GetIcon(Name);

                return _icon;
            }
            set
            {
                _icon = value;
            }
        }
        #endregion

        #region Keyboard shortcut property

        private KeyGesture _shortcut;

        /// <summary>
        /// Gets or sets the keyboard shortcut of this command
        /// </summary>
        public KeyGesture Shortcut
        {
            get
            {
                if (_shortcut == null)
                {
                    string str = SettingsProvider.GetSetting<string>(Name + "_Shortcut");
                    return GetKeyGestureFromString(str);
                }

                return _shortcut;
            }
            set
            {
                _shortcut = value;
            }
        }

        /// <summary>
        /// Gets the text representation of the keyboard shortcut
        /// </summary>
        public string ShortcutText
        {
            get
            {
                // Safety check
                if (Shortcut == null)
                    return null;

                // Build string
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
            set
            {
                Shortcut = GetKeyGestureFromString(value);
            }
        }

        private KeyGesture GetKeyGestureFromString(string k)
        {
            // Safety check
            if (k == null)
                return null;

            // Variables
            ModifierKeys mods = ModifierKeys.None;
            Key key = Key.None;

            // Parse each field
            foreach (var field in k.Split('+'))
            {
                // Trim surrounding white space
                string trimmed = field.Trim();

                // Parse
                if (trimmed.Equals("Win", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Windows;
                if (trimmed.Equals("Ctrl", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Control;
                if (trimmed.Equals("Alt", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Alt;
                if (trimmed.Equals("Shift", StringComparison.InvariantCultureIgnoreCase))
                    mods |= ModifierKeys.Shift;
                else Enum.TryParse<Key>(field, out key);
            }

            return new KeyGesture(key, mods);
        }

        #endregion

        #endregion



        public event EventHandler CanExecuteChanged;

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

    public static class UIElementExtensions
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
