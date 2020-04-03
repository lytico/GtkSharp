// adopted from: https://github.com/mono/xwt/blob/master/Xwt.XamMac/Xwt.Mac/ImageHandler.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Gtk;
using Gdk;
using System.Runtime.InteropServices;
using Cairo;
using Rectangle = Gdk.Rectangle;

namespace Samples
{
	public class GtkWorkarounds
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		delegate double d_gtk_widget_get_scale_factor(IntPtr widget);

		static d_gtk_widget_get_scale_factor gtk_widget_get_scale_factor =
			FuncLoader.LoadFunction<d_gtk_widget_get_scale_factor>(
				FuncLoader.GetProcAddress(GLibrary.Load(Library.Gtk), "gtk_widget_get_scale_factor"));

		static bool supportsHiResIcons = false;

		public static double GetScaleFactor(Gtk.Widget w)
		{
			if (!supportsHiResIcons)
				return 1;

			try {
				return gtk_widget_get_scale_factor(w.Handle);
			} catch (DllNotFoundException) { } catch (EntryPointNotFoundException) { }

			supportsHiResIcons = false;
			return 1;
		}
	}

	public class Util
	{
		public static double GetScaleFactor(Gtk.Widget w)
		{
			return GtkWorkarounds.GetScaleFactor(w);
		}

		public static Stream GetResourceStream(Assembly assembly, string name)
		{
			var resources = assembly.GetManifestResourceNames();
			var resourceName = resources.SingleOrDefault(str => str == name);

			// try harder:
			if (resourceName == default) {
				resourceName = resources.SingleOrDefault(str => str.EndsWith(name));
			}

			if (resourceName == default)
				return default;
			var stream = assembly.GetManifestResourceStream(resourceName);
			return stream;
		}
	}

	// This is a completely pointless widget, but its for testing subclassing Widget.OnDrawn
	public class GtkDrawingArea : Gtk.DrawingArea { }

	public class ImageBox : GtkDrawingArea
	{
		Pixbuf image;
		float yalign = 0.5f, xalign = 0.5f;

		public ImageBox(Pixbuf img) : this()
		{
			Image = img;
		}

		public ImageBox()
		{
			this.HasWindow = false;
			this.AppPaintable = true;
		}

		public Pixbuf Image {
			get { return image; }
			set {
				image = value;
				SetSizeRequest((int) image.Width, (int) image.Height);
				QueueResize();
			}
		}

		public float Yalign {
			get { return yalign; }
			set {
				yalign = value;
				QueueDraw();
			}
		}

		public float Xalign {
			get { return xalign; }
			set {
				xalign = value;
				QueueDraw();
			}
		}

		protected override void OnAdjustSizeRequest(Orientation orientation, out int minimum_size, out int natural_size)
		{
			base.OnAdjustSizeRequest(orientation, out minimum_size, out natural_size);
		}

 		// TODO
		// protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		// {
		//     base.OnSizeRequested (ref requisition);
		//     if (!image.IsNull) {
		//         requisition.Width = (int) image.Width;
		//         requisition.Height = (int) image.Height;
		//     }
		// }

		void DrawPixbuf(Cairo.Context ctx, Gdk.Pixbuf img, double x, double y, Size idesc)
		{
			ctx.Save();
			ctx.Translate(x, y);

			ctx.Scale(idesc.Width / (double) img.Width, idesc.Height / (double) img.Height);
			Gdk.CairoHelper.SetSourcePixbuf(ctx, img, 0, 0);

#pragma warning disable 618
			using (var p = ctx.Source) {
				if (p is SurfacePattern pattern) {
					if (idesc.Width > img.Width || idesc.Height > img.Height) {
						// Fixes blur issue when rendering on an image surface
						pattern.Filter = Cairo.Filter.Fast;
					} else
						pattern.Filter = Cairo.Filter.Good;
				}
			}
#pragma warning restore 618


			ctx.Paint();

			ctx.Restore();
		}

		protected override bool OnDrawn(Cairo.Context cr)
		{
			if (image == default)
				return true;
			var a = Allocation;
			Pixbuf pixbuff = image;
			// HACK: Gtk sends sometimes an expose/draw event while the widget reallocates.
			//       In that case we would draw in the wrong area, which may lead to artifacts
			//       if no other widget updates it. Alternative: we could clip the
			//       allocation bounds, but this may have other issues.
			if (a.Width == 1 && a.Height == 1 && a.X == -1 && a.Y == -1) // the allocation coordinates on reallocation
				return base.OnDrawn(cr);

			var x = (int) ((a.Width - (float) pixbuff.Width) * xalign);
			var y = (int) ((a.Height - (float) pixbuff.Height) * yalign);
			if (x < 0) x = 0;
			if (y < 0) y = 0;
			DrawPixbuf(cr, pixbuff, x, y, a.Size);
			return base.OnDrawn(cr);
		}
	}
}