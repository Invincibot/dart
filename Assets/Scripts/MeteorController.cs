using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MeteorController : HealthController
{
    private static float unitSize = 0.01f;
    private static float startSize = 0.05f;

    public override void OnStartServer()
    {
        base.OnStartServer();

        GetComponent<Rigidbody2D>().simulated = true;
    }

    public float Resize(int newSize)
    {
        health = newSize;
        return ScaleToSize();
    }

    private float ScaleToSize()
    {
        float worldRadius = startSize + (health - 1) * unitSize;
        transform.localScale = worldRadius * Vector3.one;
        return worldRadius;
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);
        if (health > 0) ScaleToSize();
    }
}