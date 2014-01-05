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
	internal class SimpleShaderProgram : ShaderProgram
	{
		public int UniformTexTransform {
			get;
			private set;
		}

		public int UniformTex {
			get;
			private set;
		}

		public SimpleShaderProgram(AssetManager assetMan)
			: base(assetMan, "Resources/shaders/Simple.vsh", "Resources/shaders/Simple.fsh")
		{
			UniformTexTransform = GL.GetUniformLocation(Program, new StringBuilder("texTransform"));
			UniformTex = GL.GetUniformLocation(Program, new StringBuilder("tex"));
		}
	}
}

