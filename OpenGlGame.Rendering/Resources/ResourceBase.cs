namespace OpenGlGame.Rendering.Resources;

public abstract class ResourceBase
{
    protected ResourceBase(uint resourceId)
    {
        ResourceId = resourceId;
    }

    public uint ResourceId { get; }
}