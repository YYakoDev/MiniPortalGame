using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementListener : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerAnimatorController _animatorController;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private readonly string _animatorParameter = "Movement";
    private bool _isFlipped;
    
    public bool IsFlipped => _isFlipped;

    private void Awake()
    {
        var GO = gameObject;
        GO.CheckComponent(ref _movement);
        GO.CheckComponent(ref _spriteRenderer);
        GO.CheckComponent(ref _animatorController);
        if (_movement == null)
        {
            Debug.LogError("PlayerMovement component not found on this GameObject or its children.");
            gameObject.SetActive(false);
            return;
        }
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject or its children.");
            gameObject.SetActive(false);
            return;
        }
        if (_animatorController == null)
        {
            Debug.LogError("PlayerAnimatorController component not found on this GameObject or its children.");
            gameObject.SetActive(false);
            return;
        }
        
    }

    private void Start()
    {
        _movement.OnMovement += CheckFlip;
        _movement.OnInputDetected += CheckDirection;
    }

    private void OnDestroy()
    {
        if (_movement != null)
        {
            _movement.OnMovement -= CheckFlip;
            _movement.OnInputDetected -= CheckDirection;
        }
    }

    private void CheckFlip(Vector3 direction)
    {
        direction.Normalize();
        //Debug.Log($"Direction: {direction}");
        if (direction.x > 0.3f && _isFlipped)
        {
            Flip();
        }
        else if (direction.x < -0.3f && !_isFlipped)
        {
            Flip();
        }
        return;

        void Flip()
        {
            _isFlipped = !_isFlipped;
            var newScale = _spriteRenderer.transform.localScale;
            newScale.x *= -1; // Flip the sprite by inverting the x scale
            _spriteRenderer.transform.localScale = newScale;
        }
    }

    private void CheckDirection(Vector3 direction)
    {
        direction.Normalize();
        _animatorController.SetParameter(_animatorParameter, direction.z);
        bool idle = (direction.sqrMagnitude < 0.1f);
        var hash = idle ? PlayerAnimationsNames.IdleAnimHash : PlayerAnimationsNames.MoveAnimHash;
        _animatorController.PlayStated(hash);
        
    }
}
