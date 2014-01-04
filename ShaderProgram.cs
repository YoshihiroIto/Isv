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
        private int _program;
        private int _vsh;
        private int _fsh;

		public ShaderProgram(AssetManager assetMan, string vshPath, string fshPath)
        {
            _program = GL.CreateProgram();
			_vsh = LoadShader(All.VertexShader,   LoadShaderSource(assetMan, vshPath));
			_fsh = LoadShader(All.FragmentShader, LoadShaderSource(assetMan, fshPath));

            System.Diagnostics.Debug.Assert(_program != 0);
            System.Diagnostics.Debug.Assert(_vsh != 0);
            System.Diagnostics.Debug.Assert(_fsh != 0);

			GL.AttachShader(_program, _vsh);
			GL.AttachShader(_program, _fsh);

			GL.BindAttribLocation (_program, 0, "position");
			GL.LinkProgram (_program);

			int linked;
			GL.GetProgram (_program, All.LinkStatus, out linked);
			if (linked == 0) {
				// link failed
				int length = 0;
				GL.GetProgram (_program, All.InfoLogLength, out length);
				if (length > 0) {
					var log = new StringBuilder (length);
					GL.GetProgramInfoLog (_program, length, out length, log);
					Log.Debug ("ShaderProgram", "Couldn't link program: " + log.ToString ());
				}

				GL.DeleteProgram (_program);
                _program = 0;
				throw new InvalidOperationException ("Unable to link program");
			}
        }

        public void Dispose()
        {
            Release();
        }

        public void Use()
        {
            if (_program == 0)
                return;

            GL.UseProgram(_program);
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

            if (_program != 0)
            {
                GL.DeleteProgram(_program);
                _program = 0;
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

