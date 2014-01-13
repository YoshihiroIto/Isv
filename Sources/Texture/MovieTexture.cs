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
	internal class MovieTexture : VideoTextureBase, IDisposable
	{
        private MediaPlayer _mediaPlayer;

        public MovieTexture()
        {
			_mediaPlayer = new MediaPlayer ();
			_mediaPlayer.SetSurface (new Surface(SurfaceTexture));
			_mediaPlayer.SetVolume (0.0f, 0.0f);
			//_mediaPlayer.SetScreenOnWhilePlaying (true);
        }

		public new void Dispose()
		{
			_mediaPlayer.Stop ();
			_mediaPlayer.Dispose ();

			base.Dispose ();
		}

        public void Play(string filePath)
        {
			_mediaPlayer.Stop ();

			_mediaPlayer.SetDataSource (filePath);
			_mediaPlayer.Looping = true;

			_mediaPlayer.Prepare ();
			_mediaPlayer.Start ();
		}
	}
}

