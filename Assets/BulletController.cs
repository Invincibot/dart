using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] [Range(0, 20)] private float bulletSpeed;
    private void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        rigidbody2D.velocity = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)) * bulletSpeed;
    }
}
