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
        private const double CircleRadius = 15;
        private readonly Brush CircleFill = new SolidColorBrush(Colors.White);
        private readonly Brush CircleStroke = new SolidColorBrush(Colors.Black);
        private readonly Brush CircleLabelColor = new SolidColorBrush(Colors.Black);
        private Circle _previousCircle;
        private Circle _selectedCircle;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(_selectedCircle != null)
                {
                    _selectedCircle.Position = new Point(pos.X - _selectedCircle.Radius, pos.Y - _selectedCircle.Radius);
                }
                else
                {
                    if (_previousCircle == null)
                    {
                        _previousCircle = new Circle(CircleRadius, fill: CircleFill.Clone(),
                                                                   stroke: CircleStroke.Clone(),
                                                                   labelColor: CircleLabelColor.Clone(),
                                                                   opacity: 0.5);
                        _previousCircle.MouseEnter += (s, args) =>
                        {
                            this.Cursor = Cursors.Hand;
                            _selectedCircle = s as Circle;
                        };
                        _previousCircle.MouseLeave += (s, args) =>
                        {
                            this.Cursor = Cursors.Arrow;
                            _selectedCircle = null;
                        };
                        foreach (var el in _previousCircle.UIElements)
                        {
                            canvas.Children.Add(el);
                        
                        }
                    }
                    _previousCircle.Position = new Point(pos.X - _previousCircle.Radius, pos.Y - _previousCircle.Radius);
                }
            }
            else if (_previousCircle != null)
            {
                _previousCircle = null;
            }
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_previousCircle == null)
                return;
            _previousCircle.Opacity = 1;
            _previousCircle = null;
        }
    }
}
