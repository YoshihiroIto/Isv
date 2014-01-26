using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;

namespace Isv
{
	internal abstract class VideoTextureBase : Java.Lang.Object,
	IDisposable,
	SurfaceTexture.IOnFrameAvailableListener
	{
		public int TextureName{ get { return _textures [0]; } }
		public EventHandler FrameAvailable;

		private int[] _textures = new int[1];

		public float[] Transform {
			private set;
			get;
		}

		private float Scale { get; set; }
		private float Rotate { get; set; }      // unit:degree

		protected SurfaceTexture SurfaceTexture {
			get;
			private set;
		}

		public VideoTextureBase()
		{
			Transform = new float[16];
			//Scale = 1.0f;
			//Rotate = 0.0f;
			Scale = 3.0f;
			Rotate = 45.0f;

			GL.GenTextures (_textures.Length, _textures);

			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, TextureName);

			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMinFilter, (int)All.Linear);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMagFilter, (int)All.Linear);

			SurfaceTexture = new SurfaceTexture (TextureName);
			SurfaceTexture.SetOnFrameAvailableListener (this);
		}

		public new void Dispose()
		{
			GL.DeleteTextures(_textures.Length, _textures);
		}

		public virtual void OnFrameAvailable (SurfaceTexture surfaceTexture)
		{
			SurfaceTexture.GetTransformMatrix (Transform);

			Android.Opengl.Matrix.ScaleM (Transform, 0, Scale, Scale, 1.0f);
			Android.Opengl.Matrix.RotateM (Transform, 0, Rotate, 0.0f, 0.0f, 1.0f);

			if (FrameAvailable != null)
				FrameAvailable (this, EventArgs.Empty);
		}

		public virtual void UpdateTexImage ()
		{
			SurfaceTexture.UpdateTexImage ();
		}
	}
}

