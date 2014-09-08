using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RainmeterStudio.TextEditorPlugin
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        private TextDocument _document;

        public TextEditorControl(TextDocument document)
        {
            InitializeComponent();
            DataContext = this;

            _document = document;
            textBox.Text = _document.Text;
            textBox.TextChanged += text_TextChanged;
        }

        private void text_TextChanged(object sender, TextChangedEventArgs e)
        {
            _document.IsDirty = true;
            _document.Text = textBox.Text;
        }
    }
}
