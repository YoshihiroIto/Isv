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

namespace Isv
{
	class PaintingView : AndroidGameView
	{
		int viewportWidth, viewportHeight;
		float[] vertices;
		private ShaderProgram _simple;
		private CameraTexture _cameraTex;

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
			ContextRenderingApi = GLVersion.ES3;

			try {
				Log.Verbose ("TexturedCube", "Loading with high quality settings");

				GraphicsMode = new GraphicsMode (new ColorFormat (32), 24, 0, 0); 
				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
			try {
				Log.Verbose ("TexturedCube", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// Fallback modes
			// If the first attempt at initializing the surface with a default graphics
			// mode fails, then the app can try different configurations. Devices will
			// support different modes, and what is valid for one might not be valid for
			// another. If all options fail, you can set all values to 0, which will
			// ask for the first available configuration the device has without any
			// filtering.
			// After a successful call to base.CreateFrameBuffer(), the GraphicsMode
			// object will have its values filled with the actual values that the
			// device returned.


			// This is a setting that asks for any available 16-bit color mode with no
			// other filters. It passes 0 to the buffers parameter, which is an invalid
			// setting in the default OpenTK implementation but is valid in some
			// Android implementations, so the AndroidGraphicsMode object allows it.
			try {
				Log.Verbose ("TexturedCube", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (16, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			// this is a setting that doesn't specify any color values. Certain devices
			// return invalid graphics modes when any color level is requested, and in
			// those cases, the only way to get a valid mode is to not specify anything,
			// even requesting a default value of 0 would return an invalid mode.
			try {
				Log.Verbose ("TexturedCube", "Loading with no Android settings");
				GraphicsMode = new AndroidGraphicsMode (0, 4, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}
			throw new Exception ("Can't load egl, aborting");
		}

		protected override void OnLoad (EventArgs e)
		{
			// This is completely optional and only needed
			// if you've registered delegates for OnLoad
			base.OnLoad (e);

			GL.Disable (All.DepthTest);

			viewportHeight = Height;
			viewportWidth = Width;

			_simple = new ShaderProgram (Context.Assets, "Resources/shaders/Simple.vsh", "Resources/shaders/Simple.fsh"); 
			_cameraTex = new CameraTexture ();

			_cameraTex.PreviewFrame += (ss, ee) =>
			{
				MakeCurrent ();
				_cameraTex.UpdateTexImage();
				RenderTriangle ();
			};

			RenderTriangle ();
		}

		private unsafe void RenderTriangle ()
		{
			var scale = 1.0f;

			vertices = new [] {
				-1.0f*scale, +1.0f*scale, 0.0f, 0.0f, 0.0f,
				+1.0f*scale, +1.0f*scale, 0.0f, 1.0f, 0.0f,

				-1.0f*scale, -1.0f*scale, 0.0f, 0.0f, 1.0f,
				+1.0f*scale, -1.0f*scale, 0.0f, 1.0f, 1.0f,
			};

			GL.ClearColor (1.0f, 0.0f, 0.0f, 1);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Viewport (0, 0, viewportWidth, viewportHeight);

			_simple.Use ();

			var tex = (All)Android.Opengl.GLES11Ext.GlTextureExternalOes;
			GL.ActiveTexture (All.Texture0);
			GL.BindTexture (tex, _cameraTex.TextureName);

			GL.VertexAttribPointer (ShaderProgram.AttrPosition, 3, All.Float, false, sizeof(float) * 5, vertices);
			GL.EnableVertexAttribArray (ShaderProgram.AttrPosition);

			fixed(float *texCoordHead = &vertices[3]) {
				GL.VertexAttribPointer (ShaderProgram.AttrTexcoord, 2, All.Float, false, sizeof(float) * 5, new IntPtr (texCoordHead));
				GL.EnableVertexAttribArray (ShaderProgram.AttrTexcoord);
			}

			GL.DrawArrays (All.TriangleStrip, 0, 4);

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
