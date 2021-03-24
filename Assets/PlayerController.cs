using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;

    private Rigidbody2D _rigidbody2D;
    private InputAction _moveAction;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveAction = playerInput.actions["Move"];
    }

    private void FixedUpdate()
    {
        Vector2 moveDirection = _moveAction.ReadValue<Vector2>();
        _rigidbody2D.AddRelativeForce(new Vector2(0, moveDirection.y * moveSpeed * Time.deltaTime));
        _rigidbody2D.AddTorque(-moveDirection.x * rotateSpeed * Time.deltaTime);
    }
}
