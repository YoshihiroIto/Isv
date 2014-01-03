using System;
using System.Runtime.InteropServices;
using System.Text;

using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;

using Android.Util;
using Android.Views;
using Android.Content;

// Render a triangle using OpenGLES 3.0

namespace Isv {

	class PaintingView : AndroidGameView
	{
		int viewportWidth, viewportHeight;
		//int program;
		float [] vertices;

		public PaintingView (Context context, IAttributeSet attrs) :
			base (context, attrs)
		{
			Initialize ();
		}

		public PaintingView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
			: base (handle, transfer)
		{
			Initialize ();
		}

		private void Initialize ()
		{
		}

		protected override void CreateFrameBuffer ()
		{
			GLContextVersion = GLContextVersion.Gles3_0;

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("GLTriangle", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLTriangle", "{0}", ex);
			}

			// this is a graphics setting that sets everything to the lowest mode possible so
			// the device returns a reliable graphics setting.
			try {
				Log.Verbose ("GLTriangle", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode ();//0, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("GLTriangle", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}


        private ShaderProgram _simple;

		protected override void OnLoad (EventArgs e)
		{
			// This is completely optional and only needed
			// if you've registered delegates for OnLoad
			base.OnLoad (e);

			viewportHeight = Height;
			viewportWidth = Width;

			_simple = new ShaderProgram (Context.Assets, "Resources/shaders/Simple.vsh", "Resources/shaders/Simple.fsh"); 

			RenderTriangle ();
		}

		void RenderTriangle ()
		{
			vertices = new float [] {
					0.0f, 0.5f, 0.0f,
					-0.5f, -0.5f, 0.0f,
					0.5f, -0.5f, 0.0f
				};

			GL.ClearColor (0.7f, 0.7f, 0.7f, 1);
			GL.Clear (ClearBufferMask.ColorBufferBit);

			GL.Viewport (0, 0, viewportWidth, viewportHeight);

			_simple.Use ();

			GL.VertexAttribPointer (0, 3, All.Float, false, 0, vertices);
			GL.EnableVertexAttribArray (0);

			GL.DrawArrays (All.Triangles, 0, 3);

			SwapBuffers ();
		}

		// this is called whenever android raises the SurfaceChanged event
		protected override void OnResize (EventArgs e)
		{
			viewportHeight = Height;
			viewportWidth = Width;

			// the surface change event makes your context
			// not be current, so be sure to make it current again
			MakeCurrent ();
			RenderTriangle ();
		}
	}
}
