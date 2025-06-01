using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteShadowSetter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        SetShadows();
    }

    public void SetShadows()
    {
        gameObject.CheckComponent(ref _spriteRenderer);
        if (_spriteRenderer != null)
        {
            _spriteRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SpriteShadowSetter)), CanEditMultipleObjects]
public class SpriteShadowSetterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var spriteShadowSetter = (SpriteShadowSetter)target;
        if (GUILayout.Button("Set Shadow Casting Mode"))
        {
            spriteShadowSetter.SetShadows();
        }
    }
}

#endif