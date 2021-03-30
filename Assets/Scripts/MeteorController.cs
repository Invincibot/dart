using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeteorController : NetworkBehaviour
{
    private static float unitSize = 0.01f;
    private static float startSize = 0.05f;

    [SerializeField] private int size;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GetComponent<Rigidbody2D>().simulated = true;
    }

    public float Resize(int newSize)
    {
        size = newSize;
        return ScaleToSize();
    }

    private float ScaleToSize()
    {
        float worldRadius = startSize + (size - 1) * unitSize;
        transform.localScale = worldRadius * Vector3.one;
        return worldRadius;
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            size -= 1;
            Destroy(other.gameObject);
            if (size <= 0)
            {
                Destroy(gameObject);
                return;
            }

            ScaleToSize();
        }
    }
}