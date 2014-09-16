using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RainmeterStudio.Resources;

namespace RainmeterStudio.UI.Dialogs
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        private Func<string, bool> _validateFunction;

        /// <summary>
        /// A validate function that always returns true
        /// </summary>
        public static readonly Func<string, bool> AlwaysValid = (str => true);

        #region Properties

        /// <summary>
        /// Gets or sets the prompt text
        /// </summary>
        public string Prompt
        {
            get
            {
                return textPrompt.Text;
            }
            set
            {
                textPrompt.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the text on the 'ok' button
        /// </summary>
        public string OKCaption
        {
            get
            {
                return (string)buttonOK.Content;
            }
            set
            {
                buttonOK.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the text on the 'cancel' button
        /// </summary>
        public string CancelCaption
        {
            get
            {
                return (string)buttonCancel.Content;
            }
            set
            {
                buttonCancel.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the text inputted by the user
        /// </summary>
        public string InputText
        {
            get
            {
                return textInput.Text;
            }
            set
            {
                textInput.Text = value;
                Validate();
            }
        }

        /// <summary>
        /// Gets or sets a function used to validate the input
        /// </summary>
        public Func<string, bool> ValidateFunction
        {
            get
            {
                return _validateFunction;
            }
            set
            {
                _validateFunction = value;
                Validate();
            }
        }

        #endregion

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        public InputDialog(string prompt)
            :this(prompt, String.Empty, String.Empty, AlwaysValid)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        public InputDialog(string prompt, string caption)
            : this(prompt, caption, String.Empty, AlwaysValid)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        public InputDialog(string prompt, string caption, string okCaption, string cancelCaption)
            : this(prompt, caption, String.Empty, AlwaysValid, okCaption, cancelCaption)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        public InputDialog(string prompt, string caption, Func<string, bool> validateFunction)
            : this(prompt, caption, String.Empty, validateFunction)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        public InputDialog(string prompt, string caption, Func<string, bool> validateFunction, string okCaption, string cancelCaption)
            : this(prompt, caption, String.Empty, validateFunction, okCaption, cancelCaption)
        {
        }


        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        public InputDialog(string prompt, string caption, string initialValue)
            : this(prompt, caption, initialValue, AlwaysValid)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        public InputDialog(string prompt, string caption, string initialValue, string okCaption, string cancelCaption)
            : this(prompt, caption, initialValue, AlwaysValid, okCaption, cancelCaption)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        public InputDialog(string prompt, string caption, string initialValue, Func<string, bool> validateFunction)
            : this(prompt, caption, initialValue, validateFunction, Strings.Dialog_OK, Strings.Dialog_Cancel)
        {
        }

        /// <summary>
        /// Initializes the input dialog
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        public InputDialog(string prompt, string caption, string initialValue, Func<string, bool> validateFunction, string okCaption, string cancelCaption)
        {
            InitializeComponent();

            Prompt = prompt;
            Title = caption;
            InputText = initialValue;
            ValidateFunction = validateFunction;
            OKCaption = okCaption;
            CancelCaption = cancelCaption;
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void textInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            Validate();
        }

        private void Validate()
        {
            bool res = true;

            if (ValidateFunction != null)
                res = ValidateFunction(textInput.Text);

            buttonOK.IsEnabled = res;
        }

        #region Static show

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt)
        {
            return ShowCommon(new InputDialog(prompt));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption)
        {
            return ShowCommon(new InputDialog(prompt, caption));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, string okCaption, string cancelCaption)
        {
            return ShowCommon(new InputDialog(prompt, caption, okCaption, cancelCaption));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, Func<string, bool> validateFunction)
        {
            return ShowCommon(new InputDialog(prompt, caption, validateFunction));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, Func<string, bool> validateFunction, string okCaption, string cancelCaption)
        {
            return ShowCommon(new InputDialog(prompt, caption, validateFunction, okCaption, cancelCaption));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, string initialValue)
        {
            return ShowCommon(new InputDialog(prompt, caption, initialValue));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, string initialValue, string okCaption, string cancelCaption)
        {
            return ShowCommon(new InputDialog(prompt, caption, initialValue, okCaption, cancelCaption));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, string initialValue, Func<string, bool> validateFunction)
        {
            return ShowCommon(new InputDialog(prompt, caption, initialValue, validateFunction));
        }

        /// <summary>
        /// Shows a dialog prompting the user for input
        /// </summary>
        /// <param name="prompt">Message displayed to user</param>
        /// <param name="caption">Title of dialog</param>
        /// <param name="initialValue">Initial value of the input dialog</param>
        /// <param name="validateFunction">Callback function which validates the inputted text</param>
        /// <param name="okCaption">Caption of the 'OK' button</param>
        /// <param name="cancelCaption">Caption of the 'Cancel' button</param>
        /// <returns>Input text, or null if canceled</returns>
        public static string Show(string prompt, string caption, string initialValue, Func<string, bool> validateFunction, string okCaption, string cancelCaption)
        {
            return ShowCommon(new InputDialog(prompt, caption, initialValue, validateFunction, okCaption, cancelCaption));
        }

        private static string ShowCommon(InputDialog dialog)
        {
            bool? res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                return dialog.InputText;
            }

            return null;
        }

        #endregion
    }
}
