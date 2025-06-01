using System;
using UnityEngine;

public class TweenScale : TweenAnimationBase
{
    RectTransform _rectTransform;
    Vector3 _startingSize;
    Vector3 _endScale;
    AnimationCurve _curve;


    public TweenScale(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(RectTransform rectTransform, Vector3 endScale, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _rectTransform = rectTransform;
        _startingSize = rectTransform.localScale;
        _endScale = endScale;
        _totalDuration = (duration == 0) ?  0.0001f : duration ;
        _elapsedTime = 0;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
        _animator.EnableAnimator = true;
    }
    public override void Play()
    {
        base.Play();
        _rectTransform.localScale = Vector3.Lerp(_startingSize, _endScale, _curve.Evaluate(_percent));

        
        AnimationEnd();
    }

    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            Vector3 oldStartingSize = _startingSize;
            _startingSize = _endScale;
            _endScale = oldStartingSize;
        }
        base.AnimationEnd();
    }
}
