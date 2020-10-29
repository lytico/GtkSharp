// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System.Runtime.CompilerServices;
using Gtk;

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
            var label = new Label {
                LabelProp = "This is a sized label",
                // right align text, center is default
                Xalign = 1f,
                WidthRequest = 10,
            };

            return ($"Label (Size={label.WidthRequest}):", label);
        }

        public (string, Widget) CreateSizedBoxedLabel()
            {
            HBox box6 = new HBox ();
            var with = 10;
            for (int n=0; n<15; n++) {
                var w = new Label ("TestLabel" + n) {
                    WidthRequest = with,
                    Expand =  false,
                };
                box6.PackStart (w,false,false,1);
            }
            return ($"Labels Boxed (Size={with}):", box6);
        }
        
        public (string, Widget) CreateCharSizedLabel()
        {
            var label = new Label {
                LabelProp = "This is a charsized label",
                // left align text, center is default
                Xalign = 0f, 
                WidthChars = 10
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
