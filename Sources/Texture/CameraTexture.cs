using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware;
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
	internal class CameraTexture : VideoTextureBase
	{
		private Android.Hardware.Camera _camera;

		public CameraTexture ()
		{
			_camera = Android.Hardware.Camera.Open ();
			_camera.StopFaceDetection ();

			var param = _camera.GetParameters ();

			// FPS
			param.PreviewFrameRate = param.SupportedPreviewFrameRates.Last ().IntValue ();

			// オートフォーカス
			param.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousVideo;

			#if false
			// サイズ
			{
				var a = param.SupportedPictureSizes.ToList();
				var b = param.SupportedPreviewSizes.ToList();

				param.SetPreviewSize (176, 144);
			}
			#endif

			_camera.SetParameters (param);
			_camera.SetPreviewTexture (SurfaceTexture);
			_camera.StartPreview ();
		}

		public override void OnFrameAvailable (SurfaceTexture surfaceTexture)
		{
			base.OnFrameAvailable (surfaceTexture);

			// 180度回転
			Android.Opengl.Matrix.RotateM (Transform, 0, 180.0f, 0.0f, 0.0f, 1.0f);
			Android.Opengl.Matrix.TranslateM (Transform, 0, -1.0f, -1.0f, 0.0f);
		}
	}
}
