using RoutingSimulator.UI.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Delete,
            Send
        }

        private const double SendLineDashLength = 5;
        private const double MovingOpacity = 0.5;
        private const double StationaryOpacity = 1.0;
        private const double CircleRadius = 15;
        private readonly Brush CircleFill = new SolidColorBrush(Colors.White);
        private readonly Brush CircleStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush CircleLabelColor = new SolidColorBrush(Colors.Black);
        private readonly Brush CommunicatingCircleFill = new SolidColorBrush(Colors.Yellow);
        private Shapes.Line _tempLine = null;
        private Mode _currentMode = Mode.Default;
        private Dictionary<Mode, Cursor> _modeCursorMap = new Dictionary<Mode, Cursor>()
        {
            {Mode.Default, Cursors.Arrow },
            {Mode.Delete, Cursors.Arrow },
            {Mode.Send, Cursors.Cross} 
        };

        public MainWindow()
        {
            InitializeMouseEventHandlers();
            InitializeComponent();
            EnableCanvasMouseEvents();
        }

        #region Helper Methods
        private void ResetCircleStyle(Circle c)
        {
            c.Fill = new SolidColorBrush(Colors.White);
            c.Stroke = new SolidColorBrush(Colors.Black);
            c.LabelColor = new SolidColorBrush(Colors.Black);
            c.Opacity = StationaryOpacity;
        }
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
        private MouseButtonEventHandler OnEmptyCircleMouseLeftButtonUp;
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
                else if(_currentMode == Mode.Delete)
                {
                    var c = s as Circle;
                    canvas.Children.Remove(c);
                    c.Dispose();
                }
                else if(_currentMode == Mode.Send)
                {
                    var start = s as Circle;
                    start.Fill = CommunicatingCircleFill.Clone();
                    _tempLine = new Shapes.Line(start,
                                            Circle.Empty,
                                            new SolidColorBrush(Colors.Red),
                                            new SolidColorBrush(Colors.Red),
                                            MovingOpacity, SendLineDashLength);
                    canvas.Children.Add(_tempLine);
                }
            };
            OnCircleMouseLeftButtonUp = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    Mouse.Capture(null);
                    (s as Circle).Opacity = StationaryOpacity;
                }
                else if(_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if (_tempLine == null || _tempLine.Start == c)
                        return;
                    ResetCircleStyle(_tempLine.Start);
                    ResetCircleStyle(c);
                    // send a packet from _tempLine.Start to _tempLine.End
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
                else if (_currentMode == Mode.Delete)
                {
                    this.Cursor = Cursors.No;
                }
                else if (_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if(_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = c.PositionCenter;
                        if (c != _tempLine.Start)
                            c.Fill = CommunicatingCircleFill.Clone();
                    }
                }
            };
            OnCircleMouseLeave = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    this.Cursor = Cursors.Arrow;
                    EnableCanvasMouseEvents();
                }
                else if (_currentMode == Mode.Delete)
                {
                    this.Cursor = Cursors.Arrow;
                }
                else if(_currentMode == Mode.Send)
                {
                    var c = s as Circle;
                    if (_tempLine != null && _tempLine.Start != c)
                        ResetCircleStyle(c);
                }
            };
            OnCircleMouseMove = (s, args) =>
            {
                if(_currentMode == Mode.Default)
                {
                    var c = s as Circle;
                    var pos = args.GetPosition(canvas);
                    if (pos.X < CircleRadius)
                        pos.X = CircleRadius;
                    if (pos.X >= canvas.ActualWidth - CircleRadius)
                        pos.X = canvas.ActualWidth - CircleRadius;
                    if (pos.Y < CircleRadius)
                        pos.Y = CircleRadius;
                    if (pos.Y >= canvas.ActualHeight - CircleRadius)
                        pos.Y = canvas.ActualHeight - CircleRadius;
                    if(Mouse.Captured == c.UIElements.Where(x => x is Ellipse).FirstOrDefault())
                    {
                        c.PositionCenter = pos;
                    }
                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                        args.Handled = true;
                    }
                }
                else if(_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                    {
                        _tempLine.End.PositionCenter = (s as Circle).PositionCenter;
                        args.Handled = true;
                    }
                }
            };

            OnEmptyCircleMouseMove += (sender, args) =>
            {
                var pos = args.GetPosition(canvas);
                if(_currentMode == Mode.Default)
                {
                    if (pos.X < 0)
                        pos.X = 0;
                    if (pos.X >= canvas.ActualWidth)
                        pos.X = canvas.ActualWidth - 1;
                    if (pos.Y < 0)
                        pos.Y = 0;
                    if (pos.Y >= canvas.ActualHeight)
                        pos.Y = canvas.ActualHeight - 1;
                    if (_tempLine == null)
                        return;
                    if (args.RightButton == MouseButtonState.Pressed)
                        _tempLine.End.PositionCenter = pos;
                    else
                        _tempLine = null;
                }
                else if(_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                        _tempLine.End.PositionCenter = pos;
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
                if (_currentMode == Mode.Default)
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
                if (_currentMode == Mode.Default)
                {
                    if (_tempLine != null)
                        canvas.Children.Remove(_tempLine);
                    _tempLine = null;
                }
            };

            OnEmptyCircleMouseLeftButtonUp = (s, args) =>
            {
                if (_currentMode == Mode.Send)
                {
                    if (_tempLine != null)
                    {
                        canvas.Children.Remove(_tempLine);
                        ResetCircleStyle(_tempLine.Start);
                    }
                    _tempLine = null;
                }
            };
        }

        private void DisableCanvasMouseEvents()
        {
            canvas.MouseMove -= OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp -= OnEmptyCircleMouseRightButtonUp;
            canvas.MouseLeftButtonUp -= OnEmptyCircleMouseLeftButtonUp;
        }

        private void EnableCanvasMouseEvents()
        {
            canvas.MouseMove += OnEmptyCircleMouseMove;
            canvas.MouseRightButtonUp += OnEmptyCircleMouseRightButtonUp;
            canvas.MouseLeftButtonUp += OnEmptyCircleMouseLeftButtonUp;
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

        private void mode_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            switch(rb.Name)
            {
                case "deleteMode":
                    _currentMode = Mode.Delete;
                    break;
                case "sendMode":
                    _currentMode = Mode.Send;
                    break;
                default:
                    _currentMode = Mode.Default;
                    break;
            }
        }
    }
}
