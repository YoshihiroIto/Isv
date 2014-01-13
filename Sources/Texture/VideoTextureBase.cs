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

		protected SurfaceTexture SurfaceTexture {
			get;
			private set;
		}

		public VideoTextureBase()
		{
			Transform = new float[16];

			GL.GenTextures (_textures.Length, _textures);

			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, TextureName);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMinFilter, (int)All.Nearest);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMagFilter, (int)All.Nearest);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureWrapS, (int)All.Repeat);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureWrapT, (int)All.Repeat);

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

			if (FrameAvailable != null)
				FrameAvailable (this, EventArgs.Empty);
		}

		public virtual void UpdateTexImage ()
		{
			SurfaceTexture.UpdateTexImage ();
		}
	}
}

