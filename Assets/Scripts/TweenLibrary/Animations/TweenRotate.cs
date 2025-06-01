using UnityEngine;
using System;

public class TweenRotate : TweenAnimationBase
{
    RectTransform _rect;
    Vector3 _startingRotation, _endRotation;
    AnimationCurve _curve;
    public TweenRotate(TweenAnimator animator) : base(animator)
    {
    }

    public void Initialize(RectTransform rect, float endRotation, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _rect = rect;
        _startingRotation = _rect.localRotation.eulerAngles;
        _endRotation = _startingRotation;
        _endRotation.z = endRotation;
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
        _rect.localRotation = Quaternion.Lerp(Quaternion.Euler(_startingRotation), Quaternion.Euler(_endRotation), _curve.Evaluate(_percent));
        AnimationEnd();
    }

    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            Vector3 oldStart = _startingRotation;
            _startingRotation = _endRotation;
            _endRotation = oldStart;
        }
        base.AnimationEnd();
    }
}
