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
	internal class CameraTexture : VideoTextureBase, IDisposable
	{
		private Android.Hardware.Camera _camera;
		private float _previewWidth = 1.0f;
		private float _previewHeight = 1.0f;

		//private readonly float[] _cameraSize = { 800, 600 };
		//private readonly float[] _cameraInvSize = { 1.0f / 800.0f, 1.0f / 600.0f };

		private readonly float[] _cameraSize = { 1920.0f, 1080.0f };
		private readonly float[] _cameraInvSize = { 1.0f / 1920.0f, 1.0f / 1080.0f };

		public float[] CameraInvSize
		{
			get {
				return _cameraInvSize;
			}

		}

		public CameraTexture ()
		{
			_camera = Android.Hardware.Camera.Open ();
			_camera.StopFaceDetection ();

			var param = _camera.GetParameters ();

			// FPS
			param.PreviewFrameRate = param.SupportedPreviewFrameRates.Last ().IntValue ();

			// オートフォーカス
			param.FocusMode = Android.Hardware.Camera.Parameters.FocusModeContinuousVideo;

			// サイズ
			{
				//var a = param.SupportedPictureSizes.ToList();
				//var b = param.SupportedPreviewSizes.ToList();

				param.SetPreviewSize ((int)_cameraSize[0], (int)_cameraSize[1]);
			}

			_previewWidth = param.PreviewSize.Width;
			_previewHeight = param.PreviewSize.Height;

			_camera.SetParameters (param);
			_camera.SetPreviewTexture (SurfaceTexture);
			_camera.StartPreview ();
		}

		private bool _isDisposed;

		public new void Dispose()
		{
			_isDisposed = true;

			_camera.StopPreview ();
			_camera.Release ();
			_camera.Dispose ();

			base.Dispose ();
		}

		public override void OnFrameAvailable (SurfaceTexture surfaceTexture)
		{
			base.OnFrameAvailable (surfaceTexture);

			if (_isDisposed)
				return;
				
			// アスペクト比補正
			{
				var param = _camera.GetParameters ();

				var width = (float)param.PreviewSize.Width;
				var height = (float)param.PreviewSize.Height;

				var scale = (_previewWidth / _previewHeight) / (width / height);

				Android.Opengl.Matrix.ScaleM (Transform, 0, scale, 1.0f, 1.0f);
			}
		}

		public void SetPreviewSize(int width, int height)
		{
			_previewWidth = width;
			_previewHeight = height;
		}
	}
}
