using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    [SerializeField] private SpriteRenderer _renderer;
    private Material _originalMaterial;
    [SerializeField] private float _blinkDuration = 0.2f; // Duration of the blink effect
    [SerializeField] private AudioClip[] _hitSfxs;
    private void Awake()
    {
        var GO = gameObject;
        GO.CheckComponent(ref _renderer);
        if (_renderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on this GameObject or its children.");
            return;
        }
        _originalMaterial = _renderer.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        _renderer?.Blink(_originalMaterial, _blinkDuration);
        HelperMethods.PlaySfx(_hitSfxs?.GetRandom());
    }

    public void Die()
    {
        
    }
}
