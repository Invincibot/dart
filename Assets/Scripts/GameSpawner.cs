using Mirror;
using UnityEngine;

public class GameSpawner : NetworkBehaviour
{
    public Transform[] boundaryWalls;
    [SerializeField] private GameObject boundaryWallPrefab;
    [HideInInspector] public Rect bounds;

    [Header("Player Settings")] [SerializeField]
    private GameObject playerPrefab;

    [Header("Meteor Settings")] [SerializeField]
    private GameObject meteorPrefab;

    [SerializeField] [Range(0, 1000)] private float maxForce = 2;
    [SerializeField] [Range(0.01f, 1f)] private float minimumSpawningDistance = 0.05f;
    private static int maxSpawnAttempts = 100;

    [SerializeField] private int minMeteors = 5;
    [SerializeField] private int maxMeteors = 25;
    [SerializeField] private int minMeteorSize = 1;
    [SerializeField] private int maxMeteorSize = 5;

    public void SpawnBounds()
    {
        foreach (Transform boundaryWall in boundaryWalls)
        {
            GameObject wall = Instantiate(boundaryWallPrefab, boundaryWall.position, boundaryWall.rotation);
            NetworkServer.Spawn(wall);
        }
    }

    [Server]
    public GameObject SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab);
        Vector2 startingPosition = GetValidSpawningPosition(player.transform.localScale.magnitude);
        player.transform.position = startingPosition;
        float angle = Vector2.Angle(Vector2.right, bounds.center - startingPosition);
        player.transform.rotation = Quaternion.Euler(0, 0, angle);
        return player;
    }

    [Server]
    public void SpawnMeteors()
    {
        if (!NetworkServer.active) return;

        int numMeteors = Random.Range(minMeteors, maxMeteors + 1);
        for (int i = 0; i < numMeteors; i++)
        {
            SpawnMeteor();
        }
    }

    private void SpawnMeteor()
    {
        int size = Random.Range(minMeteorSize, maxMeteorSize + 1);
        float worldRadius = MeteorController.GetMeteorRadius(size);
        GameObject meteor = Instantiate(meteorPrefab, GetValidSpawningPosition(worldRadius), Quaternion.identity);
        float force = Random.Range(0, maxForce);
        meteor.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * force);
        NetworkServer.Spawn(meteor);
    }

    private Vector2 GetValidSpawningPosition(float objectRadius)
    {
        Vector2 position = GetRandomPositionInsideBounds();
        objectRadius += minimumSpawningDistance;
        for (int spawnAttempt = 0; spawnAttempt < maxSpawnAttempts; spawnAttempt++)
        {
            RaycastHit2D hit = Physics2D.CircleCast(
                position,
                objectRadius,
                Vector2.zero
            );
            if (!hit) break;

            position = GetRandomPositionInsideBounds();
            if (spawnAttempt == maxSpawnAttempts - 1)
                Debug.LogError($"Maximum spawn attempts ({maxSpawnAttempts}) exceeded");
        }

        return position;
    }

    private Vector2 GetRandomPositionInsideBounds()
    {
        float x = Random.Range(bounds.xMin, bounds.xMax);
        float y = Random.Range(bounds.yMin, bounds.yMax);
        return new Vector2(x, y);
    }
}