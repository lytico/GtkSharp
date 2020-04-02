using System;
using Gtk;

class Test {
    public static void Main ()
    {
        uint r;
        double theta;

        Application.Init ();

        Gtk.Window win = new Gtk.Window ("Polar Coordinate Container");
        win.DeleteEvent += new DeleteEventHandler (Window_Delete);

        Notebook notebook = new Notebook ();
        win.Add (notebook);

        // Clock
        PolarFixed pf = new PolarFixed ();
        notebook.AppendPage (pf, new Label ("Clock"));

        for (int hour = 1; hour <= 12; hour ++) {
            theta = (Math.PI / 2) - hour * (Math.PI / 6);
            if (theta < 0)
                theta += 2 * Math.PI;

            Label l = new Label ("<big><b>" + hour.ToString () + "</b></big>");
            l.UseMarkup = true;
            pf.Put (l, theta, 200);
        }

        // Spiral
        pf = new PolarFixed ();
        notebook.AppendPage (pf, new Label ("Spiral"));

        r = 0;
        theta = 0.0;

        foreach (string id in Gtk.Stock.ListIds ()) {
            StockItem item = Gtk.Stock.Lookup (id);
            if (item.Label == null)
                continue;

            pf.Put (new Gtk.Button (id), theta, r);

            // Logarithmic spiral: r = a*e^(b*theta)
            r += 5;
            theta = 10 * Math.Log (10 * r);
        }

        win.ShowAll ();

        Application.Run ();
    }

    static void Window_Delete (object obj, DeleteEventArgs args)
    {
        Application.Quit ();
        args.RetVal = true;
    }
}