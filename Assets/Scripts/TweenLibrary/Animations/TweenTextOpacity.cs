using System;
using UnityEngine;
using TMPro;
public class TweenTextOpacity : TweenAnimationBase
{
    TMP_Text _text;
    AnimationCurve _curve;
    Color _newColor;
    float _startingOpacity;
    float _endOpacity;

    public TweenTextOpacity(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(TMP_Text text, float opacityEndValue, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _text = text;
        _newColor = _text.color;
        _startingOpacity = _text.color.a;
        _endOpacity = opacityEndValue/255f;
        _totalDuration = duration;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
    }

    public override void Play()
    {
        base.Play();
        _newColor.a = Mathf.Lerp(_startingOpacity, _endOpacity, _curve.Evaluate(_percent));
        _text.color = _newColor;

        AnimationEnd();
    }
    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            float oldStartValue = _startingOpacity;
            _startingOpacity = _endOpacity;
            _endOpacity = oldStartValue;
        }
        base.AnimationEnd();
    }


}
