using RoutingSimulator.UI.Shapes;
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

namespace RoutingSimulator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double CircleMovingOpacity = 0.5;
        private const double CircleStationaryOpacity = 1.0;
        private const double CircleRadius = 15;
        private readonly Brush CircleFill = new SolidColorBrush(Colors.White);
        private readonly Brush CircleStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush CircleLabelColor = new SolidColorBrush(Colors.Black);
        private Circle _previousCircle;
        private Circle _selectedCircle;

        public MainWindow()
        {
            InitializeMouseEventHandlers();
            InitializeComponent();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(Mouse.Captured == null)
                {
                    var c = CreateCircle(true);
                    canvas.Children.Add(c);
                    Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault());
                }
            }
        }

        #region Helper Methods
        private Circle CreateCircle(bool isTransaprent = true)
        {
            var c = new Circle(CircleRadius, fill: CircleFill.Clone(),
                                                   stroke: CircleStroke.Clone(),
                                                   labelColor: CircleLabelColor.Clone(),
                                                   opacity: isTransaprent ? CircleMovingOpacity : CircleStationaryOpacity);
            c.MouseLeftButtonDown += OnCircleMouseLeftButtonDown;
            c.MouseLeftButtonUp += OnCircleMouseLeftButtonUp;
            c.MouseEnter += OnCircleMouseEnter;
            c.MouseLeave += OnCircleMouseLeave;
            c.MouseMove += OnCircleMouseMove;
            return c;
        }
        #endregion

        #region CircleMouseEventHandlers
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonDown;
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonUp;
        private EventHandler<MouseEventArgs> OnCircleMouseEnter;
        private EventHandler<MouseEventArgs> OnCircleMouseLeave;
        private EventHandler<MouseEventArgs> OnCircleMouseMove;
        private void InitializeMouseEventHandlers()
        {
            OnCircleMouseLeftButtonDown = (s, args) =>
            {
                var c = s as Circle;
                c.Opacity = CircleMovingOpacity;
                Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault() as UIElement);
            };
            OnCircleMouseLeftButtonUp = (s, args) =>
            {
                Mouse.Capture(null);
                (s as Circle).Opacity = CircleStationaryOpacity;
            };
            OnCircleMouseEnter = (s, args) =>
            {
                this.Cursor = Cursors.Hand;
                _selectedCircle = s as Circle;
            };
            OnCircleMouseLeave = (s, args) =>
            {
                this.Cursor = Cursors.Arrow;
                if (Mouse.Captured == null)
                    _selectedCircle = null;
            };
            OnCircleMouseMove = (s, args) =>
            {
                var c = s as Circle;
                if(Mouse.Captured == c.UIElements.Where(x => x is Ellipse).FirstOrDefault())
                {
                    c.PositionCenter = args.GetPosition(canvas);
                }
            };
        }
        #endregion
    }
}
