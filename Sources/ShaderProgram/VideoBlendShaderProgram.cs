using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using OpenTK;

namespace Isv
{
	internal class VideoBlendShaderProgram : ShaderProgram
	{
		public int UniformTexTransformA {
			get;
			private set;
		}

		public int UniformTexTransformB {
			get;
			private set;
		}

		public int UniformTexTransformC {
			get;
			private set;
		}

		public int UniformTexA {
			get;
			private set;
		}

		public int UniformTexB {
			get;
			private set;
		}

		public int UniformTexC {
			get;
			private set;
		}

		public int UniformBlendRatio {
			get;
			private set;
		}

		public int UniformCameraInvSize {
			get;
			private set;
		}

		public int UniformPoster {
			get;
			private set;
		}

		public VideoBlendShaderProgram (AssetManager assetMan)
			: base (assetMan, "Resources/shaders/VideoBlend.vsh", "Resources/shaders/VideoBlend.fsh")
		{
			UniformTexTransformA = GL.GetUniformLocation (Program, new StringBuilder ("texTransformA"));
			UniformTexTransformB = GL.GetUniformLocation (Program, new StringBuilder ("texTransformB"));
			UniformTexTransformC = GL.GetUniformLocation (Program, new StringBuilder ("texTransformC"));
			UniformTexA = GL.GetUniformLocation (Program, new StringBuilder ("texA"));
			UniformTexB = GL.GetUniformLocation (Program, new StringBuilder ("texB"));
			UniformTexC = GL.GetUniformLocation (Program, new StringBuilder ("texC"));
			UniformBlendRatio = GL.GetUniformLocation (Program, new StringBuilder ("blendRatio"));
			UniformCameraInvSize = GL.GetUniformLocation (Program, new StringBuilder ("cameraInvSize"));
			UniformPoster = GL.GetUniformLocation (Program, new StringBuilder ("poster"));
		}
	}
}

