using System.Collections.Generic;
using OpenGlGame.Rendering.Operations;
using OpenTK.Graphics.OpenGL;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace OpenGlGame.Rendering;

public class Renderer
{
    public const float ASPECT_RATIO = 16.0f / 9.0f;

    internal const int RENDER_PASS_OFFSET = 60;
    internal const ulong RENDER_PASS_MASK = 0x0fUL << 60;

    internal const int RENDER_TARGET_OFFSET = 56;
    internal const ulong RENDER_TARGET_MASK = 0x0fUL << 56;

    internal const int SHADER_PROGRAM_OFFSET = 44;
    internal const ulong SHADER_PROGRAM_MASK = 0x0fffUL << 44;

    internal const int BLENDING_OFFSET = 40;
    internal const ulong BLENDING_MASK = 0x0fUL << 40;

    internal const int TEXTURE_ATTACHMENT_OFFSET = 32;
    internal const ulong TEXTURE_ATTACHMENT_MASK = 0xffUL << 32;

    internal const int VERTEX_ATTRIBUTE_FORMAT_OFFSET = 24;
    internal const ulong VERTEX_ATTRIBUTE_FORMAT_MASK = 0xffUL << 24;

    internal const int UNIFORM_BUFFER_OFFSET = 12;
    internal const ulong UNIFORM_BUFFER_MASK = 0x0fffUL << 12;

    internal const int VERTEX_BUFFER_OFFSET = 0;
    internal const ulong VERTEX_BUFFER_MASK = 0x0fffUL;

    private readonly List<OperationBase> _commands = [];

    //set what's modified initially
    private ulong _lastFrameModifiedStates = SHADER_PROGRAM_MASK | VERTEX_BUFFER_MASK;

    private Matrix4 _viewProjectionMatrix = Matrix4.Identity;

    public void UpdateViewProjectionMatrix(Matrix4 viewProjectionMatrix)
    {
        _viewProjectionMatrix = viewProjectionMatrix;
    }

    public void Render(int fbWidth, int fbHeight)
    {
        _commands.Sort((OperationBase x, OperationBase y) =>
        {
            if (x.OrderKey < y.OrderKey)
            {
                return -1;
            }

            return x.OrderKey == y.OrderKey ? 0 : 1;
        });

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //TODO: this is a state, it needs to be either enabled or disabled at runtime depending on the object
        // GL.Enable(EnableCap.CullFace);

        //keep 16:9 aspect ratio
        float actualAspectRatio = fbWidth / (float)fbHeight;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (actualAspectRatio > ASPECT_RATIO)
        {
            float newWidth = fbHeight * ASPECT_RATIO;
            float offsetX = (fbWidth - newWidth) / 2.0f;
            GL.Viewport((int)offsetX, 0, (int)newWidth, fbHeight);
        }
        else if (actualAspectRatio < ASPECT_RATIO)
        {
            float newHeight = fbWidth / ASPECT_RATIO;
            float offsetY = (fbHeight - newHeight) / 2.0f;
            GL.Viewport(0, (int)offsetY, fbWidth, (int)newHeight);
        }

        ulong lastOrderKey = _lastFrameModifiedStates;

        foreach (OperationBase command in _commands)
        {
            if (command is DrawOperation drawOp)
            {
                if (drawOp.OrderKey != lastOrderKey)
                {
                    ChangeState(drawOp.OrderKey, lastOrderKey, drawOp);
                    lastOrderKey = drawOp.OrderKey;
                }

                drawOp.Draw();
            }
        }

        GL.Viewport(0, 0, fbWidth, fbHeight);
        _commands.Clear();
    }

    public void AddCommand(OperationBase command)
    {
        command.OrderKey = command.ComputeOrderKey();
        _commands.Add(command);
    }


    private void ChangeState(ulong thisOrderKey, ulong lastOrderKey, DrawOperation drawOp)
    {
        if ((thisOrderKey & SHADER_PROGRAM_MASK) != (lastOrderKey & SHADER_PROGRAM_MASK))
        {
            GL.UseProgram(drawOp.ShaderDep.GlId);

            Matrix4 viewProjMatrix = _viewProjectionMatrix;
            drawOp.ShaderDep.SetUniformMat4("viewProjection", ref viewProjMatrix);

            _lastFrameModifiedStates |= SHADER_PROGRAM_MASK;
        }

        if ((thisOrderKey & TEXTURE_ATTACHMENT_MASK) != (lastOrderKey & TEXTURE_ATTACHMENT_MASK))
        {
            //TODO: add textures to the draw operation

            // for (int i = 0; i < drawOp.Textures.Length; i++)
            // {
            //     GL.ActiveTexture(TextureUnit.Texture0 + i);
            //     GL.BindTexture(TextureTarget.Texture2D, drawOp.Textures[i].GlTextureId);
            // }
        }

        if ((thisOrderKey & VERTEX_BUFFER_MASK) != (lastOrderKey & VERTEX_BUFFER_MASK))
        {
            GL.BindVertexArray(drawOp.VertexBuffersDep.GlVao);
            _lastFrameModifiedStates |= VERTEX_BUFFER_MASK;
        }
    }
}