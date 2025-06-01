using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TweenAnimator))]
public class CameraEffects : MonoBehaviour
{
    static Camera _camera;
    static Transform _cameraTransform;
    static TweenAnimator _animator;
    static Vector3 _initialPos;
    static int _loops;
    static float _defaultSize, _startSize, _newSize, _scaleDuration;
    float _elapsedTime;
    static bool _scaling = false;

    private static float GetRandomShakeRange => Random.Range(-0.045f, 0.055f);

    private void Awake() {
        _camera = GetComponent<Camera>();
        _defaultSize = _camera.orthographicSize;
        _startSize = _defaultSize;
        _animator = GetComponent<TweenAnimator>();
        _animator.ChangeTimeScalingUsage(TweenAnimator.TimeUsage.ScaledTime);
        _cameraTransform = _camera.transform;
        _initialPos = _cameraTransform.localPosition;
        _elapsedTime = 0f;
        _scaling = false;
    }
    public static void Shake(float strength, float duration = 0.021f)
    {
        if(_animator.AnimationsQueued > 3) _animator.Clear();
        int maxIterations = 6 + (int)(30f * duration);
        Vector3 previousDir = _cameraTransform.localPosition;
        for (int i = 0; i < maxIterations; i++)
        {
            var randomSign = Mathf.Sign(Random.Range(-1, 2));
            var dir = GetNewDirection(1.5f * strength * randomSign);
            var realDuration = (duration) / (float)maxIterations;
            _animator.TweenTransformMoveTo(_cameraTransform, previousDir, dir, realDuration, CurveTypes.EaseInOut);
            previousDir = dir;
        }
    }

    private void Update() {
        if(_scaling)
        {
            var percent = _elapsedTime / _scaleDuration;
            _elapsedTime += Time.deltaTime;
            _camera.orthographicSize = Mathf.Lerp(_startSize, _newSize, TweenCurveLibrary.EaseInOutExpo.Evaluate(percent));
            if(percent >= 1f)
            {
                _scaling = false;
                _elapsedTime = 0f;
            }
        }
    }

    public static void Scale(int percentage, float duration = 0.5f)
    {
        _scaling = true;
        _startSize = _camera.orthographicSize;
        percentage = Mathf.Clamp(percentage, -100, 100);
        _newSize = _defaultSize + _defaultSize * (percentage / 100f);
        _scaleDuration = duration;
    }

    public static void ResetScale()
    {
        _scaling = true;
        _startSize = _camera.orthographicSize;
        _newSize = _defaultSize;
        _scaleDuration = 0.25f;
    }

    public static void ResetShake()
    {
        _animator.Clear();
    }

    static Vector3 GetNewDirection(float strength)
    {
        Vector3 randomDir = new Vector3(GetRandomShakeRange * strength, GetRandomShakeRange * strength, 0f) ;
        return _cameraTransform.position + randomDir;
    }
}
