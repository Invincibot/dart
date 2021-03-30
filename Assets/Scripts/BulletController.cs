using System;
using Mirror;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    [SerializeField] [Range(0, 20)] private float bulletSpeed;
    [SerializeField] [Range(0, 5)] private float lifetime;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), lifetime);
    }

    private void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        rigidbody2D.velocity = new Vector2((float) Math.Cos(angle), (float) Math.Sin(angle)) * bulletSpeed;
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        DestroySelf();
        HealthController healthController = other.GetComponent<HealthController>();
        if (healthController != null) healthController.Damage(1);
    }
}
