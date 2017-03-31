using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RoutingSimulator.UI.Shapes
{
    public class Circle
    {
        private class CircleLabel
        {
            private Label _label;
            public Label UILabel
            {
                get
                {
                    return _label;
                }
            }
            public string Text
            {
                get
                {
                    return _label.Content.ToString();
                }
                set
                {
                    _label.Content = value;
                }
            }
            public Brush Foreground
            {
                get
                {
                    return _label.Foreground;
                }
                set
                {
                    _label.Foreground = value;
                }
            }
            public Brush Background
            {
                get
                {
                    return _label.Background;
                }
                set
                {
                    _label.Background = value;
                }
            }
            public double Height
            {
                get
                {
                    _label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return _label.DesiredSize.Height;
                }
            }
            public double Width
            {
                get
                {
                    _label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    return _label.DesiredSize.Width;
                }
            }

            public CircleLabel() : this(new SolidColorBrush(Colors.Black), null)
            { }

            public CircleLabel(Brush foreground, Brush background)
            {
                _label = new Label();
                Foreground = foreground;
                Background = background;
                Text = Id;
            }

            private static int _id = 0;
            private const string Letters = "abcdefghijklmnopqrstuvwxyz";
            private static string Id
            {
                get
                {
                    string result = "";
                    int b = Letters.Length;
                    int tmpId = _id++;
                    while(tmpId >= 0)
                    {
                        result = Letters[tmpId % b] + result;
                        tmpId /= b;
                        tmpId--;
                    }
                    return result;
                }
            }
        }
        private Ellipse _circle;
        private CircleLabel _label;
        private Point _position;

        public Brush Fill
        {
            get
            {
                return _circle.Fill;
            }
            set
            {
                _circle.Fill = value;
            }
        }
        public Brush Stroke
        {
            get
            {
                return _circle.Stroke;
            }
            set
            {
                _circle.Stroke = value;
            }
        }
        public Brush LabelColor
        {
            get
            {
                return _label.Foreground;
            }
            set
            {
                _label.Foreground = value;
            }
        }
        public double Radius
        {
            get
            {
                return _circle.Width / 2;
            }
            set
            {
                _circle.Width = _circle.Height = 2 * value;
            }
        }
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                Canvas.SetTop(_circle, _position.Y);
                Canvas.SetLeft(_circle, _position.X);
                Canvas.SetTop(_label.UILabel, _position.Y + _circle.Height / 2 - _label.Height / 2);
                Canvas.SetLeft(_label.UILabel, _position.X + _circle.Width / 2 - _label.Width / 2);
            }
        }
        public IEnumerable<UIElement> UIElements
        {
            get
            {
                yield return _circle;
                yield return _label.UILabel;
            }
        }

        public Circle() : this(1, new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black))
        { }

        public Circle(double radius) : this(radius, new SolidColorBrush(Colors.White), new SolidColorBrush(Colors.Black), new SolidColorBrush(Colors.Black))
        { }

        public Circle(double radius, Brush fill, Brush stroke, Brush labelColor)
        {
            _circle = new Ellipse();
            Radius = radius;
            Fill = fill;
            Stroke = stroke;
            _label = new CircleLabel();
            Position = new Point(0, 0);
            _circle.MouseEnter += (s, e) => MouseEnter?.Invoke(this, e);
            _circle.MouseLeave += (s, e) => MouseLeave?.Invoke(this, e);
            _label.UILabel.MouseEnter += (s, e) => MouseEnter?.Invoke(this, e);
            _label.UILabel.MouseLeave += (s, e) => MouseLeave?.Invoke(this, e);
        }


        public event EventHandler<MouseEventArgs> MouseEnter;
        public event EventHandler<MouseEventArgs> MouseLeave;
    }
}
