using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CurveTypes
{
    Linear,
    EaseInOut,
    EaseInOutExpo,
    EaseOutCirc,
    EaseInCirc,
    EaseInBounce,
    EaseInExpo
        
}
public static class TweenCurveLibrary
{

    static AnimationCurve _linearCurve, _easeInOutCurve;
    static AnimationCurve _easeInOutExpo = new AnimationCurve(
        new Keyframe(0.0067f, 0.0f, 0.0743f, 0.0743f),
        new Keyframe(0.3779f, 0.0395f, 0.5834f, 0.5834f),
        new Keyframe(0.5081f, 0.4990f, 8.6235f, 8.6235f),
        new Keyframe(0.6435f, 0.9564f, 0.3916f, 0.3916f),
        new Keyframe(0.9365f, 0.9999f, 0.0051f, 0.0051f),
        new Keyframe(1.0f, 1.0f, 0.0f, 0.0f)
    );
    static AnimationCurve _easeOutCirc = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, 4.3213f, 4.3213f),
        new Keyframe(0.2602f, 0.5880f, 1.3399f, 1.3399f),
        new Keyframe(0.7394f, 0.9949f, 0.1058f, 0.1058f),
        new Keyframe(1.0f, 1.0f, -0.0724f, -0.0724f)
    );
    static AnimationCurve _easeInCirc = new AnimationCurve(
        new Keyframe(0.0042f, 0.0f, 0.1398f, 0.1398f),
        new Keyframe(0.7909f, 0.1959f, 0.8599f, 0.8599f),
        new Keyframe(0.9097f, 0.6758f, 7.7954f, 7.7954f),
        new Keyframe(0.9813f, 0.9891f, 1.8254f, 1.8254f),
        new Keyframe(1.0f, 1f, 1.1175f, 1.1175f)
    );
    static AnimationCurve _easeInBounce = new AnimationCurve(
        new Keyframe(0.0125f, 0.1138f, 2.9474f, 2.9474f),
        new Keyframe(0.1125f, 0.2239f, -0.6159f, -0.6159f),
        new Keyframe(0.2383f, 0.2166f, 1.6490f, 1.6490f),
        new Keyframe(0.6153f, 0.2101f, 0.8601f, 0.8601f),
        new Keyframe(0.7733f, 0.6330f, -0.0514f, -0.0514f),
        new Keyframe(0.9645f, 0.9756f, 1.2943f, 1.2943f),
        new Keyframe(1.0f, 1.0f, 0.0f, 0.0f)
    );
    static AnimationCurve _easeInExpo = new AnimationCurve(
        new Keyframe(0.0f, 0.0f, 0.0f, 0.0f),
        new Keyframe(0.74840986f, 0.185112282f, 0.94229459f, 0.942294f),
        new Keyframe(0.96817296f, 0.986799359f, -0.3227976f, -0.32279f),
        new Keyframe(1.0f, 1.0f, 2.0f, 2.0f)
    );

    static bool _initialized = false;

    private static Dictionary<CurveTypes, AnimationCurve> _animationCurves = new();
    public static AnimationCurve LinearCurve => _linearCurve;
    public static AnimationCurve EaseInOutCurve => _easeInOutCurve;
    public static AnimationCurve EaseInOutExpo => _easeInOutExpo;
    public static AnimationCurve EaseOutCirc => _easeOutCirc;
    public static AnimationCurve EaseInCirc => _easeInCirc;
    public static AnimationCurve EaseInBounce => _easeInBounce;
    public static AnimationCurve EaseInExpo => _easeInExpo;


    private static void SetCurves()
    {
        _linearCurve = AnimationCurve.Linear(0,0,1,1);
        _animationCurves.Add(CurveTypes.Linear, _linearCurve);
        _easeInOutCurve = AnimationCurve.EaseInOut(0,0,1,1);
        _animationCurves.Add(CurveTypes.EaseInOut, _easeInOutCurve);
        _animationCurves.Add(CurveTypes.EaseInOutExpo, _easeInOutExpo);
        _animationCurves.Add(CurveTypes.EaseOutCirc, _easeOutCirc);
        _animationCurves.Add(CurveTypes.EaseInCirc, _easeInCirc);
        _animationCurves.Add(CurveTypes.EaseInBounce, _easeInBounce);
        _animationCurves.Add(CurveTypes.EaseInExpo, _easeInExpo);
        _initialized = true;
    }

    public static AnimationCurve GetCurve(CurveTypes _curveType)
    {
        if(!_initialized) SetCurves();
        return _animationCurves[_curveType];
    }
}
