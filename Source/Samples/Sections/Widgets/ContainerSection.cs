// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System.Collections.Generic;
using Gtk;

namespace Samples
{

    [Section(ContentType = typeof(CustomContainer), Category = Category.Widgets)]
    class ContainerSection : ListSection
    {
        public ContainerSection()
        {
            AddItem(CreateContainer());
        }

        public (string, Widget) CreateContainer()
        {
            var container = new CustomContainer();

            var label = new Label
            {
                Text = "Child"
            };

            container.Add(label);

            return ("CustomContainer:", container);
        }
    }

    public class CustomContainer : Container {
        public CustomContainer() {
            this.HasWindow = false;
        }

        List<Gtk.Widget> children = new List<Widget>();
        Dictionary<object, Gdk.Rectangle> sizes = new Dictionary<object, Gdk.Rectangle>();

        public new void Add(Gtk.Widget widget) {
            children.Add(widget);
            widget.SizeAllocated += (o, args) => {
                sizes[o] = args.Allocation;
            };
        }

        protected override void OnAdded(Gtk.Widget widget) {
            widget.Parent = this;
            sizes[widget] = new Gdk.Rectangle();
        }

        protected override void OnRemoved(Gtk.Widget widget) {
            children.Remove(widget);
            widget.Unparent();
            QueueResize();
        }

        protected override void OnSizeAllocated(Gdk.Rectangle allocation) {
            base.OnSizeAllocated(allocation);
            try { } catch { }

            foreach (var cr in children.ToArray()) {
                sizes.TryGetValue(cr, out var r);
                cr.SizeAllocate(new Gdk.Rectangle(allocation.X + (int) r.X, allocation.Y + (int) r.Y, (int) r.Width,
                    (int) r.Height));
            }
        }
    }
}

