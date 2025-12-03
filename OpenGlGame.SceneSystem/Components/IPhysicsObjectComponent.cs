using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem.Components;

public interface IPhysicsObjectComponent : IGameObjectComponent
{
    public Aabb BoundingBox { get; set; }

    void PhysicsUpdate();
}