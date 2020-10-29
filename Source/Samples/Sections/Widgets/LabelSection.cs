// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cairo;
using Gtk;
using Pango;
using Context = Cairo.Context;
using Rectangle = Gdk.Rectangle;
using WrapMode = Pango.WrapMode;

namespace Samples
{

	[Section (ContentType = typeof(Label), Category = Category.Widgets)]
	class LabelSection : ListSection
	{
		public LabelSection ()
		{
			AddItem (CreateSimpleLabel ());
			AddItem (CreateSizedLabel ());
			AddItem (CreateCharSizedLabel ());
			AddItem (CreateSizedBoxedLabel ());
			AddItem (CreateMarkupLabel ());
		}

		public (string, Widget) CreateSimpleLabel ()
		{
			var label = new Label ();

			// can be defined at constructor
			label.LabelProp = "This is a label";

			// right align text, center is default
			label.Xalign = 1f;

			return ("Label :", label);
		}

		public (string, Widget) CreateSizedLabel ()
		{
			var label = new SizedLabel {
				LabelProp = "This is a sized label",
				// right align text, center is default
				Valign = Align.Start,
				Expand = false,
				WidthRequest = 10,
				Ellipsize = EllipsizeMode.End
			};

			return ($"Label (Size={label.WidthRequest}):", label);
		}

		public (string, Widget) CreateSizedBoxedLabel ()
		{
			HBox box6 = new HBox ();
			var with = 10;
			var charWith = 3;
			for (int n = 0; n < 5; n++) {
				var w = new Label {
					Text = "L____" + n,
					WidthRequest = with,
					Expand = false,
					Valign = Align.Start,
					Ellipsize = EllipsizeMode.End,
					MaxWidthChars = charWith,
					LineWrapMode = WrapMode.Char,
					LineWrap = true,
				};
				w.Layout.GetExtents (out var inkRect, out var logicalRect);
				w.Layout.Width = (int)(with * Pango.Scale.PangoScale);
				// w.SizeAllocate (new Rectangle (0,0,10,10));
				box6.PackStart (w, false, false, 1);
			}

			return ($"Sized Labels Boxed ({nameof(Label.MaxWidthChars)}={charWith}):", box6);
		}

		public (string, Widget) CreateCharSizedLabel ()
		{
			var pxSize = 30;
			var label = new SizedLabel () {
				LabelProp = "This is a charsized label",
				Expand = false,
				Valign = Align.Start,
				Ellipsize = EllipsizeMode.End,
				LineWrapMode = WrapMode.Char,
				LineWrap = true,
			};
			var mChars = label.CalculateWidthChars (pxSize);
			label.MaxWidthChars = mChars;
			return ($"Label (PixelSize={pxSize}->WidthChars={label.MaxWidthChars}):", label);
		}

		public (string, Widget) CreateMarkupLabel ()
		{
			var label = new Label ();

			// activate markup, default is false
			label.UseMarkup = true;

			// define label with pango markup
			label.LabelProp = "This is a <span foreground=\"red\" size=\"large\">label</span> with <b>custom</b> markup";

			// right align text, center is default
			label.Xalign = 1f;

			return ("Label Markup:", label);
		}

	}

	internal class SizedLabel : Label
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
				Layout.Width = Math.Min ((int) (this.WidthRequest * Pango.Scale.PangoScale), Layout.Width);
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
				Layout.Width = Math.Min ((int) (this.WidthRequest * Pango.Scale.PangoScale), Layout.Width);
			}

			base.OnAdjustSizeAllocation (orientation, out minimum_size, out natural_size, out allocated_pos, out allocated_size);
			if (this.WidthRequest > 0) {
				if (orientation == Orientation.Horizontal) {
					allocated_size = Math.Max (natural_size, this.WidthRequest);
					minimum_size = Math.Max (minimum_size, this.WidthRequest);
				}
			}
		}

		public int CalculateWidthChars (int pixelWidth)
		{
			int LineWidth (Pango.LayoutLine l)
			{
				var i = new Pango.Rectangle ();
				var lo = new Pango.Rectangle ();
				l.GetExtents (ref i, ref lo);
				return i.Width;
			}

			IEnumerable<(int index, int width)> CharWidths (LayoutIter iter)
			{
				while (iter.NextChar ()) {
					var x = iter.CharExtents;
					yield return (iter.Index, x.Width);
				}
			}

			var max = this.Layout.LinesReadOnly.Aggregate ((i1, i2) => LineWidth (i1) > LineWidth (i2) ? i1 : i2);
			using var measure = Layout.Copy ();
			measure.Ellipsize = Pango.EllipsizeMode.None;
			measure.Wrap = WrapMode.Char;
			using var iter = measure.Iter;
			var lls = CharWidths (iter)
				.OrderBy (cw => cw.index)
				.ToArray ();
			var iLen = 0;
			foreach (var cwi in lls) {
				iLen += cwi.width;
				if (iLen > pixelWidth * Pango.Scale.PangoScale) {
					return cwi.index - 1;
				}
			}

			return -1;
		}

	}

}