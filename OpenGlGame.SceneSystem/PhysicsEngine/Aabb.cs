using OpenTK.Mathematics;

namespace OpenGlGame.SceneSystem.PhysicsEngine;

/// <summary>
/// Represents an Axis-Aligned Bounding Box.
/// </summary>
public readonly struct Aabb
{
    public Vector3 Start { get; }
    public Vector3 Extents { get; }

    public Vector3 Center => Start + Extents / 2f;

    public Vector3 End => Start + Extents;

    public Aabb(Vector3 start, Vector3 extents)
    {
        Start = start;
        Extents = extents;
    }

    public Aabb(Aabb nonAlignedBox, Matrix4 transformationMatrix)
    {
        // taken and adapted from: https://gamedev.stackexchange.com/a/162824
        Vector3[] corners = new Vector3[8];
        corners[0] = nonAlignedBox.Start;
        corners[1] = nonAlignedBox.Start + new Vector3(nonAlignedBox.Extents.X, 0, 0);
        corners[2] = nonAlignedBox.Start + new Vector3(0, nonAlignedBox.Extents.Y, 0);
        corners[3] = nonAlignedBox.Start + new Vector3(0, 0, nonAlignedBox.Extents.Z);
        corners[4] = nonAlignedBox.Start + new Vector3(nonAlignedBox.Extents.X, nonAlignedBox.Extents.Y, 0);
        corners[5] = nonAlignedBox.Start + new Vector3(nonAlignedBox.Extents.X, 0, nonAlignedBox.Extents.Z);
        corners[6] = nonAlignedBox.Start + new Vector3(0, nonAlignedBox.Extents.Y, nonAlignedBox.Extents.Z);
        corners[7] = nonAlignedBox.End;

        // Transform all corners
        for (int i = 0; i < 8; i++)
        {
            corners[i] = (transformationMatrix * new Vector4(corners[i], 1.0f)).Xyz;
        }

        // Find the min and max of all transformed corners
        Vector3 min = corners[0];
        Vector3 max = corners[0];

        for (int i = 1; i < 8; i++)
        {
            min = Vector3.ComponentMin(min, corners[i]);
            max = Vector3.ComponentMax(max, corners[i]);
        }

        Start = min;
        Extents = max - min;
    }

    public static bool IsPointInside(Aabb aabb, Vector3 point)
    {
        if (aabb.Start.X > point.X)
        {
            return false;
        }

        if (aabb.End.X < point.X)
        {
            return false;
        }

        if (aabb.Start.Y > point.Y)
        {
            return false;
        }

        if (aabb.End.Y < point.Y)
        {
            return false;
        }

        if (aabb.Start.Z > point.Z)
        {
            return false;
        }

        if (aabb.End.Z < point.Z)
        {
            return false;
        }

        return true;
    }

    public static bool DoBoxesIntersect(Aabb aabb1, Aabb aabb2)
    {
        if (aabb1.End.X < aabb2.Start.X || aabb2.End.X < aabb1.Start.X)
        {
            return false;
        }

        if (aabb1.End.Y < aabb2.Start.Y || aabb2.End.Y < aabb1.Start.Y)
        {
            return false;
        }

        if (aabb1.End.Z < aabb2.Start.Z || aabb2.End.Z < aabb1.Start.Z)
        {
            return false;
        }

        return true;
    }
}