using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    private static float unitSize = 0.01f;
    private static float startSize = 0.05f;
    
    [SerializeField] private int size;

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
