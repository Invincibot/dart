using System;
using Mirror;
using Mono.CecilX;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] [Range(1, 50)] private float moveSpeed;
    [SerializeField] [Range(1, 360)] private float rotateSpeed;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private InputAction _moveAction;
    private InputAction _fireAction;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    public override void OnStartAuthority()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _fireAction = playerInput.actions["Fire"];
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        Vector2 moveDirection = _moveAction.ReadValue<Vector2>();
        _rigidbody2D.AddRelativeForce(moveDirection.y * moveSpeed * Time.deltaTime * Vector2.right);
        _rigidbody2D.angularVelocity = -moveDirection.x * rotateSpeed;

        if (_fireAction.triggered)
        {
            CmdFire();
        }
    }
    
    [Command]
    private void CmdFire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        Physics2D.IgnoreCollision(_collider2D, bullet.GetComponent<Collider2D>());
        NetworkServer.Spawn(bullet);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
