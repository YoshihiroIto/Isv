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
	SurfaceTexture.IOnFrameAvailableListener
	{
		public int TextureName{ get { return _textures [0]; } }
		public EventHandler FrameAvailable;

		private int[] _textures = new int[1];

		protected SurfaceTexture SurfaceTexture {
			get;
			private set;
		}

		public VideoTextureBase()
		{
			GL.GenTextures (_textures.Length, _textures);

			GL.BindTexture ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, TextureName);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMinFilter, (int)All.Nearest);
			GL.TexParameter ((All)Android.Opengl.GLES11Ext.GlTextureExternalOes, All.TextureMagFilter, (int)TextureMagFilter.Nearest);

			SurfaceTexture = new SurfaceTexture (TextureName);
			SurfaceTexture.SetOnFrameAvailableListener (this);
		}

		public void OnFrameAvailable (SurfaceTexture surfaceTexture)
		{
			if (FrameAvailable != null)
				FrameAvailable (this, EventArgs.Empty);
		}

		public void UpdateTexImage ()
		{
			SurfaceTexture.UpdateTexImage ();
		}

		public void GetTransformMatrix (float[] dst)
		{
			SurfaceTexture.GetTransformMatrix (dst);
		}
	}
}

