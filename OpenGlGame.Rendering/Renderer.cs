using System;
using System.Collections.Generic;
using OpenGlGame.Data;
using OpenGlGame.Rendering.Operations;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Matrix4 = OpenTK.Mathematics.Matrix4;

namespace OpenGlGame.Rendering;

public class Renderer
{
    public readonly struct RenderFrameData
    {
        public int FbWidth { get; }
        public int FbHeight { get; }
        public Vector3 CameraWorldPosition { get; }
        public double DeltaTime { get; }

        public RenderFrameData(int fbWidth, int fbHeight, Vector3 cameraWorldPosition, double deltaTime)
        {
            FbWidth = fbWidth;
            FbHeight = fbHeight;
            CameraWorldPosition = cameraWorldPosition;
            DeltaTime = deltaTime;
        }
    }

    public readonly struct PointLightData
    {
        public Vector3 Position { get; }
        public Vector3 LightColor { get; } = new(1f);

        public float AmbientIntensity { get; } = 0.07f;
        public float DiffuseIntensity { get; } = 0.5f;
        public float SpecularIntensity { get; } = 0.2f;

        public float ConstantAttenuation { get; } = 1.0f;
        public float LinearAttenuation { get; } = 1.0f;
        public float QuadraticAttenuation { get; } = 1.0f;

        public PointLightData(
            Vector3 position,
            Vector3 lightColor,
            float ambientIntensity,
            float diffuseIntensity,
            float specularIntensity,
            float constantAttenuation,
            float linearAttenuation,
            float quadraticAttenuation)
        {
            Position = position;
            LightColor = lightColor;

            AmbientIntensity = ambientIntensity;
            DiffuseIntensity = diffuseIntensity;
            SpecularIntensity = specularIntensity;

            ConstantAttenuation = constantAttenuation;
            LinearAttenuation = linearAttenuation;
            QuadraticAttenuation = quadraticAttenuation;
        }
    }


    public const float ASPECT_RATIO = 16.0f / 9.0f;

    internal const int RENDER_PASS_OFFSET = 60;
    internal const ulong RENDER_PASS_MASK = 0x0fUL << 60;

    internal const int RENDER_TARGET_OFFSET = 56;
    internal const ulong RENDER_TARGET_MASK = 0x0fUL << 56;

    // internal const int SHADER_PROGRAM_OFFSET = 44;
    // internal const ulong SHADER_PROGRAM_MASK = 0x0fffUL << 44;
    //
    // internal const int RENDER_STATE_OFFSET = 40;
    // internal const ulong RENDER_STATE_MASK = 0x0fUL << 40;
    //
    // internal const int TEXTURE_ATTACHMENT_OFFSET = 32;
    // internal const ulong TEXTURE_ATTACHMENT_MASK = 0xffUL << 32;
    //
    // internal const int VERTEX_ATTRIBUTE_FORMAT_OFFSET = 24;
    // internal const ulong VERTEX_ATTRIBUTE_FORMAT_MASK = 0xffUL << 24;
    //
    // internal const int UNIFORM_BUFFER_OFFSET = 12;
    // internal const ulong UNIFORM_BUFFER_MASK = 0x0fffUL << 12;

    internal const int MATERIAL_OFFSET = 12;
    internal const ulong MATERIAL_MASK = 0xff_ff_ff_ff_ffUL << 12;

    internal const int VERTEX_BUFFER_OFFSET = 0;
    internal const ulong VERTEX_BUFFER_MASK = 0x0fffUL;

    internal const ulong DEFAULT_MATERIAL_STATE_MASK =
        MeshMaterial.RENDER_STATE_MASK
        | MeshMaterial.SHADER_PROGRAM_MASK
        | MeshMaterial.VERTEX_ATTRIBUTE_FORMAT_MASK;

    private readonly List<OperationBase> _commands = [];

    //set what's modified initially
    private ulong _lastFrameModifiedStates = DEFAULT_MATERIAL_STATE_MASK | VERTEX_BUFFER_MASK;

    private Matrix4 _viewProjectionMatrix = Matrix4.Identity;
    private PointLightData[] _pointLights = [];

    public void UpdateViewProjectionMatrix(Matrix4 viewProjectionMatrix)
    {
        _viewProjectionMatrix = viewProjectionMatrix;
    }

    public void UpdatePointLights(PointLightData[] pointLights)
    {
        _pointLights = pointLights;
    }

    public void Render(ref RenderFrameData frameData)
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

        //keep 16:9 aspect ratio
        float actualAspectRatio = frameData.FbWidth / (float)frameData.FbHeight;

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (actualAspectRatio > ASPECT_RATIO)
        {
            float newWidth = frameData.FbHeight * ASPECT_RATIO;
            float offsetX = (frameData.FbWidth - newWidth) / 2.0f;
            GL.Viewport((int)offsetX, 0, (int)newWidth, frameData.FbHeight);
        }
        else if (actualAspectRatio < ASPECT_RATIO)
        {
            float newHeight = frameData.FbWidth / ASPECT_RATIO;
            float offsetY = (frameData.FbHeight - newHeight) / 2.0f;
            GL.Viewport(0, (int)offsetY, frameData.FbWidth, (int)newHeight);
        }

        ulong lastOrderKey = _lastFrameModifiedStates;

        foreach (OperationBase command in _commands)
        {
            if (command is DrawOperation drawOp)
            {
                if (drawOp.OrderKey != lastOrderKey)
                {
                    ChangeState(drawOp.OrderKey, lastOrderKey, drawOp, ref frameData);
                    lastOrderKey = drawOp.OrderKey;
                }

                drawOp.MeshMaterialDep.UpdateProperties();
                drawOp.Draw();
            }
        }

        GL.Viewport(0, 0, frameData.FbWidth, frameData.FbHeight);
        _commands.Clear();
    }

    public void AddCommand(OperationBase command)
    {
        command.OrderKey = command.ComputeOrderKey();
        _commands.Add(command);
    }


    private void ChangeState(ulong thisOrderKey, ulong lastOrderKey, DrawOperation drawOp,
        ref RenderFrameData frameData)
    {
        bool hasMultipleTextures = drawOp.MeshMaterialDep.Textures?.Length > 1;

        if ((thisOrderKey & MATERIAL_MASK) != (lastOrderKey & MATERIAL_MASK) || hasMultipleTextures)
        {
            if ((thisOrderKey & MeshMaterial.RENDER_STATE_MASK) != (lastOrderKey & MeshMaterial.RENDER_STATE_MASK))
            {
                byte data = (byte)(thisOrderKey >> MeshMaterial.RENDER_STATE_OFFSET);
                //backface culling
                if (!DebuggingData.IS_CAMERA_FREE_ROAM)
                {
                    if ((data & 0b1) != 0)
                    {
                        GL.Enable(EnableCap.CullFace);
                    }
                    else
                    {
                        GL.Disable(EnableCap.CullFace);
                    }
                }
            }

            if ((thisOrderKey & MeshMaterial.SHADER_PROGRAM_MASK) != (lastOrderKey & MeshMaterial.SHADER_PROGRAM_MASK))
            {
                GL.UseProgram(drawOp.MeshMaterialDep.UsedShader.GlId);

                drawOp.MeshMaterialDep.SetProperty("viewProjection", _viewProjectionMatrix);
                drawOp.MeshMaterialDep.SetProperty("viewPos", frameData.CameraWorldPosition);

                drawOp.MeshMaterialDep.SetProperty("lightCount", _pointLights.Length);
                for (int i = 0; i < _pointLights.Length; i++)
                {
                    Vector3 diffuseColor = _pointLights[i].LightColor * _pointLights[i].DiffuseIntensity;
                    Vector3 ambientColor = diffuseColor * _pointLights[i].AmbientIntensity;
                    Vector3 specularColor = _pointLights[i].LightColor * _pointLights[i].SpecularIntensity;

                    drawOp.MeshMaterialDep.SetProperty($"lights[{i}].ambient", ambientColor);
                    drawOp.MeshMaterialDep.SetProperty($"lights[{i}].diffuse", diffuseColor);
                    drawOp.MeshMaterialDep.SetProperty($"lights[{i}].specular", specularColor);
                    drawOp.MeshMaterialDep.SetProperty($"lights[{i}].worldPos", _pointLights[i].Position);

                    drawOp.MeshMaterialDep.SetProperty(
                        $"lights[{i}].constantAttenuation",
                        _pointLights[i].ConstantAttenuation);
                    drawOp.MeshMaterialDep.SetProperty(
                        $"lights[{i}].linearAttenuation",
                        _pointLights[i].LinearAttenuation);
                    drawOp.MeshMaterialDep.SetProperty(
                        $"lights[{i}].quadraticAttenuation",
                        _pointLights[i].QuadraticAttenuation);
                }
            }

            //if the texture changed, but it changed so it actually has at least a texture OR it has multiple textures,
            // configure OpenGL for textures
            if (((thisOrderKey & MeshMaterial.TEXTURE_ATTACHMENTS_HASH_MASK) !=
                 (lastOrderKey & MeshMaterial.TEXTURE_ATTACHMENTS_HASH_MASK)
                 && (thisOrderKey & MeshMaterial.TEXTURE_ATTACHMENTS_HASH_MASK) != 0)
                || hasMultipleTextures)
            {
                if (drawOp.MeshMaterialDep.Textures == null)
                {
                    throw new Exception(
                        "Texture was changed, but the draw operation doesn't specify a texture dependency");
                }

                for (int i = 0; i < drawOp.MeshMaterialDep.Textures.Length; i++)
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + i);
                    GL.BindTexture(TextureTarget.Texture2D, drawOp.MeshMaterialDep.Textures[i].GlTextureId);
                }
            }

            _lastFrameModifiedStates |= MATERIAL_MASK;
        }

        if ((thisOrderKey & VERTEX_BUFFER_MASK) != (lastOrderKey & VERTEX_BUFFER_MASK))
        {
            GL.BindVertexArray(drawOp.VertexBuffersDep.GlVao);
            _lastFrameModifiedStates |= VERTEX_BUFFER_MASK;
        }
    }
}