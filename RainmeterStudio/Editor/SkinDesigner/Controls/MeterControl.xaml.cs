using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RainmeterStudio.Rainmeter;

namespace RainmeterStudio.Editor.SkinDesigner.Controls
{
    /// <summary>
    /// Interaction logic for MeterControl.xaml
    /// </summary>
    public partial class MeterControl : UserControl
    {
        private Skin _skin;

        /// <summary>
        /// Gets or sets the skin being edited
        /// </summary>
        public Skin Skin
        {
            get
            {
                return Skin;
            }
            set
            {
                _skin = value;

                Reset();
            }
        }

        /// <summary>
        /// Gets an enumerable with the meter items
        /// </summary>
        public IEnumerable<Meter> Items { get { return Skin.Meters; } }

        /// <summary>
        /// Gets a collection with the currently selected items
        /// </summary>
        public ObservableCollection<Meter> SelectedItems { get; private set; }

        /// <summary>
        /// Gets or sets the currently selected item
        /// </summary>
        public Meter SelectedItem { get; set; }

        /// <summary>
        /// Gets or sets the selected item index
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Initializes the meter control
        /// </summary>
        public MeterControl()
        {
            InitializeComponent();
            Loaded += MeterControl_Loaded;
        }

        void MeterControl_Loaded(object sender, RoutedEventArgs e)
        {
            //AdornerLayer.GetAdornerLayer(ellipse).Add(new SelectAdorner(ellipse));
        }

        private void Reset()
        {
        }
    }
}
