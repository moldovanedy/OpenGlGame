using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenGlGame.Rendering.Resources;

public class Shader : ResourceBase, IDisposable
{
    internal Shader(uint resourceId, string vertexSource, string fragmentSource) : base(resourceId)
    {
        int sVertex = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(sVertex, vertexSource);
        GL.CompileShader(sVertex);
        bool success = IsCompiledSuccessfully(sVertex, ShaderType.VertexShader);
        if (!success)
        {
            GL.DeleteShader(sVertex);
            throw new Exception("Failed to compile vertex shader.");
        }

        int sFragment = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(sFragment, fragmentSource);
        GL.CompileShader(sFragment);
        success = IsCompiledSuccessfully(sFragment, ShaderType.FragmentShader);
        if (!success)
        {
            GL.DeleteShader(sVertex);
            GL.DeleteShader(sFragment);
            throw new Exception("Failed to compile fragment shader.");
        }

        GlId = GL.CreateProgram();
        GL.AttachShader(GlId, sVertex);
        GL.AttachShader(GlId, sFragment);

        GL.LinkProgram(GlId);
        success = IsCompiledSuccessfully(GlId, null);
        if (!success)
        {
            GL.DeleteShader(sVertex);
            GL.DeleteShader(sFragment);
            throw new Exception("Failed to link final shader program.");
        }

        //delete the shaders as they're linked into the program
        GL.DeleteShader(sVertex);
        GL.DeleteShader(sFragment);
    }

    internal int GlId { get; private set; }

    public void Dispose()
    {
        GL.DeleteProgram(GlId);
        GlId = -1;
        GC.SuppressFinalize(this);
    }

    public void SetUniformVec2(string name, ref Vector2 vector)
    {
        GL.Uniform2(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformVec2I(string name, ref Vector2i vector)
    {
        GL.Uniform2(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformVec3(string name, ref Vector3 vector)
    {
        GL.Uniform3(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformVec3I(string name, ref Vector3i vector)
    {
        GL.Uniform3(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformVec4(string name, ref Vector4 vector)
    {
        GL.Uniform4(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformVec4I(string name, ref Vector4i vector)
    {
        GL.Uniform4(GL.GetUniformLocation(GlId, name), vector);
    }

    public void SetUniformMat3(string name, ref Matrix3 matrix)
    {
        GL.UniformMatrix3(GL.GetUniformLocation(GlId, name), false, ref matrix);
    }

    public void SetUniformMat4(string name, ref Matrix4 matrix)
    {
        GL.UniformMatrix4(GL.GetUniformLocation(GlId, name), false, ref matrix);
    }

    private static bool IsCompiledSuccessfully(int shaderId, ShaderType? shaderType)
    {
        if (shaderType == null)
        {
            GL.GetProgram(shaderId, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string errorMsg = GL.GetProgramInfoLog(shaderId);
#if DEBUG
                Console.WriteLine("Shader program compilation error: " + errorMsg);
#endif

                return false;
            }
        }
        else
        {
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string errorMsg = GL.GetShaderInfoLog(shaderId);
#if DEBUG
                Console.WriteLine("Shader compilation error: " + errorMsg);
#endif

                return false;
            }
        }

        return true;
    }
}