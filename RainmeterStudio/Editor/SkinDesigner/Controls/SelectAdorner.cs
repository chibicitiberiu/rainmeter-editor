using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace RainmeterStudio.Editor.SkinDesigner.Controls
{
    public class SelectAdorner : Adorner
    {
        public SelectAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            // Calculate DPI factor
            Matrix matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            double dpiFactor = 1.0 / matrix.M11;

            // Rectangle
            Rect rect = new Rect(AdornedElement.DesiredSize);
            Rect selectionRect = new Rect(rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Height + 2);

            // Create pen
            Pen pen = new Pen(Brushes.Blue, 1 * dpiFactor);

            // Create a guidelines set for on-pixel drawing
            GuidelineSet guidelines = new GuidelineSet();
            guidelines.GuidelinesX.Add(selectionRect.Left + pen.Thickness / 2);
            guidelines.GuidelinesX.Add(selectionRect.Right + pen.Thickness / 2);
            guidelines.GuidelinesY.Add(selectionRect.Top + pen.Thickness / 2);
            guidelines.GuidelinesY.Add(selectionRect.Bottom + pen.Thickness / 2);

            // Draw
            drawingContext.PushGuidelineSet(guidelines);
            drawingContext.DrawRectangle(null, pen, selectionRect);
            drawingContext.Pop();
        }
    }
}
