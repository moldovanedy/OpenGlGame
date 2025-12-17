using OpenGlGame.SceneSystem.Character;
using OpenGlGame.SceneSystem.PhysicsEngine;

namespace OpenGlGame.SceneSystem.Components;

/// <summary>
/// Represents an object that delivers damage to the player.
/// </summary>
public class Enemy : IGameObjectComponent
{
    public string Name => nameof(Enemy);
    public GameObject? Owner { get; set; }

    public float HitsPerSecond { get; set; }

    public Aabb BoundingBox { get; set; }

    public Enemy(float hitsPerSecond, Aabb boundingBox)
    {
        HitsPerSecond = hitsPerSecond;
        BoundingBox = boundingBox;
    }

    public void EnterScene()
    {
    }

    public void ExitScene()
    {
    }

    public void Update(double deltaTime)
    {
        if (Player.Instance.GetComponent(nameof(RigidBody)) is not RigidBody playerRigidBody)
        {
            return;
        }

        if (Owner?.GetComponent(nameof(Transform3D)) is not Transform3D enemyTransform)
        {
            return;
        }

        Aabb enemyAabb = new(BoundingBox, enemyTransform.GetModelMatrix());
        if (Aabb.DoBoxesIntersect(playerRigidBody.InternalAaBoundingBox, enemyAabb))
        {
            Player.Instance.DecreaseHealth(HitsPerSecond * (float)deltaTime);
        }
    }
}