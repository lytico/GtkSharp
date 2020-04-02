namespace Gtk {
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public partial class Container {
        public partial struct CallbackInvoker {
            
            internal CallbackInvoker(IntPtr cb,bool include_internals,IntPtr data)
            {
                this.cb = cb;
                this.data = data;
                this.GtkCallback = default;
                this.Include_internals = include_internals;
            }
            
            public void Invoke(Widget w) {
               // gtksharp_container_invoke_gtk_callback(Callback, w, data);
               D tryDelegate<D>(IntPtr _cb) {
                   try {
                       var d = Marshal.GetDelegateForFunctionPointer<D>(_cb);
                       return d;
                   } catch {
                       return default;
                   }
               }
              // var d1 =  tryDelegate<Callback>(Callback);
               var d2 =  tryDelegate<ForallDelegate>(Callback);
               //var d3 =  tryDelegate<Delegate>(Callback);
               d2?.Invoke(w.Handle,Include_internals, Callback,IntPtr.Zero);//data
            }


            internal Callback GtkCallback { get; set; }
            internal bool Include_internals { get; set; }
            
        }

        // from SharpGlue
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

        static void gtksharp_container_override_forall(GLib.GType gtype, ForallDelegate cb) {
            // GtkContainerClass *klass = g_type_class_peek (gtype);
            // if (!klass)
            //     klass = g_type_class_ref (gtype);
            // ((GtkContainerClass *) klass)->forall = cb;
        }

        // from mono/GtKSharp
        static void Forall_cb_(IntPtr container, bool include_internals, IntPtr cb, IntPtr data) {
            try {
                //GtkContainer's unmanaged dispose calls forall, but by that time the managed object is gone
                //so it couldn't do anything useful, and resurrecting it would cause a resurrection cycle.
                //In that case, just chain to the native base in case it can do something.
                Container obj = (Container) GLib.Object.TryGetObject(container);
                if (obj != null) {
                    CallbackInvoker invoker = new CallbackInvoker(cb, data);
                    obj.ForAll(include_internals, new Gtk.Callback(invoker.Invoke));
                } else {
                    gtksharp_container_base_forall(container, include_internals, cb, data);
                }
            } catch (Exception e) {
                GLib.ExceptionManager.RaiseUnhandledException(e, false);
            }
        }


        static void OverrideForAll_(GLib.GType gtype, ForallDelegate callback) {
            unsafe {
                IntPtr* raw_ptr = (IntPtr*) (((long) gtype.GetClassPtr()) + (long) class_abi.GetFieldOffset("forall"));
                *raw_ptr = Marshal.GetFunctionPointerForDelegate((Delegate) callback);
            }
        }

        static void OverrideForall_(GLib.GType gtype) {
            if (ForallCallback == null)
                ForallCallback = new ForallDelegate(Forall_cb_);
            gtksharp_container_override_forall(gtype, ForallCallback);
        }

        //[GLib.DefaultSignalHandler(Type = typeof(Gtk.Container), ConnectionMethod = "OverrideForall")]
        protected virtual void ForAll_(bool include_internals, Gtk.Callback callback) {
            CallbackInvoker invoker;
            try {
                invoker = (CallbackInvoker) callback.Target;
            } catch {
                throw new ApplicationException(
                    "ForAll can only be called as \"base.ForAll()\". Use Forall() or Foreach().");
            }

            gtksharp_container_base_forall(Handle, include_internals, invoker.Callback, invoker.Data);
        }

        // GtkSharp api
        static ForAllNativeDelegate ForAll_cb_delegate;

        static ForAllNativeDelegate ForAllVMCallback {
            get {
                if (ForAll_cb_delegate == null)
                    ForAll_cb_delegate = new ForAllNativeDelegate(ForAll_cb);
                return ForAll_cb_delegate;
            }
        }

        static void OverrideForAll(GLib.GType gtype) {
            OverrideForAll(gtype, ForAllVMCallback);
        }

        static void OverrideForAll(GLib.GType gtype, ForAllNativeDelegate callback) {
            unsafe {
                IntPtr* raw_ptr = (IntPtr*) (((long) gtype.GetClassPtr()) + (long) class_abi.GetFieldOffset("forall"));
                *raw_ptr = Marshal.GetFunctionPointerForDelegate((Delegate) callback);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void ForAllNativeDelegate(IntPtr inst, bool include_internals, IntPtr cb, IntPtr data);

        static void ForAll_cb(IntPtr inst, bool include_internals, IntPtr cb, IntPtr data) {
            try {
                //GtkContainer's unmanaged dispose calls forall, but by that time the managed object is gone
                //so it couldn't do anything useful, and resurrecting it would cause a resurrection cycle.
                //In that case, just chain to the native base in case it can do something.
                Container __obj = GLib.Object.GetObject(inst, false) as Container;
                if (__obj != null) {
                    CallbackInvoker invoker = new CallbackInvoker(cb, include_internals, data);
                    __obj.ForAll(include_internals, new Gtk.Callback(invoker.Invoke));
                } else {
                    gtksharp_container_base_forall(inst, include_internals, cb, data);
                }
            } catch (Exception e) {
                GLib.ExceptionManager.RaiseUnhandledException(e, true);
                // NOTREACHED: above call does not return.
                throw e;
            }
        }

        [GLib.DefaultSignalHandler(Type = typeof(Gtk.Container), ConnectionMethod = "OverrideForAll")]
        protected virtual void ForAll(bool include_internals, Callback callback) {
            InternalForAll(include_internals, callback);
        }

        ForAllNativeDelegate InternalForAllNativeDelegate() {
            ForAllNativeDelegate unmanaged = null;
            unsafe {
                IntPtr* raw_ptr = (IntPtr*) (((long) this.LookupGType().GetThresholdType().GetClassPtr()) +
                                             (long) class_abi.GetFieldOffset("forall"));
                unmanaged = (ForAllNativeDelegate) Marshal.GetDelegateForFunctionPointer(*raw_ptr,
                    typeof(ForAllNativeDelegate));
            }

            return unmanaged;
        }

        private void InternalForAll(bool include_internals, Callback callback) {

            var unmanaged = InternalForAllNativeDelegate();
            if (unmanaged == null) return;

            CallbackInvoker invoker;
            try {
                invoker = (CallbackInvoker) callback.Target;
            } catch {
                throw new ApplicationException(
                    "ForAll can only be called as \"base.ForAll()\". Use Forall() or Foreach().");
            }

            unmanaged(this.Handle, include_internals, invoker.Callback, invoker.Data);
        }
    }
}