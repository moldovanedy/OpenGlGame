using System;
using System.Collections.Generic;
using OpenGlGame.Rendering.Resources;
using Silk.NET.Assimp;
using PrimitiveType = OpenTK.Graphics.OpenGL.PrimitiveType;
using Texture = OpenGlGame.Rendering.Resources.Texture;

namespace OpenGlGame.Rendering;

public static unsafe class ModelLoaderUtility
{
    private static readonly Assimp AssimpCtx = Assimp.GetApi();

    /// <summary>
    /// Given the path to a .glb file, will load all the meshes, as well as all textures (only if those are in the
    /// same directory), returning the number of loaded vertex buffers.
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="vertAttribDescriptors">The vertex attribute descriptors for all the contained meshes.</param>
    /// <example>
    /// Loads model /Assets/Models/cube_01/cube_01.glb; will use texture tex_diffuse01.png with the ID of
    /// "/assets/models/cube_01/tex_diffuse01.png"; will return the ID of the created vertex buffer:
    /// "/assets/models/cube_01/cube_01.glb.vert01".
    /// </example>
    /// <returns></returns>
    public static int LoadModelFromFile(string assetPath, VertexAttribDescriptors vertAttribDescriptors)
    {
        Scene* entireScene = AssimpCtx.ImportFile(
            AppDomain.CurrentDomain.BaseDirectory + assetPath,
            (uint)(PostProcessSteps.Triangulate | PostProcessSteps.GenerateNormals));

        if ((entireScene->MFlags & Assimp.SceneFlagsIncomplete) != 0)
        {
            throw new Exception("Assimp: Scene is incomplete");
        }

        int meshCount = 0;
        ProcessNode(entireScene->MRootNode, entireScene, vertAttribDescriptors, assetPath, ref meshCount);
        return meshCount;
    }

    private static void ProcessNode(
        Node* modelNode,
        Scene* parentScene,
        VertexAttribDescriptors vertAttribDescriptors,
        string assetPath,
        ref int meshCount)
    {
        for (int i = 0; i < parentScene->MNumMeshes; i++)
        {
            ProcessMesh(parentScene->MMeshes[i], parentScene, vertAttribDescriptors, assetPath, ref meshCount);
        }


        for (int i = 0; i < modelNode->MNumChildren; i++)
        {
            ProcessNode(modelNode->MChildren[i], parentScene, vertAttribDescriptors, assetPath, ref meshCount);
        }
    }

    private static void ProcessMesh(
        Mesh* mesh,
        Scene* parentScene,
        VertexAttribDescriptors vertAttribDescriptors,
        string assetPath,
        ref int meshCount)
    {
        List<float> vertexData = new((int)mesh->MNumVertices);
        List<uint> indices = new((int)mesh->MNumVertices * 3);

        for (uint i = 0; i < mesh->MNumVertices; i++)
        {
            vertexData.Add(mesh->MVertices[i].X);
            vertexData.Add(mesh->MVertices[i].Y);
            vertexData.Add(mesh->MVertices[i].Z);

            if (mesh->MNormals != null)
            {
                vertexData.Add(mesh->MNormals[i].X);
                vertexData.Add(mesh->MNormals[i].Y);
                vertexData.Add(mesh->MNormals[i].Z);
            }

            if (mesh->MTextureCoords[0] != null)
            {
                vertexData.Add(mesh->MTextureCoords[0][i].X);
                vertexData.Add(mesh->MTextureCoords[0][i].Y);
            }
        }

        for (uint i = 0; i < mesh->MNumFaces; i++)
        {
            Face face = mesh->MFaces[i];
            for (uint j = 0; j < face.MNumIndices; j++)
            {
                indices.Add(face.MIndices[j]);
            }
        }

        Material* mat = parentScene->MMaterials[mesh->MMaterialIndex];
        int indexOfSlash = assetPath.LastIndexOf('/');
        if (indexOfSlash == -1)
        {
            throw new Exception("Model loader: could not find the model's directory");
        }

        string dirPath = assetPath.Substring(0, indexOfSlash + 1);

        // ProcessTextures(mat, TextureType.BaseColor, dirPath + "tex_albedo.png");
        ProcessTextures(mat, TextureType.Diffuse, dirPath + "tex_diffuse.png");
        // ProcessTextures(mat, TextureType.Specular, dirPath + "tex_specular.png");
        // ProcessTextures(mat, TextureType.Normals, dirPath + "tex_normal.png");

        ResourceManager.Instance.CreateVertexBuffers(
            assetPath + ".vert" + (meshCount + 1).ToString("D2"),
            PrimitiveType.Triangles,
            vertAttribDescriptors,
            vertexData.ToArray(),
            (int)mesh->MNumVertices,
            indices.ToArray(),
            indices.Count);
        meshCount++;
    }

    private static void ProcessTextures(Material* mat, TextureType textureType, string textureBaseId)
    {
        for (uint i = 0; i < AssimpCtx.GetMaterialTextureCount(mat, textureType); i++)
        {
            AssimpString filePath;
            TextureMapMode mapMode;
            uint x = 0, y = 0;

            AssimpCtx.GetMaterialTexture(
                mat,
                textureType,
                i,
                &filePath,
                null,
                ref x,
                null,
                null,
                &mapMode,
                ref y);

            if (filePath.AsString.Contains('/'))
            {
                throw new Exception("Model loader: texture file name contains a relative or absolute path");
            }

            //TODO: figure out how to use this for X and Y separately
            var format = new Texture.TextureFormat();
            switch (mapMode)
            {
                default:
                case TextureMapMode.Clamp:
                    format.XWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;
                    break;
                case TextureMapMode.Decal:
                    format.XWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder;
                    break;
                case TextureMapMode.Mirror:
                    format.XWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat;
                    break;
                case TextureMapMode.Wrap:
                    format.XWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
                    break;
            }

            switch (mapMode)
            {
                default:
                case TextureMapMode.Clamp:
                    format.YWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Clamp;
                    break;
                case TextureMapMode.Decal:
                    format.YWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder;
                    break;
                case TextureMapMode.Mirror:
                    format.YWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat;
                    break;
                case TextureMapMode.Wrap:
                    format.YWrapMode = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
                    break;
            }

            textureBaseId = textureBaseId.Insert(
                textureBaseId.LastIndexOf('.'), (i + 1).ToString("D2"));
            Texture? texResult = ResourceManager.Instance.GetOrCreateTexture(textureBaseId, format);
            if (texResult == null)
            {
                throw new Exception("Model loader: Could not read texture file with ID: " + textureBaseId);
            }
        }
    }
}