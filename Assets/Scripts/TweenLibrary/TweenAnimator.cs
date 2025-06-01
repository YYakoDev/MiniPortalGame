using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TweenAnimator : MonoBehaviour
{
    public enum TimeUsage
    {
        ScaledTime,
        UnscaledTime
    }
    [HideInInspector]public bool EnableAnimator = false;
    TweenAnimationBase _currentAnimation;
    Queue<TweenAnimationBase> _animationQueue = new();
    bool _isAnimationPlaying = false;
    [SerializeField]TimeUsage _timeUsage = TimeUsage.ScaledTime;

    TweenDestination[] _endPositionsList = new TweenDestination[0];

    //public accessor

    public TimeUsage timeUsage => _timeUsage;
    public int AnimationsQueued => _animationQueue.Count;

    // Update is called once per frame
    void Update()
    {
        if(!EnableAnimator) return;
        _currentAnimation.Play();
    }


    public virtual void Clear()
    {
        SetAnimatorState(false);
        _animationQueue.Clear();
    }

    public TweenDestination GetDestination(Vector3 endPosition)
    {
        var indexResult = CheckIfDestinationExist(endPosition);
        if(indexResult == -1)
        {
            var index = AddDestination(endPosition);
            return _endPositionsList[index];
        }else
        {
            return _endPositionsList[indexResult];
        }
    }

    int AddDestination(Vector3 endPos)
    {
        var size = _endPositionsList.Length;
        Array.Resize(ref _endPositionsList, size + 1);
        _endPositionsList[size] = new(endPos);
        return size;
    }

    int CheckIfDestinationExist(Vector3 endPos)
    {
        int indexResult = -1;
        for (int i = 0; i < _endPositionsList.Length; i++)
        {
            var destination = _endPositionsList[i];
            if (destination.RawEndPosition == endPos)
            {
                indexResult = i;
                break;
            }
        }
        return indexResult;
    }
    
    //NEVER CALL THIS ANIMATION FROM THE AWAKE FUNCTION - the canvas doesnt update correctly until the start
    public void MoveTo
    (RectTransform rectTransform, Vector3 startPosition, Vector3 endPosition, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        if(rectTransform == null)
        {
            Debug.LogError("RectTransform is null");
            return;
        }
        
        TweenMoveTo moveToAnim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);    

        moveToAnim.Initialize(rectTransform, startPosition, GetDestination(endPosition), duration, curve, loop, onComplete);
        SwitchCurrentAnimation(moveToAnim);
    }
    public void TweenMoveToBouncy
    (RectTransform rect, Vector3 startPos, Vector3 endPos, Vector3 offset, float duration, int iteration, int maxIterations,
    CurveTypes curve = CurveTypes.EaseInOut, bool loop = false, Action onBounceComplete = null)
    {
        iteration++;

        //Debug.Log($"<b> Start position: {startPos} </b>");
        if(iteration >= maxIterations)
        {
            MoveTo(rect, startPos,endPos, duration / maxIterations, onComplete: onBounceComplete);
            return;   
        }
        int sign = (iteration % 2 == 0) ? -1 : 1;
        offset *= sign / (iteration * 1.4f);
        float timeDivision = iteration * 1.75f;
        var endPosition = endPos + offset;
        MoveTo(rect, startPos, endPosition, duration / timeDivision, onComplete: () => 
        {
            startPos = endPosition;
            TweenMoveToBouncy(rect, startPos, endPos, offset, duration, iteration, maxIterations, curve, loop, onBounceComplete);
        });
    }
    
    public void TweenMoveToBouncy
    (RectTransform rect, Vector3 endPos, Vector3 offset, float duration, int iteration, int maxIterations,
        CurveTypes curve = CurveTypes.EaseInOut, bool loop = false, Action onBounceComplete = null)
    {
        TweenMoveToBouncy(rect, rect.anchoredPosition, endPos, offset, duration, iteration, maxIterations, curve, loop, onBounceComplete);
    }
    
    public void TweenMoveTransformBouncy
    (Transform target, Vector3 endPos, Vector3 offset, float duration, int iteration, int maxIterations,
    CurveTypes curve = CurveTypes.EaseInOut, bool loop = false, Action onBounceComplete = null)
    {
        iteration++;

        if(iteration >= maxIterations)
        {
            TweenTransformMoveTo(target, endPos, duration / maxIterations, onComplete: onBounceComplete);
            return;   
        }
        int sign = (iteration % 2 == 0) ? -1 : 1;
        offset *= sign / (iteration * 1.4f);
        float timeDivision = iteration * 1.75f;
        TweenTransformMoveTo(target, endPos + offset, duration / timeDivision, onComplete: () => 
        {
            TweenMoveTransformBouncy(target, endPos, offset, duration, iteration, maxIterations, curve, loop, onBounceComplete);
        });
    }

    public void TweenScaleBouncy(RectTransform rect, Vector3 endScale, Vector3 offset, float duration, int iteration, int maxIterations,
    CurveTypes curve = CurveTypes.EaseInOut, bool loop = false, Action onBounceComplete = null)
    {
        iteration++;

        if(iteration >= maxIterations)
        {
            Scale(rect, endScale, duration / maxIterations, onComplete: onBounceComplete);
            return;   
        }
        int sign = (iteration % 2 == 0) ? -1 : 1;
        offset *= sign * (1f / iteration);
        float timeDivision = iteration * 1.75f;
        Scale(rect, endScale + offset, duration / timeDivision, onComplete: () => 
        {
            TweenScaleBouncy(rect, endScale, offset, duration, iteration, maxIterations, curve, loop, onBounceComplete);
        });
    }

    /// <summary>
    /// Change opacity of an UnityEngine Image type, opacity value goes from 0 to 255.
    /// </summary>
    public void TweenImageOpacity
    (RectTransform rect, float endValue, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenImageOpacity imgOpacityAnim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);

        imgOpacityAnim.Initialize(rect, endValue, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(imgOpacityAnim);
    }


    public void Scale
    (RectTransform rectTransform, Vector3 endSize, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenScale scaleAnim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);    

        scaleAnim.Initialize(rectTransform, endSize, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(scaleAnim);
    }

    /// <summary>
    /// Change opacity of a TMPRO Text, opacity value goes from 0 to 255.
    /// </summary>
    public void TweenTextOpacity
    (TMP_Text text, float opacityEndValue, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenTextOpacity txtOpacityAnim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        txtOpacityAnim.Initialize(text, opacityEndValue, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(txtOpacityAnim); 
    }

    public void TweenTransformMoveTo
    (Transform target, Vector3 endPosition, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenTransformMoveTo anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(target, endPosition, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }
    public void TweenTransformMoveTo
    (Transform target, Vector3 startPosition, Vector3 endPosition, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenTransformMoveTo anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(target, startPosition, endPosition, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }

    public void TweenTransformRotate(Transform target, float endRotation, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenTransformRotate anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(target, endRotation, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }
    public void TweenRotate(RectTransform rect, float endRotation, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenRotate anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(rect, endRotation, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }


    public void TweenImageColor
    (RectTransform rect, Color endValue, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenImageColor anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(rect, endValue, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }
    public void TweenImageColor
    (RectTransform rect, Color initialColor, Color endValue, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenImageColor anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(rect, initialColor, endValue, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }

    public void TweenSpriteColor
    (SpriteRenderer renderer, Color endColor, float duration, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenSpriteColor anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(renderer, endColor, duration, curve, loop, onComplete);
        SwitchCurrentAnimation(anim); 
    }
    public void TweenFloatValue
        (float valueToModify, float endValue, float duration,  Action<float> valueCallback, CurveTypes curveType = CurveTypes.EaseInOut, bool loop = false, Action onComplete = null)
    {
        TweenFloatValue anim = new(this);
        AnimationCurve curve = TweenCurveLibrary.GetCurve(curveType);   

        anim.Initialize(valueToModify, endValue, duration, curve, loop, onComplete, valueCallback);
        SwitchCurrentAnimation(anim); 
    }

    protected virtual void SwitchCurrentAnimation(TweenAnimationBase animationBase)
    {
        _animationQueue.Enqueue(animationBase);
        TryPlayNext();
    }

    void TryPlayNext()
    {
        if(_animationQueue.Count > 0 && !_isAnimationPlaying)
        {
            SetAnimatorState(true);
            _currentAnimation = _animationQueue.Dequeue();
        }
    }

    void SetAnimatorState(bool state)
    {
        EnableAnimator = state;
        _isAnimationPlaying = state;
    }

    public virtual void AnimationComplete()
    {
        _isAnimationPlaying = false;
        TryPlayNext();
    }


    public void ChangeTimeScalingUsage(TimeUsage usage)
    {
        _timeUsage = usage;
    }

    
    //todo: apply the 16:9 percentages logic to draw this, the size that should be used to display this is Screen.size i think
    public static void DrawTweenPosition(Vector3 position, Vector3 size, Color color, Canvas canvas = null, bool drawOnPlay = false)
    {
        if (!drawOnPlay && Application.isPlaying) return;
        if (canvas == null)
            canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null) return;
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Gizmos.color = color;
        Debug.Log(canvasRect.sizeDelta);
        Vector2 endPosition = new
        (
            canvasRect.position.x + ((canvasRect.sizeDelta.x) * (GetPercentage(canvasRect.sizeDelta.x, position.x) / 100f) - canvasRect.sizeDelta.x / 2f) / 2f,
            canvasRect.position.y + ((canvasRect.sizeDelta.y) * (GetPercentage(canvasRect.sizeDelta.y, position.y) / 100f) - canvasRect.sizeDelta.y / 2f) / 2f
        );
        Gizmos.DrawWireCube(endPosition, size);

        float GetPercentage(float screenValue, float screenPosition)
        {
            return ((screenPosition + (screenValue / 2f)) * 100f) / screenValue;
        }
    }
}
