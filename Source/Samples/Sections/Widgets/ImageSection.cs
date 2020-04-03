// This is free and unencumbered software released into the public domain.
// Happy coding!!! - GtkSharp Team

using System.Collections.Generic;
using Gdk;
using Gtk;

namespace Samples
{
	[Section(ContentType = typeof(ImageBox), Category = Category.Widgets)]
	class ImageSection : ListSection
	{
		public ImageSection()
		{
			AddItem(CreateContainer());
		}

		public (string, Widget) CreateContainer()
		{
			Pixbuf image = default;
			using (var stream = Util.GetResourceStream(typeof(ImageSection).Assembly, "Testpic.png")) {
				image = new Pixbuf(stream);
			}

			var container = new ImageBox(image);


			return ($"{nameof(ImageBox)}:", container);
		}
	}
}