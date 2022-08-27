using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    private Controls _controls;
    private Vector2 _moveInput;
    private Rigidbody2D _rbody;
    

    PlayerInput playerInput;
    // Start is called before the first frame update
    void Awake()
    {
        _controls = new Controls();
        _rbody = GetComponent<Rigidbody2D>();
        if (_rbody == null)
        {
            Debug.Log("Rigidbody is null");
        }
    }
    private void OnEnable()
    {
        _controls.Player.Enable();
    }
    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _moveInput = _controls.Player.Move.ReadValue<Vector2>();
        _rbody.velocity = _moveInput * _speed;
    }
}
