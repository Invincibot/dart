using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Telepathy;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : NetworkManager
{
    [Header("Boundary Walls")]
    [SerializeField] private Transform[] boundaryWalls;
    private Rect _bounds;
    
    [Header("Meteor Settings")]
    [SerializeField] private GameObject meteor;
    [SerializeField] [Range(0, 1000)] private float maxForce = 2;
    [SerializeField] [Range(0.01f, 1f)] private float minimumSpawningDistance = 0.05f;
    private static int maxSpawnAttempts = 100;
    
    [HideInInspector] public int minMeteors = 5;
    [HideInInspector] public int maxMeteors = 25;

    [HideInInspector] public int minMeteorSize = 1;
    [HideInInspector] public int maxMeteorSize = 5;

    public override void OnStartServer()
    {
        base.OnStartServer();
        
        _bounds.xMax = boundaryWalls[0].position.x;
        _bounds.yMax = boundaryWalls[1].position.y;
        _bounds.xMin = boundaryWalls[2].position.x;
        _bounds.yMin = boundaryWalls[3].position.y;
        
        // spawnPrefabs.Add(meteor);
        // SpawnMeteors();
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
        for (int i = 0; i < numMeteors; i++)
        {
            GameObject meteorInstance = Instantiate(meteor);
            float force = Random.Range(0, maxForce);
            meteorInstance.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * force);
            int size = Random.Range(minMeteorSize, maxMeteorSize + 1);
            float worldRadius = meteorInstance.GetComponent<MeteorController>().Resize(size);
            meteorInstance.transform.position = GetValidSpawningPosition(worldRadius);
            NetworkServer.Spawn(meteorInstance);
        }
    }
        
    private Vector2 GetValidSpawningPosition(float objectRadius)
    {
        Vector2 position = GetRandomPositionInsideBounds();
        objectRadius += minimumSpawningDistance;
        for (int spawnAttempt = 0; spawnAttempt < maxSpawnAttempts; spawnAttempt++) {
            RaycastHit2D hit = Physics2D.CircleCast(
                position,
                objectRadius,
                Vector2.zero
            );
            if (!hit)
            {
                break;
            }
                
            position = GetRandomPositionInsideBounds();
            if (spawnAttempt == maxSpawnAttempts - 1)
            {
                Debug.LogError($"Maximum spawn attempts ({maxSpawnAttempts}) exceeded");
            }
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