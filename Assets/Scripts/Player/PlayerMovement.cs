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
    public event Action<Vector3> OnMovement, OnInputDetected; 
    //properties
    public Rigidbody Rb => _rb;
    public InputActionReference MoveInput => _moveInput;
    
    // Start is called before the first frame update
    void Start()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
            if (_rb == null)
            {
                Debug.LogError("Rigidbody component not found on this GameObject or its children.");
                gameObject.SetActive(false);
                return;
            }
        }

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main.transform;
        }

        if (_moveInput != null)
        {
            _moveInput.action.performed += StartMovement;
            _moveInput.action.canceled += StopMovement;
        }
    }

    private void OnDestroy()
    {
        if (_moveInput != null)
        {
            _moveInput.action.performed -= StartMovement;
            _moveInput.action.canceled -= StopMovement;
        }
    }


    void StartMovement(InputAction.CallbackContext context)
    {
        var input = context.ReadValue<Vector2>();
        _movement.x = input.x;
        _movement.z = input.y;
        OnInputDetected?.Invoke(_movement);
    }
    
    void StopMovement(InputAction.CallbackContext context)
    {
        _movement = Vector3.zero;
        OnInputDetected?.Invoke(_movement);
    }

    private void FixedUpdate()
    {
        if (!(_movement.sqrMagnitude > 0.1f)) return;
        _camForwardVector = _mainCamera.forward;
        _camRightwardVector = _mainCamera.right;
        _camForwardVector.y = 0;
        _camRightwardVector.y = 0f;
        _camForwardVector.Normalize();
        _camRightwardVector.Normalize();

        Move();
        if (!_rotateToFaceCamera) return;
        var sum = _camForwardVector + _camRightwardVector;
        var angle = Mathf.Atan2(sum.x, sum.z) * Mathf.Rad2Deg + _angleOffset;
        var newRotation = transform.rotation.eulerAngles;
        newRotation.y = angle;
        transform.rotation = Quaternion.Euler(newRotation);
    }

    void Move()
    {
        Vector3 dir = (_camForwardVector * _movement.z + _camRightwardVector * _movement.x) * (Time.fixedDeltaTime * _speed);
        Vector3 finalPosition = transform.position + dir;
        OnMovement?.Invoke(dir);
        _rb.MovePosition(finalPosition);
    }
}
