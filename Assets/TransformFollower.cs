using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformFollower : MonoBehaviour
{
    [SerializeField] private bool _follow;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private CameraFollowMode _followMode = CameraFollowMode.LateUpdate;
    [SerializeField] private float _smoothSpeed = 0.125f;
    Transform _cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = transform;
    }

    private void FixedUpdate()
    {
        Follow();
    }

    void Follow()
    {
        if (!_follow || _targetTransform == null)
            return;
        var deltaTime = _followMode == CameraFollowMode.FixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        var targetPosition = (_targetTransform.position + _offset);
        var direction = targetPosition - transform.position;
        
        //_cameraTransform.position += direction * (_smoothSpeed * deltaTime);
        _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, targetPosition, _smoothSpeed * deltaTime);
        
        
    }
}

public enum CameraFollowMode
{
    FixedUpdate,
    LateUpdate,
}
