using System;
using UnityEngine;
using UnityEngine.UI;

public class TweenImageOpacity : TweenAnimationBase
{
    Image _image;
    Color _newColor;
    float _startingValue;
    float _endValue;
    AnimationCurve _curve;
    public TweenImageOpacity(TweenAnimator animator) : base(animator)
    {
    }
    public void Initialize(RectTransform rect, float endValue, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _image = rect.GetComponent<Image>();
        if(_image == null)
        {
            Debug.LogError($"The rect {rect.name} has no image component to make the tweenimageopacity animation");
            _animator.EnableAnimator = false;
            return;
        }
        _newColor = _image.color;
        _startingValue = _newColor.a;
        _endValue = endValue / 255;
        _totalDuration = (duration == 0) ?  0.001f : duration ;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
        _animator.EnableAnimator = true;
    }

    public override void Play()
    {
        base.Play();
        _newColor.a = Mathf.Lerp(_startingValue, _endValue, _curve.Evaluate(_percent));
        _image.color = _newColor;
        AnimationEnd();
    }

    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            var oldStartValue = _startingValue;
            _startingValue = _endValue;
            _endValue = oldStartValue;
        }
        base.AnimationEnd();
    }
}
