using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : HealthController
{
    [SyncVar] public bool controlsEnabled;
    
    public int index;

    [SerializeField] [Range(1, 50)] private float moveSpeed;
    [SerializeField] [Range(1, 360)] private float rotateSpeed;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] [Range(0, 5)] private float fireRate;
    private float _lastFire;
    
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private InputAction _moveAction;
    private InputAction _fireAction;

    [SerializeField] private GameObject enemyArrowPrefab;
    private bool _spawnedEnemyArrows;
    
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        controlsEnabled = false;
    }

    public override void OnStartLocalPlayer()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _fireAction = playerInput.actions["Fire"];
        if (!(Camera.main is null)) Camera.main.GetComponent<CameraController>().target = transform;
        
        _spawnedEnemyArrows = false;
    }
    
    [Client]
    private void SpawnEnemyArrows()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Player");

        if (enemies.Length == 1) return;

        _spawnedEnemyArrows = true;
        
        foreach (GameObject enemy in enemies)
        {
            if (enemy == gameObject) continue;
            
            Debug.Log(enemy.transform.name);
            
            GameObject arrow = Instantiate(enemyArrowPrefab, transform);
            arrow.GetComponent<EnemyArrowController>().target = enemy;
        }
    }
    

    [Client]
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (!_spawnedEnemyArrows)
        {
            SpawnEnemyArrows();
        }

        if (!controlsEnabled) return;

        Vector2 moveDirection = _moveAction.ReadValue<Vector2>();
        _rigidbody2D.AddRelativeForce(moveDirection.y * moveSpeed * Time.deltaTime * Vector2.right);
        _rigidbody2D.angularVelocity = -moveDirection.x * rotateSpeed;

        if (_fireAction.triggered) CmdFire();
    }

    [Command]
    private void CmdFire()
    {
        if (Time.time - _lastFire < fireRate) return;

        _lastFire = Time.time;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);
        Physics2D.IgnoreCollision(_collider2D, bullet.GetComponent<Collider2D>());
        NetworkServer.Spawn(bullet);
    }

    public override void Damage(int damage)
    {
        health -= damage;
        if (health > 0) return;
        gameObject.SetActive(false);
        ((NetworkRoomManager) NetworkManager.singleton).PlayerDead(index);
    }
}