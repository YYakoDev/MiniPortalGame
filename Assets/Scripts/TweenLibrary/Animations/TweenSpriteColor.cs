using System;
using UnityEngine;
using TMPro;
public class TweenSpriteColor : TweenAnimationBase
{
    SpriteRenderer _renderer;
    AnimationCurve _curve;
    
    Color _startColor, _endColor;

    public TweenSpriteColor(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(SpriteRenderer renderer, Color endValue, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _renderer = renderer;
        _startColor = renderer.color;
        _endColor = endValue;
        _totalDuration = duration;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
    }

    public override void Play()
    {
        base.Play();
        _renderer.color = Color.Lerp(_startColor, _endColor, _curve.Evaluate(_percent));
        AnimationEnd();
    }
    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            Color oldStartValue = _startColor;
            _startColor = _endColor;
            _endColor = oldStartValue;
        }
        base.AnimationEnd();
    }


}
