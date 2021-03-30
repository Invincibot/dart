using Mirror;
using UnityEngine;

[AddComponentMenu("")]
public class MainNetworkManager : NetworkManager
{
    [Header("Boundary Walls")] [SerializeField]
    private Transform[] boundaryWalls;

    private Rect _bounds;

    [Header("Meteor Settings")] [SerializeField]
    private GameObject meteorPrefab;

    [SerializeField] [Range(0, 1000)] private float maxForce = 2;
    [SerializeField] [Range(0.01f, 1f)] private float minimumSpawningDistance = 0.05f;
    private static int maxSpawnAttempts = 100;

    [SerializeField] private int minMeteors = 5;
    [SerializeField] private int maxMeteors = 25;
    [SerializeField] private int minMeteorSize = 1;
    [SerializeField] private int maxMeteorSize = 5;

    private GameObject[] _meteors;

    public override void OnStartServer()
    {
        base.OnStartServer();

        _bounds.xMax = boundaryWalls[0].position.x;
        _bounds.yMax = boundaryWalls[1].position.y;
        _bounds.xMin = boundaryWalls[2].position.x;
        _bounds.yMin = boundaryWalls[3].position.y;

        meteorPrefab = spawnPrefabs.Find(prefab => prefab.name == "Meteor");
        SpawnMeteors();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        GameObject player = Instantiate(playerPrefab);
        Vector2 startingPosition = GetValidSpawningPosition(player.transform.localScale.magnitude);
        player.transform.position = startingPosition;
        float angle = Vector2.Angle(Vector2.right, _bounds.center - startingPosition);
        player.transform.rotation = Quaternion.Euler(0, 0, angle);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    private void SpawnMeteors()
    {
        int numMeteors = Random.Range(minMeteors, maxMeteors + 1);
        _meteors = new GameObject[numMeteors];
        for (int i = 0; i < numMeteors; i++)
        {
            GameObject meteor = Instantiate(meteorPrefab);
            float force = Random.Range(0, maxForce);
            meteor.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * force);
            int size = Random.Range(minMeteorSize, maxMeteorSize + 1);
            float worldRadius = meteor.GetComponent<MeteorController>().Resize(size);
            meteor.transform.position = GetValidSpawningPosition(worldRadius);
            NetworkServer.Spawn(meteor);
            _meteors[i] = meteor;
        }
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
        float x = Random.Range(_bounds.xMin, _bounds.xMax);
        float y = Random.Range(_bounds.yMin, _bounds.yMax);
        return new Vector2(x, y);
    }
}