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
	internal class ShaderProgram : IDisposable
	{
		protected int Program {
			private set;
			get;
		}

        private int _vsh;
        private int _fsh;

        public const int AttribPosition = 0;
        public const int AttribTexcoord = 1;

		public ShaderProgram(AssetManager assetMan, string vshPath, string fshPath)
        {
            Program = GL.CreateProgram();
			_vsh = LoadShader(All.VertexShader,   LoadShaderSource(assetMan, vshPath));
			_fsh = LoadShader(All.FragmentShader, LoadShaderSource(assetMan, fshPath));

            System.Diagnostics.Debug.Assert(Program != 0);
            System.Diagnostics.Debug.Assert(_vsh != 0);
            System.Diagnostics.Debug.Assert(_fsh != 0);

			GL.AttachShader(Program, _vsh);
			GL.AttachShader(Program, _fsh);

			GL.BindAttribLocation (Program, AttribPosition, "inPosition");
			GL.BindAttribLocation (Program, AttribTexcoord, "inTexcoord");
			GL.LinkProgram (Program);

			int linked;
			GL.GetProgram (Program, All.LinkStatus, out linked);

			if (linked == 0) {
				// link failed
				int length = 0;
				GL.GetProgram (Program, All.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetProgramInfoLog (Program, length, out length, log);
					Log.Debug ("ShaderProgram", "Couldn't link program: " + log.ToString ());
				}

				GL.DeleteProgram (Program);
                Program = 0;
				throw new InvalidOperationException ("Unable to link program");
			}
        }

        public void Dispose()
        {
            Release();
        }

        public void Use()
        {
            if (Program == 0)
                return;

            GL.UseProgram(Program);
        }
     
        private void Release()
        {
            if (_vsh != 0)
            {
                GL.DeleteShader(_vsh);
                _vsh = 0;
            }

            if (_fsh != 0)
            {
                GL.DeleteShader(_fsh);
                _fsh = 0;
            }

            if (Program != 0)
            {
                GL.DeleteProgram(Program);
                Program = 0;
            }
        }

		private static string LoadShaderSource(AssetManager assetMan, string path)
        {
			using (var input = assetMan.Open(path))
			using (var reader = new StreamReader(input))
            {
                return reader.ReadToEnd();
            }
        }

		private static int LoadShader (All type, string source)
		{
			int shader = GL.CreateShader (type);
			if (shader == 0)
				throw new InvalidOperationException ("Unable to create shader");

			int length = 0;
			GL.ShaderSource (shader, 1, new string [] {source}, (int[])null);
			GL.CompileShader (shader);

			int compiled = 0;
			GL.GetShader (shader, All.CompileStatus, out compiled);

			if (compiled == 0) {
				length = 0;
				GL.GetShader (shader, All.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetShaderInfoLog (shader, length, out length, log);
					Log.Debug ("ShaderProgram", "Couldn't compile shader: " + log.ToString ());
				}

				GL.DeleteShader (shader);
				throw new InvalidOperationException ("Unable to compile shader of type : " + type.ToString ());
			}

			return shader;
		}
	}
}

