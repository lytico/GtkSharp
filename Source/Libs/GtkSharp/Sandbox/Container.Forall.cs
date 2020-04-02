namespace Gtk
{

    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public partial class Container {

        public partial struct CallbackInvoker {
            public void Invoke(Widget w) {
                gtksharp_container_invoke_gtk_callback(Callback, w, data);
            }
        }

        static void gtksharp_container_invoke_gtk_callback(IntPtr cb, Widget widget, IntPtr data) {
            // TODO: cb(widget, data);
        }

        static void gtksharp_container_base_forall(IntPtr container, bool include_internals, IntPtr cb, IntPtr data) {
            // Find and call the first base callback that's not the GTK# callback. The GTK# callback calls down the whole
            // managed override chain, so calling it on a subclass-of-a-managed-container-subclass causes a stack overflow.

            // GtkContainerClass *parent = (GtkContainerClass *) G_OBJECT_GET_CLASS (container);
            // while ((parent = g_type_class_peek_parent (parent))) {
            //     if (strncmp (G_OBJECT_CLASS_NAME (parent), "__gtksharp_", 11) != 0) {
            //         if (parent->forall) {
            //             (*parent->forall) (container, include_internals, cb, data);
            //         }
            //         return;
            //     }
            // }
            var parent = TryGetObject(container).NativeType;
            while ((parent = parent.GetBaseType()) != GLib.GType.None) {
                if (parent == default) { // TODO
                }
            }
        }
    

    static void Forall_cb (IntPtr container, bool include_internals, IntPtr cb, IntPtr data)
        {
            try {
                //GtkContainer's unmanaged dispose calls forall, but by that time the managed object is gone
                //so it couldn't do anything useful, and resurrecting it would cause a resurrection cycle.
                //In that case, just chain to the native base in case it can do something.
                Container obj = (Container) GLib.Object.TryGetObject (container);
                if (obj != null) {
                    CallbackInvoker invoker = new CallbackInvoker (cb, data);
                    obj.ForAll (include_internals, new Gtk.Callback (invoker.Invoke));
                } else {
                    gtksharp_container_base_forall (container, include_internals, cb, data);
                }
            } catch (Exception e) {
                GLib.ExceptionManager.RaiseUnhandledException (e, false);
            }
        }

        static void gtksharp_container_override_forall (GLib.GType gtype, ForallDelegate cb)
        {
            // GtkContainerClass *klass = g_type_class_peek (gtype);
            // if (!klass)
            //     klass = g_type_class_ref (gtype);
            // ((GtkContainerClass *) klass)->forall = cb;
        }
        
        static void OverrideForAll (GLib.GType gtype, ForallDelegate callback)
        {
            unsafe {
                IntPtr* raw_ptr = (IntPtr*)(((long) gtype.GetClassPtr()) + (long) class_abi.GetFieldOffset("forall"));
                *raw_ptr = Marshal.GetFunctionPointerForDelegate((Delegate) callback);
            }
        }
        
        static void OverrideForall__ (GLib.GType gtype)
        {
            if (ForallCallback == null)
                ForallCallback = new ForallDelegate (Forall_cb);
            gtksharp_container_override_forall (gtype, ForallCallback);
        }

        [GLib.DefaultSignalHandler (Type=typeof(Gtk.Container), ConnectionMethod="OverrideForall")]
        protected virtual void ForAll (bool include_internals, Gtk.Callback callback)
        {
            CallbackInvoker invoker;
            try {
                invoker = (CallbackInvoker)callback.Target;
            } catch {
                throw new ApplicationException ("ForAll can only be called as \"base.ForAll()\". Use Forall() or Foreach().");
            }
            gtksharp_container_base_forall (Handle, include_internals, invoker.Callback, invoker.Data);
        }

        
    }
}