namespace OpenGlGame.Rendering.Operations;

public abstract class OperationBase
{
    public ulong OrderKey { get; internal set; }

    public abstract ulong ComputeOrderKey();
}