using RoutingSimulator.UI.Shapes;
using System.Windows.Controls;

namespace RoutingSimulator.UI
{
    static class Extensions
    {
        public static void Add(this UIElementCollection collection, Circle c)
        {
            foreach (var el in c.UIElements)
                collection.Add(el);
        }
        public static void Remove(this UIElementCollection collection, Circle c)
        {
            foreach (var el in c.UIElements)
                collection.Remove(el);
        }
    }
}
