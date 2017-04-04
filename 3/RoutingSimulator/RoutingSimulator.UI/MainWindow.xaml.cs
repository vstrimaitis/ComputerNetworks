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
        enum Mode
        {
            Default,
            Delete
        }

        private const double MovingOpacity = 0.5;
        private const double StationaryOpacity = 1.0;
        private const double CircleRadius = 15;
        private readonly Brush CircleFill = new SolidColorBrush(Colors.White);
        private readonly Brush CircleStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush CircleLabelColor = new SolidColorBrush(Colors.Black);
        private Shapes.Line _tempLine = null;
        private Mode _currentMode = Mode.Default;

        public MainWindow()
        {
            InitializeMouseEventHandlers();
            InitializeComponent();
            EnableCanvasMouseEvents();
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.LeftCtrl)
                {
                    _currentMode = Mode.Delete;
                }
            };
            this.KeyUp += (s, e) =>
            {
                _currentMode = Mode.Default;
            };
        }

        #region Helper Methods
        private Circle CreateCircle(bool isTransaprent = true)
        {
            var c = new Circle(CircleRadius, fill: CircleFill.Clone(),
                                                   stroke: CircleStroke.Clone(),
                                                   labelColor: CircleLabelColor.Clone(),
                                                   opacity: isTransaprent ? MovingOpacity : StationaryOpacity);
            c.MouseRightButtonUp += OnCircleMouseRightButtonUp;
            c.MouseRightButtonDown += OnCircleMouseRightButtonDown;
            c.MouseLeftButtonDown += OnCircleMouseLeftButtonDown;
            c.MouseLeftButtonUp += OnCircleMouseLeftButtonUp;
            c.MouseEnter += OnCircleMouseEnter;
            c.MouseLeave += OnCircleMouseLeave;
            c.MouseMove += OnCircleMouseMove;
            return c;
        }
        #endregion

        #region CircleMouseEventHandlers
        private EventHandler<MouseEventArgs> OnCircleMouseRightButtonUp;
        private EventHandler<MouseEventArgs> OnCircleMouseRightButtonDown;
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonDown;
        private EventHandler<MouseEventArgs> OnCircleMouseLeftButtonUp;
        private EventHandler<MouseEventArgs> OnCircleMouseEnter;
        private EventHandler<MouseEventArgs> OnCircleMouseLeave;
        private EventHandler<MouseEventArgs> OnCircleMouseMove;
        private MouseEventHandler OnEmptyCircleMouseMove;
        private MouseButtonEventHandler OnEmptyCircleMouseRightButtonUp;
        private void InitializeMouseEventHandlers()
        {
            OnCircleMouseLeftButtonDown = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    c.Opacity = MovingOpacity;
                    Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault() as UIElement);
                }
                else
                {
                    var c = s as Circle;
                    canvas.Children.Remove(c);
                    c.Dispose();
                }
            };
            OnCircleMouseLeftButtonUp = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    Mouse.Capture(null);
                    (s as Circle).Opacity = StationaryOpacity;
                }
            };
            OnCircleMouseEnter = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    this.Cursor = Cursors.Hand;
                    DisableCanvasMouseEvents();

                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                    }
                }
                else
                {
                    this.Cursor = Cursors.No;
                }
            };
            OnCircleMouseLeave = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    this.Cursor = Cursors.Arrow;
                    EnableCanvasMouseEvents();
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }
            };
            OnCircleMouseMove = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    if(Mouse.Captured == c.UIElements.Where(x => x is Ellipse).FirstOrDefault())
                    {
                        c.PositionCenter = args.GetPosition(canvas);
                    }
                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                        args.Handled = true;
                    }
                }
            };

            OnEmptyCircleMouseMove += (sender, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    if (_tempLine == null)
                        return;
                    if (args.RightButton == MouseButtonState.Pressed)
                        _tempLine.End.PositionCenter = args.GetPosition(canvas);
                    else
                        _tempLine = null;
                }
            };

            OnCircleMouseRightButtonDown = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var start = s as Circle;
                    _tempLine = new Shapes.Line(start,
                                                Circle.Empty,
                                                new SolidColorBrush(Colors.Black),
                                                new SolidColorBrush(Colors.Black),
                                                MovingOpacity);
                    _tempLine.Disposing += (sender, e) =>
                    {
                        canvas.Children.Remove(sender as Shapes.Line);
                    };
                    _tempLine.DragStart += (sender, e) =>
                    {
                        (sender as Shapes.Line).Opacity = MovingOpacity;
                    };
                    _tempLine.DragEnd += (sender, e) =>
                    {
                        (sender as Shapes.Line).Opacity = StationaryOpacity;
                    };
                    canvas.Children.Add(_tempLine);
                }
            };

            OnCircleMouseRightButtonUp = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    if (_tempLine == null)
                        return;
                    var c = s as Circle;
                    if (_tempLine.Start == c)
                        return;
                    var window = new IntegerInputWindow(this);
                    window.InputEntered += (sender, e) =>
                    {
                        _tempLine.LabelText = e.ToString();
                        _tempLine.End = c;
                        _tempLine.Opacity = StationaryOpacity;
                        _tempLine = null;
                    };
                    window.ShowDialog();
                }
            };

            OnEmptyCircleMouseRightButtonUp = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    if (_tempLine != null)
                        canvas.Children.Remove(_tempLine);
                    _tempLine = null;
                }
            };

        }

        private void DisableCanvasMouseEvents()
        {
            canvas.MouseMove -= OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp -= OnEmptyCircleMouseRightButtonUp;
        }

        private void EnableCanvasMouseEvents()
        {
            canvas.MouseMove += OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp += OnEmptyCircleMouseRightButtonUp;
        }


        #endregion

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(_currentMode == Mode.Default)
            {
                var pos = e.GetPosition(canvas);
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (Mouse.Captured == null)
                    {
                        var c = CreateCircle(true);
                        canvas.Children.Add(c);
                        Mouse.Capture(c.UIElements.Where(x => x is Ellipse).FirstOrDefault());
                    }
                }
            }
        }
    }
}
