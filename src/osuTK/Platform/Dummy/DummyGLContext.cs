/* Licensed under the MIT/X11 license.
 * Copyright (c) 2006-2008 the osuTK Team.
 * This notice may not be removed from any source distribution.
 * See license.txt for licensing detailed licensing details.
 */

using System;
using System.Threading;

using osuTK.Graphics;

namespace osuTK.Platform.Dummy
{
    /// \internal
    /// <summary>
    /// An empty IGraphicsContext implementation to be used inside the Visual Studio designer.
    /// This class supports osuTK, and is not intended for use by osuTK programs.
    /// </summary>
    internal sealed class DummyGLContext : GraphicsContextBase
    {
        private readonly GraphicsContext.GetAddressDelegate Loader;

        private static int handle_count;
        private Thread current_thread;

        public DummyGLContext()
        {
            Handle = new ContextHandle(
                new IntPtr(Interlocked.Increment(
                    ref handle_count)));
        }

        public DummyGLContext(ContextHandle handle, GraphicsContext.GetAddressDelegate loader)
            : this()
        {
            if (handle != ContextHandle.Zero)
            {
                Handle = handle;
            }
            Loader = loader;
            Mode = new GraphicsMode(new IntPtr(2), 32, 16, 0, 0, 0, 2, false);
        }

        public override void SwapBuffers() { }

        public override void MakeCurrent(IWindowInfo info)
        {
            Thread new_thread = Thread.CurrentThread;
            // A context may be current only on one thread at a time.
            if (current_thread != null && new_thread != current_thread)
            {
                throw new GraphicsContextException(
                    "Cannot make context current on two threads at the same time");
            }

            if (info != null)
            {
                current_thread = Thread.CurrentThread;
            }
            else
            {
                current_thread = null;
            }
        }

        public override bool IsCurrent
        {
            get { return current_thread != null && current_thread == Thread.CurrentThread; }
        }

        public override IntPtr GetAddress(IntPtr function)
        {
            string str = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(function);
            return Loader(str);
        }

        public override int SwapInterval { get; set; }

        public override void Update(IWindowInfo window)
        { }

        public override void LoadAll()
        {
            #if OPENGL
            new osuTK.Graphics.OpenGL.GL().LoadEntryPoints();
            new osuTK.Graphics.OpenGL4.GL().LoadEntryPoints();
            #endif
            #if OPENGLES
            new osuTK.Graphics.ES11.GL().LoadEntryPoints();
            new osuTK.Graphics.ES20.GL().LoadEntryPoints();
            new osuTK.Graphics.ES30.GL().LoadEntryPoints();
            #endif
        }

        protected override void Dispose(bool disposing) { IsDisposed = true; }
    }
}
