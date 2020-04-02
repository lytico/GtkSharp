using System.Xml.Schema;
using GtkSharp;

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
            public void Invoke(Widget w) {
               // gtksharp_container_invoke_gtk_callback(Callback, w, data);
    
              // var d1 =  tryDelegate<Callback>(Callback);
              //var d3 =  tryDelegate<Delegate>(Callback);
               var d2 =  Container.tryDelegate<ForallDelegate>(Callback);
              
               d2?.Invoke(w.Handle,false, Callback,data);//data
            }

            
        }

       internal static D tryDelegate<D>(IntPtr _cb) {
            try {
                var d = Marshal.GetDelegateForFunctionPointer<D>(_cb);
                return d;
            } catch {
                return default;
            }
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

        /// <summary>
        /// sets callback forall in GtkContainerClass-Struct
        /// </summary>
        /// <param name="gtype"></param>
        /// <param name="callback"></param>
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
                    
                    __obj.ForAll(include_internals, w=>throw new NotImplementedException());
                } else {
                    gtksharp_container_base_forall(inst, include_internals, cb, data);
                }
            } catch (Exception e) {
                GLib.ExceptionManager.RaiseUnhandledException(e, true);
                // NOTREACHED: above call does not return.
                throw e;
            }
        }

        // [GLib.DefaultSignalHandler(Type = typeof(Gtk.Container), ConnectionMethod = "OverrideForAll")]
        // protected virtual void ForAll(bool include_internals, Callback callback) {
        //     InternalForAll(include_internals, callback);
        // }

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

            // TODO: unmanaged(this.Handle, include_internals, invoker.Callback, invoker.Data);
        }

        static void gtksharp_container_base_forall(IntPtr container, bool include_internals, IntPtr cb, IntPtr data) {
            
        }

        static void gtksharp_container_base_forall(Container container, bool include_internals, Callback cb, IntPtr data) {
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
            var parent = container.NativeType; //TryGetObject(container).NativeType;
            while ((parent = parent.GetBaseType()) != GLib.GType.None) {
                if (parent == default) { // TODO
                }
            }
        }

        static void gtksharp_container_override_forall_dummy(GLib.GType gtype, ForallDelegate cb) {
#if RELEASE
            throw new ApplicationException($"use {nameof(OverrideForAll)}");
#endif
            // GtkContainerClass *klass = g_type_class_peek (gtype);
            // if (!klass)
            //     klass = g_type_class_ref (gtype);
            // ((GtkContainerClass *) klass)->forall = cb;
            var isManaged = GLib.GType.IsManaged(gtype);
            var managedType = GLib.GType.LookupType(gtype.Val);
            var klass = GLib.GType.TypeClassPeek(gtype.Val);
            if (klass == IntPtr.Zero) {
                klass = GLib.GType.TypeClassPeek(gtype.Val);
            }
            // this is null, and is the container_struct:var managedTypeKlass =  GLib.GType.LookupType(klass);
        }
        
        //from gtk-sharp
        
        static void Forall_cb (IntPtr container, bool include_internals, IntPtr cb, IntPtr data)
        {
            try {
                //GtkContainer's unmanaged dispose calls forall, but by that time the managed object is gone
                //so it couldn't do anything useful, and resurrecting it would cause a resurrection cycle.
                //In that case, just chain to the native base in case it can do something.
                Container obj = (Container) GLib.Object.TryGetObject (container);
                var nativeCb = tryDelegate<CallbackNative>(cb);
                Callback callback = w => {
                    var wr = new GtkSharp.CallbackInvoker(nativeCb, data);
                    wr.Handler?.Invoke(w);
                };
                if (obj != null) {
                    //ContainerCallbackInvoker invoker = new ContainerCallbackInvoker (cb, data);
                   //obj.ForAll (include_internals, invoker.Invoke());

                    obj.ForAll (include_internals, callback);
                } else {
                    gtksharp_container_base_forall(obj, include_internals, callback, data);
                }
            } catch (Exception e) {
                GLib.ExceptionManager.RaiseUnhandledException (e, false);
            }
        }


        static void OverrideForall (GLib.GType gtype)
        {
            if (ForallCallback == null)
                ForallCallback = new ForallDelegate (Forall_cb);
            gtksharp_container_override_forall_dummy (gtype, ForallCallback);
            unsafe {
                IntPtr* raw_ptr = (IntPtr*) (((long) gtype.GetClassPtr()) + (long) class_abi.GetFieldOffset("forall"));
                *raw_ptr = Marshal.GetFunctionPointerForDelegate((Delegate) ForallCallback);
            }
        }

        [GLib.DefaultSignalHandler (Type=typeof(Gtk.Container), ConnectionMethod=nameof(OverrideForall))]
        protected virtual void ForAll (bool include_internals, Gtk.Callback callback)
        {
            
                // ContainerCallbackInvoker invoker;
                // try {
                //     invoker = (ContainerCallbackInvoker) callback.Target;
                // } catch {
                //     throw new ApplicationException(
                //         "ForAll can only be called as \"base.ForAll()\". Use Forall() or Foreach().");
                // }
                GtkSharp.CallbackWrapper cb_wrapper = new GtkSharp.CallbackWrapper (callback);
                gtksharp_container_base_forall(this, include_internals, callback, IntPtr.Zero);
            
        }
    }
}