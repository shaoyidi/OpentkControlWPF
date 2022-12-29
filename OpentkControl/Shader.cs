using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpentkControl
{
    class Shader : IDisposable
    {
        int Handle;
        //private int _handle;

        //public int Handle
        //{
        //    get { return _handle; }
        //    set { _handle = value; }
        //}



        public Shader(string vertexPath, string fragmentPath)
        {
            int VertexShader, FragmentShader;
            //读取shader内容
            string VertexShaderSource = File.ReadAllText(vertexPath);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);

            //绑定shader
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            //编译shader        
            int vertexShaderCompileSatus, fragmentShaderCompileStatus;

            GL.CompileShader(VertexShader);
            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out vertexShaderCompileSatus);
            if (vertexShaderCompileSatus == 0)
            {
                string infoLog = GL.GetShaderInfoLog(VertexShader);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(FragmentShader);
            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus == 0)
            {
                string infoLog = GL.GetShaderInfoLog(FragmentShader);
                Console.WriteLine(infoLog);
            }

            //Our individual shaders are compiled,
            //but to actually use them, we have to link them together into a program that can be run on the GPU.
            //This is what we mean when we talk about a "shader" from here on out. We do that like this:
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            //Before we leave the constructor, we should do a little cleanup.
            //The individual vertex and fragment shaders are useless now that they've been linked;
            //the compiled data is copied to the shader program when you link it.
            //You also don't need to have those individual shaders attached to the program;
            //let's detach and then delete them.
            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);
        }

        public void SetMatrix4(string argName, Matrix4 matrix)
        {
            int location = GL.GetUniformLocation(Handle, argName);

            GL.UniformMatrix4(location, true, ref matrix);
        }
        public void SetVector3(string argName, Vector3 vector3)
        {
            int location = GL.GetUniformLocation(Handle, argName);

            GL.Uniform3(location, vector3.X, vector3.Y, vector3.Z);
        }

        public void SetFloat(string argName, float vector3)
        {
            int location = GL.GetUniformLocation(Handle, argName);

            GL.Uniform1(location, vector3);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        //非托管资源回收
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue == false)
            {
                //释放非托管资源
                //GC在debug进行回收时会报错
                GL.DeleteProgram(Handle);
                if (disposing)
                {
                    //释放托管资源
                }
                disposedValue = true;
            }

        }

        ~Shader()
        {
            //GC调用时不用手动释放托管资源
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
