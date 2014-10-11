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
using System.Windows.Navigation;
using System.Windows.Shapes;
using RainmeterStudio.Core.Editor.Features;
using RainmeterStudio.UI.ViewModel;

namespace RainmeterStudio.UI.Panels
{
    /// <summary>
    /// Interaction logic for ToolboxPanel.xaml
    /// </summary>
    public partial class ToolboxPanel : UserControl
    {
        private IToolboxProvider _itemsSource = null;

        /// <summary>
        /// Gets or sets the items source
        /// </summary>
        public IToolboxProvider ItemsSource
        {
            get
            {
                return _itemsSource;
            }
            set
            {
                // Unsubscribe from old items source
                if (_itemsSource != null)
                {
                    _itemsSource.ToolboxItemsChanged -= ItemsSource_ToolboxItemsChanged;
                }

                // Change items source
                _itemsSource = value;

                // Subscribe to new items source
                if (_itemsSource != null)
                {
                    value.ToolboxItemsChanged += ItemsSource_ToolboxItemsChanged;
                }

                // Refresh items
                RefreshItems();
            }
        }

        /// <summary>
        /// Initializes the toolbox panel
        /// </summary>
        public ToolboxPanel()
        {
            InitializeComponent();
        }

        private void RefreshItems()
        {
            if (_itemsSource == null)
            {
                listItems.ItemsSource = Enumerable.Empty<ToolboxItemViewModel>();
            }
            else
            {
                listItems.ItemsSource = _itemsSource.ToolboxItems.Select(item => new ToolboxItemViewModel(item));
            }
        }

        void ItemsSource_ToolboxItemsChanged(object sender, EventArgs e)
        {
            RefreshItems();
        }

        #region Adding to editor (drag and double clicking)

        void Item_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            var item = treeViewItem.Header as ToolboxItem;

            if (item != null && e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, item, DragDropEffects.Move);
            }
        }

        void Item_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            var item = treeViewItem.Header as ToolboxItem;

            if (item != null)
            {
                ItemsSource.ToolboxItemDrop(item);
            }
        }

        #endregion
    }
}
