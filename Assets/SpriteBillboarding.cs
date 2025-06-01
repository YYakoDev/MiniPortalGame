using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBillboarding : MonoBehaviour
{
    [SerializeField] private bool _rotateToFaceCamera = true;
    [SerializeField]private Transform _spriteTransform;
    private Camera _mainCamera;
    private Transform _cameraTransform;
    [SerializeField] private float _angleOffset = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogWarning("Main camera not found. Please ensure there is a camera tagged as 'MainCamera'.");
            gameObject.SetActive(false);
            return;
        }
        _cameraTransform = _mainCamera.transform;
        if (_spriteTransform == null)
        {
            _spriteTransform = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_rotateToFaceCamera) return;
        var newRot = _spriteTransform.rotation.eulerAngles;
        var directionToCamera = _cameraTransform.position - _spriteTransform.position;
        directionToCamera.y = 0; // Keep the rotation on the horizontal plane
        var sum = -directionToCamera.normalized;
        var angle = Mathf.Atan2(sum.x, sum.z) * Mathf.Rad2Deg + _angleOffset;
        newRot.y = angle;
        _spriteTransform.rotation = Quaternion.Euler(newRot);
    }
}
