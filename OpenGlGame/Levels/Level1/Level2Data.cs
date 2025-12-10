using System.Collections.Generic;
using OpenGlGame.SceneSystem;

namespace OpenGlGame.Levels.Level1;

public class Level2Data : ILevelData
{
    public GameObject[] LoadAndGetGameObjects()
    {
        List<GameObject> objects = [];

        GameObject[] transitionRoom = new TransitionRoom(false).LoadAndGetGameObjects();
        TransitionRoom.OffsetObjects(transitionRoom, TransitionRoom.InRoomSpawnPosition);
        objects.AddRange(transitionRoom);
        return objects.ToArray();
    }
}