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
        private Circle _previousCircle;
        private Circle _hoveringOver;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(canvas);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if(_hoveringOver != null)
                {
                    _hoveringOver.Position = new Point(pos.X - _hoveringOver.Radius, pos.Y - _hoveringOver.Radius);
                }
                else
                {
                    if (_previousCircle == null)
                    {
                        _previousCircle = new Circle(20, fill: new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)),
                                                         stroke: new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                                                         labelColor: new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)));
                        _previousCircle.MouseEnter += (s, args) =>
                        {
                            this.Cursor = Cursors.Hand;
                            _hoveringOver = s as Circle;
                        };
                        _previousCircle.MouseLeave += (s, args) =>
                        {
                            this.Cursor = Cursors.Arrow;
                            _hoveringOver = null;
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
            _previousCircle.Fill = new SolidColorBrush(Colors.White);
            _previousCircle.Stroke = new SolidColorBrush(Colors.Black);
            _previousCircle.LabelColor = new SolidColorBrush(Colors.Black);
            _previousCircle = null;
        }
    }
}
