using System.Windows.Media;

namespace RainmeterStudio.Model
{
    public class DocumentFormat
    {
        public string Name { get; set; }
        public ImageSource Icon { get; set; }
        public string Description { get; set; }
        public string DefaultExtension { get; set; }
        public IDocumentEditorFactory Factory { get; set; }
        public string Category { get; set; }
    }
}
