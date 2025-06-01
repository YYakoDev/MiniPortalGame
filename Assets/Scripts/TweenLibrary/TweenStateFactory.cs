using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenStateFactory
{
    TweenAnimator _animator;

    private TweenMoveTo _moveTo;
    private TweenImageOpacity _twImgOpacity;
    private TweenScale _twScale;
    private TweenTextOpacity _twTxtOpacity;
    private TweenTransformMoveTo _twTransformMoveTo;
    private TweenImageColor _twImgColor;

    public TweenStateFactory(TweenAnimator animator)
    {
        _animator = animator;
    }
    
    public TweenMoveTo GetMoveToAnimation()
    {
        if(_moveTo == null)
        {
            _moveTo = new(_animator);
        }
        return _moveTo;
    }
    public TweenImageOpacity GetTweenImageOpacity()
    {
        if(_twImgOpacity == null)
        {
            _twImgOpacity = new(_animator);
        }
        return _twImgOpacity;
    }

    public TweenScale GetScaleAnimation()
    {
        if(_twScale == null)
        {
            _twScale = new(_animator);
        }
        return _twScale;
    }

    public TweenTextOpacity GetTextOpacityAnimation()
    {
        if(_twTxtOpacity == null) _twImgOpacity = new(_animator);
        return _twTxtOpacity;
    }
    public TweenTransformMoveTo GetTwTransformMoveTo()
    {
        if(_twTransformMoveTo == null) _twTransformMoveTo = new(_animator);
        return _twTransformMoveTo;
    }

    public TweenImageColor GetTwImgColor()
    {
        if(_twImgColor == null) _twImgColor = new(_animator);
        return _twImgColor;
    }

}
