using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Hardware;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using OpenTK;

namespace Isv
{
	internal class CameraTexture : Java.Lang.Object, IDisposable, SurfaceTexture.IOnFrameAvailableListener
	{
		private int[] _textures = new int[1];
		private SurfaceTexture _surfaceTexture;
		private Android.Hardware.Camera _camera;

		public int TextureName{ get { return _textures [0]; } }

		public CameraTexture ()
		{
			GL.GenTextures (_textures.Length, _textures);

			var tex = (All)Android.Opengl.GLES11Ext.GlTextureExternalOes;

			GL.BindTexture (tex, TextureName);
			GL.TexParameter (tex, All.TextureMinFilter, (int)All.Nearest);
			GL.TexParameter (tex, All.TextureMagFilter, (int)TextureMagFilter.Nearest);

			_surfaceTexture = new SurfaceTexture (TextureName);

			_surfaceTexture.SetOnFrameAvailableListener (this);

			_camera = Android.Hardware.Camera.Open ();
			_camera.SetPreviewTexture (_surfaceTexture);
			_camera.StartPreview ();
		}

		#if false
		public void Dispose ()
		{
			_surfaceTexture.Dispose ();
		}
		#endif

		public EventHandler PreviewFrame;

		public void OnFrameAvailable (SurfaceTexture surfaceTexture)
		{
			if (PreviewFrame != null)
				PreviewFrame (this, EventArgs.Empty);
		}

		public void UpdateTexImage()
		{
			_surfaceTexture.UpdateTexImage ();
		}
	}
}

