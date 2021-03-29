using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] [Range(1, 50)] private float moveSpeed;
    [SerializeField] [Range(1, 360)] private float rotateSpeed;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] [Range(0, 10)] private float bulletSpeed;
    
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private InputAction _moveAction;
    private InputAction _fireAction;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
        
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
        _fireAction = playerInput.actions["Fire"];
    }

    private void Update()
    {
        if (_fireAction.triggered)
        {
            GameObject _bullet = Instantiate(bullet, firePoint.position, transform.rotation);
            Rigidbody2D _bulletRigidbody2D = _bullet.GetComponent<Rigidbody2D>();
            float _rotation = _rigidbody2D.rotation * Mathf.Deg2Rad;
            Vector2 _direction = new Vector2( Mathf.Cos(_rotation), Mathf.Sin(_rotation));
            _bulletRigidbody2D.velocity = _direction * bulletSpeed;
            _bulletRigidbody2D.rotation = _rigidbody2D.rotation;
            Physics2D.IgnoreCollision(_collider2D, _bullet.GetComponent<Collider2D>());
        }
    }

    private void FixedUpdate()
    {
        Vector2 moveDirection = _moveAction.ReadValue<Vector2>();
        _rigidbody2D.AddRelativeForce(moveDirection.y * moveSpeed * Time.deltaTime * Vector2.right);
        _rigidbody2D.angularVelocity = -moveDirection.x * rotateSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
