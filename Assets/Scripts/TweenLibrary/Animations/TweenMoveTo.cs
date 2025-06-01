using System;
using UnityEngine;

public class TweenMoveTo : TweenAnimationBase
{
    RectTransform _rectTransform;
    Vector3 _startPosition, _endPosition;
    AnimationCurve _curve;
    TweenDestination _startDestination, _endDestination;

    private Vector2 _anchorOffset;
    
    bool _flip = false;

    public TweenMoveTo(TweenAnimator animator) : base(animator)
    {
    }


    public void Initialize(RectTransform rectTransform, Vector3 startPosition, TweenDestination endDestination, float duration, AnimationCurve curve, bool loop, Action onComplete)
    {
        _rectTransform = rectTransform;
        if(_rectTransform == null)
        {
            Debug.LogError("TweenMoveTo: RectTransform is null");
            return;
        }
        if(_animator == null)
        {
            Debug.LogError("TweenMoveTo: Animator is null");
            return;
        }
        if(curve == null)
        {
            Debug.LogError("TweenMoveTo: Curve is null");
            return;
        }
        _startPosition = startPosition;
        //Debug.Log($"original Start position: {_startPosition}");
        _startDestination = _animator.GetDestination(_startPosition);
        //Debug.Log("Start percentage %: " + _startDestination.EndPositionPercentage);
        _endDestination = endDestination;
        SetAnchorOffset();
        _endPosition = endDestination.GetEndPosition() + (Vector3)_anchorOffset;
        _startPosition = _startDestination.GetEndPosition();
        _startPosition += (Vector3)_anchorOffset;
        //Debug.Log($"<b> Start position: {_startPosition} </b>");
        //Debug.Log("End Percentage %: " + endDestination.EndPositionPercentage);
        //Debug.Log("End position: " + _endPosition);
        _totalDuration = (duration <= 0.0001f) ?  0.0001f : duration ;
        _elapsedTime = 0f;
        _curve = curve;
        _loop = loop;
        _onComplete = onComplete;
        _rectTransform.localPosition = _startPosition;
        _animator.EnableAnimator = true;
    }
    public override void Play()
    {
        base.Play();
        _rectTransform.localPosition = Vector3.Lerp(_startPosition, _endPosition, _curve.Evaluate(_percent));

        
        AnimationEnd();
    }

    protected override void AnimationEnd()
    {
        if(_elapsedTime >= _totalDuration && _loop)
        {
            SetAnchorOffset();
            _flip = !_flip;
            if(_flip)
            {
                _startPosition = _endDestination.GetEndPosition() + (Vector3)_anchorOffset;
                _endPosition = _startDestination.GetEndPosition();
            }else
            {
                _startPosition = _startDestination.GetEndPosition();
                _endPosition = _endDestination.GetEndPosition() + (Vector3)_anchorOffset;
            }
            
        }
        base.AnimationEnd();
    }

    //todo: the position of the animation will not be displayed correctly if the parent objects are out of the canvas resolution area,
    //todo: (weirdly when this happen, the position can be fixed by adding the parents position multiplied by 2)
    
    protected virtual void SetAnchorOffset()
    {
        /*var canvasSize = _startDestination.GetCanvasSize() * 0.5f;
        var anchorMin = _rectTransform.anchorMin;
        var anchorMax = _rectTransform.anchorMax;
        var anchors = new Vector2(1f - (anchorMax.x + anchorMin.x), 1f - (anchorMax.y + anchorMin.y));
        _anchorOffset = new Vector2(canvasSize.x * anchors.x, canvasSize.y * anchors.y);
        return;*/
        _anchorOffset = Vector2.zero;
        RectTransform rectParent = _rectTransform;
        var rootTransform = _rectTransform.transform;
        var rootCanvas = rootTransform.GetComponentInParent<Canvas>();
        if (rootCanvas != null)
        {
            rootTransform = rootCanvas.transform;
        }
        else return;
        
        
        /*var canvasSize = rootCanvas.sizeDelta;
        var endPosition = (Vector2)_endDestination.RawEndPosition;
        //Debug.Log($"<b> End position: {endPosition} </b>");
        var endPositionPercentage = new Vector2(endPosition.x / 1920f, endPosition.y / 1080f);
        //_endPosition = new Vector3((canvasSize.x * endPositionPercentage.x), (canvasSize.y * endPositionPercentage.y), 0f);*/
        
        GetRectParent(); //TODO: this is the line that was changed from the previous version, check if something breaks later
        int iterations = 0;
        while (rectParent != null)
        {
            if(iterations >= 5) break;
            iterations++;
            if (rectParent.transform == rootTransform)
            {
                break; // when reaching the canvas stopping the loop
            }
            //Debug.Log($"<b> Root Transform {rootTransform} </b>");
            //Debug.Log($"<b> Rect parent {rectParent.name} </b>");
            var offsetDirection = GetDirectionFromAnchor(rectParent.anchorMax);
            if (offsetDirection.Approximately(Directions.EightDirections[4]) && rectParent.anchorMin.Approximately(Vector3.zero)) // if anchor is top left but the min anchor is 0,0. that means is strecthed to the whole screen
            {
                //Debug.Log($"<b> Offset direction {offsetDirection} is not valid </b>");
                GetRectParent();
                continue;
            }

            if (offsetDirection.Approximately(Vector2.zero)) //this means the anchor is the center so there shouldnt be an anchor offset
            {
                GetRectParent();
                continue;
            }
            
            var screenSize = _startDestination.GetCanvasSize();
            var anchoredPos = rectParent.anchoredPosition;
            var offset = -offsetDirection * (screenSize * 0.5f);
            anchoredPos.Abs();
            anchoredPos.x *= Mathf.Sign(offsetDirection.x);
            anchoredPos.y *= Mathf.Sign(offsetDirection.y);
            //Debug.Log($"<b> Anchored position {anchoredPos} </b>");
            _anchorOffset += offset + anchoredPos;
            

            GetRectParent();
        }

        return;
        
        void GetRectParent()
        {
            rectParent = rectParent.parent?.GetComponent<RectTransform>();
        }

        Vector2 GetDirectionFromAnchor(Vector2 v)
        { 
            if (v.Approximately(_anchorDirections[0])) return Directions.EightDirections[7]; // bottom left : 0
            if (v.Approximately(_anchorDirections[1])) return Directions.EightDirections[1]; // bottom center : 1
            if (v.Approximately(_anchorDirections[2])) return Directions.EightDirections[6]; // bottom right : 2
            if (v.Approximately(_anchorDirections[3])) return Directions.EightDirections[3]; // center left : 3
            if (v.Approximately(_anchorDirections[4])) return Vector2.zero; // center center : 4
            if (v.Approximately(_anchorDirections[5])) return Directions.EightDirections[2]; // center right : 5
            if (v.Approximately(_anchorDirections[6])) return Directions.EightDirections[5]; // top left : 6 
            if (v.Approximately(_anchorDirections[7])) return Directions.EightDirections[0]; // top center : 7
            if (v.Approximately(_anchorDirections[8])) return Directions.EightDirections[4]; // top right : 8
            return Vector2.zero;
        }
    }

    private readonly Vector2[] _anchorDirections = new[]
    {
        new Vector2(0f, 0f), // bottom left : 0
        new Vector2(0.5f, 0f), // bottom center : 1
        new Vector2(1f, 0f), // bottom right : 2
        new Vector2(0f, 0.5f), // center left : 3
        new Vector2(0.5f, 0.5f), // center center : 4
        new Vector2(1f, 0.5f), // center right : 5
        new Vector2(0f, 1f), // top left : 6 
        new Vector2(0.5f, 1f), // top center : 7
        new Vector2(1f, 1f), // top right : 8
        
        // you could also the stretched anchors
    };
}
