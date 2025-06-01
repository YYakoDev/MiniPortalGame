using System;
using UnityEngine;
public abstract class TweenAnimationBase
{
    protected TweenAnimator _animator;
    protected float _elapsedTime = 0;
    protected float _totalDuration;
    protected float _percent;
    protected bool _loop = false;
    protected Action _onComplete;

    public bool Loop => _loop;
    public float Percent => _percent;

    public TweenAnimationBase(TweenAnimator animator)
    {
        _animator = animator;
    }

    public virtual void Play()
    {
        if(_elapsedTime >= _totalDuration) return;
        if(_animator.timeUsage == TweenAnimator.TimeUsage.ScaledTime) _elapsedTime += Time.deltaTime;
        else if(_animator.timeUsage == TweenAnimator.TimeUsage.UnscaledTime) _elapsedTime += Time.unscaledDeltaTime;
        _percent = _elapsedTime / _totalDuration;
    }

    protected virtual void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration)
        {
            _animator.EnableAnimator = _loop;
            _elapsedTime = 0;
            
            _onComplete?.Invoke();
            if(!_loop)_animator.AnimationComplete();
        }
    }

}
