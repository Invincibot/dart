using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform[] boundaryWalls;
    private Rect _bounds;
    
    [SerializeField] private GameObject meteor;
    [SerializeField] [Range(0, 1000)] private float maxForce = 2;
    [SerializeField] [Range(0.01f, 1f)] private float minimumDistanceFromPlayer = 0.05f;
    private static int maxSpawnAttempts = 100;
    
    [HideInInspector] public int minMeteors = 5;
    [HideInInspector] public int maxMeteors = 25;

    [HideInInspector] public int minMeteorSize = 1;
    [HideInInspector] public int maxMeteorSize = 5;

    private void Start()
    {
        _bounds.xMax = boundaryWalls[0].position.x;
        _bounds.yMax = boundaryWalls[1].position.y;
        _bounds.xMin = boundaryWalls[2].position.x;
        _bounds.yMin = boundaryWalls[3].position.y;
        int numMeteors = Random.Range(minMeteors, maxMeteors + 1);
        for (int i = 0; i < numMeteors; i++)
        {
            GameObject meteorInstance = Instantiate(meteor, Vector3.zero, Quaternion.identity);
            float force = Random.Range(0, maxForce);
            meteorInstance.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * force);
            int size = Random.Range(minMeteorSize, maxMeteorSize + 1);
            float worldRadius = meteorInstance.GetComponent<MeteorController>().Resize(size);

            float raycastRadius = worldRadius + minimumDistanceFromPlayer;
            Vector2 meteorPosition = GetRandomPositionInsideBounds();
            for (int spawnAttempt = 0; spawnAttempt < maxSpawnAttempts; spawnAttempt++) {
                RaycastHit2D hit = Physics2D.CircleCast(
                    meteorPosition,
                    raycastRadius,
                    Vector2.zero
                );
                if (!hit)
                {
                    break;
                }
                
                meteorPosition = GetRandomPositionInsideBounds();
                if (spawnAttempt == maxSpawnAttempts - 1)
                {
                    Debug.LogError($"Maximum spawn attempts ({maxSpawnAttempts}) exceeded");
                    Debug.Break();
                }
            }

            meteorInstance.transform.position = meteorPosition;
        }
    }

    private Vector2 GetRandomPositionInsideBounds()
    {
        float x = Random.Range(_bounds.xMin, _bounds.xMax);
        float y = Random.Range(_bounds.yMin, _bounds.yMax);
        return new Vector2(x, y);
    }
}