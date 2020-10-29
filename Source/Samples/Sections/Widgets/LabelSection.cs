// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System;
using System.Runtime.CompilerServices;
using Cairo;
using Gtk;
using Pango;
using Context = Cairo.Context;
using Rectangle = Gdk.Rectangle;
using WrapMode = Pango.WrapMode;

namespace Samples
{
    [Section(ContentType = typeof(Label), Category = Category.Widgets)]
    class LabelSection : ListSection
    {
        public LabelSection()
        {
            AddItem(CreateSimpleLabel());
            AddItem(CreateSizedLabel());
            AddItem(CreateCharSizedLabel());
            AddItem(CreateSizedBoxedLabel());
            AddItem(CreateMarkupLabel());
        }

        public (string, Widget) CreateSimpleLabel()
        {
            var label = new Label();

            // can be defined at constructor
            label.LabelProp = "This is a label";

            // right align text, center is default
            label.Xalign = 1f; 

            return ("Label :", label);
        }

        public (string, Widget) CreateSizedLabel ()
        {
            var label = new SizedLabel  {
                LabelProp = "This is a sized label",
                // right align text, center is default
                Valign = Align.Start,
                Expand = false,
                WidthRequest = 10,
                Ellipsize = EllipsizeMode.End
            };

            return ($"Label (Size={label.WidthRequest}):", label);
        }

        public class SizedLabel : Label
        {
            protected override bool OnDrawn (Context cr)
            {
                var bounds = cr.ClipExtents ();
                // if (this.WidthRequest > 0) {
                //     bounds = new Cairo.Rectangle (bounds.X, bounds.Y, WidthRequest, bounds.Height);
                //     cr.Rectangle (bounds);
                //     cr.Clip ();
                // }


                return base.OnDrawn (cr);
            }


            protected override void OnAdjustSizeRequest (Orientation orientation, out int minimum_size, out int natural_size)
            {
                if (this.WidthRequest > 0 && Layout.Width > 0) {
                    Layout.Width = Math.Min((int)(this.WidthRequest*Pango.Scale.PangoScale),Layout.Width);
                }
                base.OnAdjustSizeRequest (orientation, out minimum_size, out natural_size);
                // if (this.WidthRequest > 0) {
                //     Layout.Width = this.WidthRequest;
                // }
                if (this.WidthRequest > 0 && orientation == Orientation.Horizontal) {
                    minimum_size = Math.Min (minimum_size, this.WidthRequest);
                    natural_size = Math.Min (minimum_size, natural_size);
                }

            }

            protected override void OnAdjustSizeAllocation (Orientation orientation, out int minimum_size, out int natural_size, out int allocated_pos, out int allocated_size)
            {
                if (this.WidthRequest > 0 && Layout.Width > 0) {
                    Layout.Width = Math.Min((int)(this.WidthRequest*Pango.Scale.PangoScale),Layout.Width);
                }

                base.OnAdjustSizeAllocation (orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
                if (this.WidthRequest > 0) {
                  
                    if (orientation == Orientation.Horizontal) {
                        allocated_size = Math.Max (natural_size, this.WidthRequest);
                        minimum_size = Math.Max (minimum_size, this.WidthRequest);
                    }
                }
                
            }




        }

        public (string, Widget) CreateSizedBoxedLabel()
            {
            HBox box6 = new HBox ();
            var with = 10;
            var charWith = 3;
            for (int n=0; n<5; n++) {
                var w = new Label  {
                    Text = "L____" + n,
                    WidthRequest = with, 
                    Expand =  false,
                    Valign = Align.Start,
                    Ellipsize = EllipsizeMode.End,
                    //MaxWidthChars = charWith,
                    LineWrapMode = WrapMode.Char,
                    LineWrap = true,
                };
                w.Layout.GetExtents (out var inkRect, out var logicalRect);
               // w.Layout.Width = with;// Pango.Units.FromPixels (with);
               // w.SizeAllocate (new Rectangle (0,0,10,10));
                box6.PackStart (w,false,false,1);
            }
            return ($"Sized Labels Boxed ({nameof(Label.MaxWidthChars)}={charWith}):", box6);
        }
        
        public (string, Widget) CreateCharSizedLabel()
        {
            var label = new Label {
                LabelProp = "This is a charsized label",
                WidthChars = 1,
                Ellipsize = EllipsizeMode.End,
                Expand = false,
                Valign = Align.Start,
            };

            return ($"Label (WidthChars={label.WidthChars}):", label);
        }
        public (string, Widget) CreateMarkupLabel()
        {
            var label = new Label();

            // activate markup, default is false
            label.UseMarkup = true;

            // define label with pango markup
            label.LabelProp = "This is a <span foreground=\"red\" size=\"large\">label</span> with <b>custom</b> markup";

            // right align text, center is default
            label.Xalign = 1f;

            return ("Label Markup:", label);
        }

    }
}
