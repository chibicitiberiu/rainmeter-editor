using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using RainmeterStudio.Core.Model;
using RainmeterStudio.Core.Utils;
using Version = RainmeterStudio.Core.Utils.Version;

namespace RainmeterStudio.Rainmeter
{
    /// <summary>
    /// Skin background modes
    /// </summary>
    [Flags]
    public enum SkinBackgroundMode
    {
        /// <summary>
        /// Image as defined by Background
        /// </summary>
        Image = 0,

        /// <summary>
        /// Transparent background
        /// </summary>
        Transparent = 1,

        /// <summary>
        /// Fill with a solid color
        /// </summary>
        SolidColor = 2,

        /// <summary>
        /// Fill with gradient
        /// </summary>
        Gradient = 2 + 0x80,

        /// <summary>
        /// Fill by scaling image as defined by Background
        /// </summary>
        ScaledImage = 3,

        /// <summary>
        /// Fill by tiling image as defined by Background
        /// </summary>
        TiledImage = 4,

        /// <summary>
        /// Mask used when writing to INI file
        /// </summary>
        Mask = 0xf
    }

    /// <summary>
    /// Skin bevel types
    /// </summary>
    public enum SkinBevelType
    {
        None = 0,
        Raised,
        Sunken
    }

    /// <summary>
    /// Context menu item
    /// </summary>
    public struct ContextItem
    {
        public string Title { get; set; }
        public DataTypes.Action Action { get; set; }
    }

    public enum BlurRegionType
    {
        Disabled,
        Rectangular,
        RectangularWithRoundedCorners,
        Elliptical
    }

    public struct BlurRegion
    {
        BlurRegionType Type { get; set; }
        Int32Rect Region { get; set; }
        int Radius { get; set; }
    }

    /// <summary>
    /// Rainmeter skin
    /// </summary>
    public class Skin : INotifyPropertyChanged
    {
        #region Private fields

        // Metadata
        string _name, _author, _information, _license;
        Version _version;

        // General options
        int _update, _transitionUpdate;
        bool _accurateText, _dynamicWindowSize, _tooltipHidden;
        Int32Rect _dragMargins;
        DataTypes.Action _onRefreshAction, _onUpdateAction, _onCloseAction, _onFocusAction, _onUnfocusAction, _onWakeAction;

        // Background
        Reference _background;
        SkinBackgroundMode _backgroundMode;
        Int32Rect _backgroundMargins;
        Color _solidColor, _gradientColor1, _gradientColor2;
        double _gradientAngle;
        SkinBevelType _bevelType;

        // Aero blur
        bool _blur;

        #endregion

        /// <summary>
        /// Triggered when the value of a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties: Metadata

        /// <summary>
        /// Gets or sets the skin's name
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        /// <summary>
        /// Gets or sets the skin's author
        /// </summary>
        public string Author
        {
            get
            {
                return _author;
            }
            set
            {
                _author = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Author"));
            }
        }

        /// <summary>
        /// Gets or sets additional information about this skin
        /// </summary>
        public string Information
        {
            get
            {
                return _information;
            }
            set
            {
                _information = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Information"));
            }
        }

        /// <summary>
        /// Gets or sets licensing information about the skin
        /// </summary>
        public string License
        {
            get
            {
                return _license;
            }
            set
            {
                _license = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("License"));
            }
        }

        /// <summary>
        /// Gets or sets licensing information about the skin
        /// </summary>
        public Version Version
        {
            get
            {
                return _version;
            }
            set
            {
                _version = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Version"));
            }
        }

        #endregion

        #region Properties: Content (measures and meters)

        /// <summary>
        /// List of measures
        /// </summary>
        public ObservableCollection<Measure> Measures { get; private set; }

        /// <summary>
        /// List of meters
        /// </summary>
        public ObservableCollection<Meter> Meters { get; private set; }

        #endregion

        #region Properties: General options

        /// <summary>
        /// Gets or sets the skin's update rate
        /// </summary>
        public int Update
        {
            get
            {
                return _update;
            }
            set
            {
                _update = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Update"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if String meters are measured and rendered using improved 
        /// padding and character spacing similar to that provided by DiInt32Rect2D.
        /// </summary>
        public bool AccurateText
        {
            get
            {
                return _accurateText;
            }
            set
            {
                _accurateText = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("AccurateText"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the window size is adjusted on 
        /// each update to fit the meters
        /// </summary>
        public bool DynamicWindowSize
        {
            get
            {
                return _dynamicWindowSize;
            }
            set
            {
                _dynamicWindowSize = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DynamicWindowSize"));
            }
        }

        /// <summary>
        /// Gets or sets the area from where the window can be dragged.
        /// </summary>
        public Int32Rect DragMargins
        {
            get
            {
                return _dragMargins;
            }
            set
            {
                _dragMargins = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("DragMargins"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed when the skin is loaded or refreshed.
        /// </summary>
        public DataTypes.Action OnRefreshAction
        {
            get
            {
                return _onRefreshAction;
            }
            set
            {
                _onRefreshAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnRefreshAction"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed on each Update of the skin.
        /// </summary>
        public DataTypes.Action OnUpdateAction
        {
            get
            {
                return _onUpdateAction;
            }
            set
            {
                _onUpdateAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnUpdateAction"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed when the skin is unloaded or when Rainmeter is closed.
        /// </summary>
        public DataTypes.Action OnCloseAction
        {
            get
            {
                return _onCloseAction;
            }
            set
            {
                _onCloseAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnCloseAction"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed when the skin recieves focus in Windows (set by clicking the mouse on the skin).
        /// </summary>
        public DataTypes.Action OnFocusAction
        {
            get
            {
                return _onFocusAction;
            }
            set
            {
                _onFocusAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnFocusAction"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed when the skin loses focus in Windows.
        /// </summary>
        public DataTypes.Action OnUnfocusAction
        {
            get
            {
                return _onUnfocusAction;
            }
            set
            {
                _onUnfocusAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnUnfocusAction"));
            }
        }

        /// <summary>
        /// Gets or sets the action that will be executed when Windows returns from the sleep or hibernate states.
        /// </summary>
        public DataTypes.Action OnWakeAction
        {
            get
            {
                return _onWakeAction;
            }
            set
            {
                _onWakeAction = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("OnWakeAction"));
            }
        }

        /// <summary>
        /// Gets or sets the update time in milliseconds for meter transitions.
        /// </summary>
        public int TransitionUpdate
        {
            get
            {
                return _transitionUpdate;
            }
            set
            {
                _transitionUpdate = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TransitionUpdate"));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if all tooltips in the skin will be hidden.
        /// </summary>
        public bool TooltipHidden
        {
            get
            {
                return _tooltipHidden;
            }
            set
            {
                _tooltipHidden = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TooltipHidden"));
            }
        }

        /// <summary>
        /// Gets or sets the groups that the skin belongs to.
        /// </summary>
        public ObservableCollection<string> Groups { get; private set; }

        #endregion

        #region Properties: Background
        
        /// <summary>
        /// Gets or sets the background image.
        /// </summary>
        public Reference Background
        {
            get
            {
                return _background;
            }
            set
            {
                _background = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Background"));
            }
        }

        /// <summary>
        /// Gets or sets the background mode for the skin.
        /// </summary>
        public SkinBackgroundMode BackgroundMode
        {
            get
            {
                return _backgroundMode;
            }
            set
            {
                _backgroundMode = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BackgroundMode"));
            }
        }

        /// <summary>
        /// Gets or sets the margins of the Background image that are not scaled.
        /// </summary>
        public Int32Rect BackgroundMargins
        {
            get
            {
                return _backgroundMargins;
            }
            set
            {
                _backgroundMargins = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BackgroundMargins"));
            }
        }

        /// <summary>
        /// Gets or sets the solid background color.
        /// </summary>
        public Color SolidColor
        {
            get
            {
                return _solidColor;
            }
            set
            {
                _solidColor = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SolidColor"));
            }
        }

        /// <summary>
        /// Gets or sets the the first color of the gradient.
        /// </summary>
        public Color GradientColor1
        {
            get
            {
                return _gradientColor1;
            }
            set
            {
                _gradientColor1 = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GradientColor1"));
            }
        }

        /// <summary>
        /// Gets or sets the second color of the gradient.
        /// </summary>
        public Color GradientColor2
        {
            get
            {
                return _gradientColor2;
            }
            set
            {
                _gradientColor2 = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GradientColor2"));
            }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient in degrees.
        /// </summary>
        public double GradientAngle
        {
            get
            {
                return _gradientAngle;
            }
            set
            {
                _gradientAngle = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("GradientAngle"));
            }
        }

        /// <summary>
        /// Gets or sets the bevel type.
        /// </summary>
        /// <remarks>
        /// If enabled, draws a bevel around the edges of the entire skin when BackgroundMode=2.
        /// </remarks>
        public SkinBevelType BevelType
        {
            get
            {
                return _bevelType;
            }
            set
            {
                _bevelType = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("BevelType"));
            }
        }

        #endregion

        #region Properties: Context items

        /// <summary>
        /// Gets the context items
        /// </summary>
        public ObservableCollection<ContextItem> ContextItems { get; private set; }

        #endregion

        #region Properties: aero blur options

        /// <summary>
        /// Gets or sets a value indicating if Aero Blur is enabled.
        /// </summary>
        public bool Blur
        {
            get
            {
                return _blur;
            }
            set
            {
                _blur = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Blur"));
            }
        }

        /// <summary>
        /// Gets the blur regions
        /// </summary>
        public ObservableCollection<BlurRegion> BlurRegions { get; private set; }

        #endregion

        #region Variables

        /// <summary>
        /// Gets the list of variables
        /// </summary>
        public ObservableDictionary<string, object> Variables { get; private set; }

        #endregion

        public Skin()
        {
            Measures = new ObservableCollection<Measure>();
            Meters = new ObservableCollection<Meter>();
            Groups = new ObservableCollection<string>();
            ContextItems = new ObservableCollection<ContextItem>();
            BlurRegions = new ObservableCollection<BlurRegion>();
            Variables = new ObservableDictionary<string, object>();

            // Set defaults
            Update = 1000;
            TransitionUpdate = 100;
            BackgroundMode = SkinBackgroundMode.Transparent;
        }
    }
}
