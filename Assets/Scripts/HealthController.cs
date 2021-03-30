using Mirror;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    [SyncVar] public int health;

    [Server]
    public virtual void Damage(int damage)
    {
        health -= damage;
        if (health <= 0) NetworkServer.Destroy(gameObject);
    }
}