using System;
using UnityEngine;
using TMPro;
public class TweenFloatValue : TweenAnimationBase
{
    
    AnimationCurve _curve;

    private float _startValue, _endValue;
    private float _currentValue;

    private Action<float> _valueModifyingCallback;

    public TweenFloatValue(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(float valueToModify, float endValue, float duration, AnimationCurve curve, bool loop, Action onComplete, Action<float> valueCallback)
    {
        _startValue = valueToModify;
        _endValue = endValue;
        _totalDuration = duration;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
        _valueModifyingCallback = valueCallback;
        _elapsedTime = 0f;
    }

    public override void Play()
    {
        if(_elapsedTime >= _totalDuration) return;
        base.Play();
        _currentValue = Mathf.Lerp(_startValue, _endValue, _curve.Evaluate(_percent));
        _valueModifyingCallback?.Invoke(_currentValue);
        AnimationEnd();
    }
    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            (_startValue, _endValue) = (_endValue, _startValue);
        }
        base.AnimationEnd();
    }


}
