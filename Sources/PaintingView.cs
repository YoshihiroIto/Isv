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
using Android.Opengl;

namespace Isv
{
	class PaintingView : AndroidGameView
	{
		private static readonly float[] vertices = new [] {
			-1.0f, +1.0f, 0.0f, 0.0f, 1.0f,
			+1.0f, +1.0f, 0.0f, 1.0f, 1.0f,
			-1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
			+1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
		};
		private int _viewportWidth = 1;
		private int _viewportHeight = 1;
		private VideoBlendShaderProgram _videoBlend;
		private MovieTexture _movieTexA;
		private MovieTexture _movieTexB;
		private CameraTexture _movieTexC;

		private bool _frameAvailableTexA;
		private bool _frameAvailableTexB;
		private bool _frameAvailableTexC;

		private readonly float[] _blendRatio = new [] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};
		private readonly bool[] _blendDir = new [] {false, false, false, false, false, false};

		private readonly float[] _uniformBlendRatio = new float [4];


		public float Scale
		{
			set {
				_movieTexA.Scale = value;
				_movieTexB.Scale = value;
				_movieTexC.Scale = value;
			}
		}

		public float Rotate
		{
			set {
				_movieTexA.Rotate = value;
				_movieTexB.Rotate = value;
				_movieTexC.Rotate = value;
			}
		}

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

		public void SeekBegin(Channel channel)
		{
			switch(channel)
			{
			case Channel.A:
				_movieTexA.SeekBegin ();
				break;
			case Channel.B:
				_movieTexB.SeekBegin ();
				break;
			}
		}

		public void Play(Channel channel, string filePath)
		{
			switch(channel)
			{
			case Channel.A:
				_movieTexA.Play (filePath);
				break;
			case Channel.B:
				_movieTexB.Play (filePath);
				break;
			}
		}

		public void StartBlend(Channel channel, bool isUp)
		{
			_blendDir [(int)channel] = isUp;
		}

		public void SetBlendRatio(int index, float ratio)
		{
			_blendRatio [index] = ratio;
		}

		public override bool OnTouchEvent (MotionEvent e)
		{
			//_movieTexB.Play ("/sdcard/Movies/Mayday2012b.mp4");

			var srcX = (float)e.RawX;
			var srcY = (float)e.RawY;

			var x = srcX / Width;
			var y = srcY / Height;

			y -= 0.2f;
			y *= 1.4f;

			var index = Math.Max(0, Math.Min(2, (int)(x * 3.0f)));
			var value = 1.0f - Math.Max(0.0f, Math.Min(1.0f, y));

			_blendRatio[index] = value;

			return base.OnTouchEvent (e);
		}

		private void Initialize ()
		{
			RenderFrame += (s, e) => OnRender ();
			UpdateFrame += (s, e) => OnUpdate ();
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

			try {
				Log.Verbose ("TexturedCube", "Loading with default settings");

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}

			try {
				Log.Verbose ("TexturedCube", "Loading with custom Android settings (low mode)");
				GraphicsMode = new AndroidGraphicsMode (16, 0, 0, 0, 0, false);

				// if you don't call this, the context won't be created
				base.CreateFrameBuffer ();
				return;
			} catch (Exception ex) {
				Log.Verbose ("TexturedCube", "{0}", ex);
			}
				
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
			base.OnLoad (e);

			MakeCurrent ();

			_viewportWidth = Width;
			_viewportHeight = Height;

			_videoBlend = new VideoBlendShaderProgram (Context.Assets);

			_movieTexA = new MovieTexture ();
			_movieTexA.FrameAvailable += OnFrameAvailable;

			_movieTexB = new MovieTexture ();
			_movieTexB.FrameAvailable += OnFrameAvailable;

			_movieTexC = new CameraTexture ();
			_movieTexC.FrameAvailable += OnFrameAvailable;
	
			Run ();
		}

		protected override void OnUnload (EventArgs e)
		{
			_videoBlend.Dispose ();
			_movieTexA.Dispose ();
			_movieTexB.Dispose ();
			_movieTexC.Dispose ();

			base.OnUnload (e);
		}

		private void OnFrameAvailable (object sender, EventArgs e)
		{
			#if false
			MakeCurrent ();
			(sender as VideoTextureBase).UpdateTexImage ();
			#endif

			if (sender == _movieTexA)
				_frameAvailableTexA = true;

			if (sender == _movieTexB)
				_frameAvailableTexB = true;

			if (sender == _movieTexC)
				_frameAvailableTexC = true;
		}

		protected override void OnResize (EventArgs e)
		{
			_viewportWidth = Width;
			_viewportHeight = Height;

			_movieTexC.SetPreviewSize (_viewportWidth, _viewportHeight);
		}
				
		private void OnUpdate ()
		{
		}

		private void OnRender ()
		{
			MakeCurrent ();

			_videoBlend.Use ();

			if (_frameAvailableTexA) {
				_movieTexA.UpdateTexImage ();
				_frameAvailableTexA = false;
			}

			if (_frameAvailableTexB) {
				_movieTexB.UpdateTexImage ();
				_frameAvailableTexB = false;
			}

			if (_frameAvailableTexC) {
				_movieTexC.UpdateTexImage ();
				_frameAvailableTexC = false;
			}

			for (var i = 0; i != _blendDir.Length; ++i) {
				_blendRatio [i] += _blendDir [i] ? 0.3f : -0.3f;
				_blendRatio [i] = Math.Max (0.0f, Math.Min (1.0f, _blendRatio [i]));
			}

			_uniformBlendRatio [0] = _blendRatio [(int)Channel.A];
			_uniformBlendRatio [1] = _blendRatio [(int)Channel.B];
			_uniformBlendRatio [2] = _blendRatio [(int)Channel.C];
			_uniformBlendRatio [3] = _blendRatio [(int)Channel.F];

			var poster = _blendRatio [(int)Channel.D] * 256.0f + 4.0f;

			GL.UniformMatrix4 (_videoBlend.UniformTexTransformA, 1, false, _movieTexA.Transform);
			GL.UniformMatrix4 (_videoBlend.UniformTexTransformB, 1, false, _movieTexB.Transform);
			GL.UniformMatrix4 (_videoBlend.UniformTexTransformC, 1, false, _movieTexC.Transform);
			GL.Uniform1 (_videoBlend.UniformTexA, 0);
			GL.Uniform1 (_videoBlend.UniformTexB, 1);
			GL.Uniform1 (_videoBlend.UniformTexC, 2);
			GL.Uniform4 (_videoBlend.UniformBlendRatio, 1, _uniformBlendRatio);
			GL.Uniform2 (_videoBlend.UniformCameraInvSize, 1, _movieTexC.CameraInvSize);
			GL.Uniform1 (_videoBlend.UniformPoster, poster);

			RenderQuad ();
		}

		private unsafe void RenderQuad ()
		{
			GL.Disable (All.DepthTest);

			GL.ClearColor (0.0f, 0.0f, 0.0f, 1.0f);
			GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

			GL.Viewport (0, 0, _viewportWidth, _viewportHeight);

			GL.ActiveTexture (All.Texture0);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, 0);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, _movieTexA.TextureName);

			GL.ActiveTexture (All.Texture1);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, 0);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, _movieTexB.TextureName);

			GL.ActiveTexture (All.Texture2);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, 0);
			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, _movieTexC.TextureName);

			GL.VertexAttribPointer (ShaderProgram.AttribPosition, 3, All.Float, false, sizeof(float) * 5, vertices);
			GL.EnableVertexAttribArray (ShaderProgram.AttribPosition);

			fixed(float *texCoordHead = &vertices[3]) {
				GL.VertexAttribPointer (ShaderProgram.AttribTexcoord, 2, All.Float, false, sizeof(float) * 5, new IntPtr (texCoordHead));
				GL.EnableVertexAttribArray (ShaderProgram.AttribTexcoord);
			}

			GL.DrawArrays (All.TriangleStrip, 0, 4);

			SwapBuffers ();
		}
	}
}
