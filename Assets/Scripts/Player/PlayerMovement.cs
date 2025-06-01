using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Rigidbody _rb;
    [SerializeField] private Transform _mainCamera;
    [SerializeField] private InputActionReference _moveInput;
    [SerializeField] private float _speed, _angleOffset;
    [SerializeField] private bool _rotateToFaceCamera;
    private Vector3 _movement;
    private Vector3 _camForwardVector;
    private Vector3 _camRightwardVector;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        _movement.x = Input.GetAxisRaw("Horizontal");
        _movement.z = Input.GetAxisRaw("Vertical");
        

    }

    private void FixedUpdate()
    {
        _camForwardVector = _mainCamera.forward;
        _camRightwardVector = _mainCamera.right;
        _camForwardVector.y = 0;
        _camRightwardVector.y = 0f;
        _camForwardVector.Normalize();
        _camRightwardVector.Normalize();

        if (_movement.sqrMagnitude > 0.1f)
        {
            Move();
            if(_rotateToFaceCamera)
            {
                var sum = _camForwardVector + _camRightwardVector;

                var angle = Mathf.Atan2(sum.x, sum.z) * Mathf.Rad2Deg + _angleOffset;
                var newRotation = transform.rotation.eulerAngles;
                newRotation.y = angle;
                transform.rotation = Quaternion.Euler(newRotation);
            }
        }
    }

    void Move()
    {
        Vector3 direction = transform.position + (_camForwardVector * _movement.z + _camRightwardVector * _movement.x) * (Time.deltaTime * _speed); 
        _rb.MovePosition(direction);
    }
}
