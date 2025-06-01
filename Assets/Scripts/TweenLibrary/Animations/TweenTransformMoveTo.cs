using System;
using UnityEngine;

public class TweenTransformMoveTo : TweenAnimationBase
{
    Transform _transform;
    Vector3 _startPosition;
    Vector3 _endPosition;
    AnimationCurve _curve;


    public TweenTransformMoveTo(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(Transform transform, Vector3 endPosition, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        InitLogic(transform, transform.localPosition, endPosition, duration, curve, loop, onComplete);
    }
    public void Initialize(Transform transform, Vector3 startPosition, Vector3 endPosition, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        InitLogic(transform, startPosition, endPosition, duration, curve, loop, onComplete);
    }

    void InitLogic(Transform transform, Vector3 startPosition, Vector3 endPosition, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _transform = transform;
        _startPosition = startPosition;
        _endPosition = endPosition;
        _totalDuration = (duration <= 0.001f) ?  0.0001f : duration ;
        _elapsedTime = 0;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
        _animator.EnableAnimator = true;
    }

    public override void Play()
    {
        base.Play();
        _transform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _curve.Evaluate(_percent));

        
        AnimationEnd();
    }

    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            (_startPosition, _endPosition) = (_endPosition, _startPosition);
        }
        base.AnimationEnd();
    }
}