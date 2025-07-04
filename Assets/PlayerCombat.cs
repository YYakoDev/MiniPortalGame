using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerAnimatorController _animator;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerMovementListener _movementListener;
    [SerializeField] private InputActionReference _attackInput;
    
    [Header("Weapon Stats")]
    [SerializeField] private float _cooldown;
    [SerializeField] private float _attackRange;
    [SerializeField] private int _damage;
    
    [Header("Enemy detection")]
    [SerializeField] private LayerMask _enemyLayer;
    
    [Header("Attack point")]
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private bool _movePointWithMovement;
    [SerializeField] private float _maxPointDistance = 5f;
    
    [Header("Pull in effect")]
    [SerializeField] private float _pullDuration = 0.1f;
    [SerializeField,Range(0f,1f)] private float _pullInSpeed = 0.6f;
    [SerializeField] private CurveTypes _pullInCurve = CurveTypes.EaseInOutExpo;
    
    [Header("Sounds")]
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip[] _attackSounds;

    [Header("Other")] 
    [SerializeField] private float _shakeDuration = 0.1f;
    [SerializeField] private float _shakeStrength = 1f;
    [SerializeField] private bool _freezeTime = true;
    [SerializeField] private float _freezeDuration = 0.02f;
    [SerializeField] private float _attackDelay = 0.1f;
    
    private float _lastAttackTime;
    private Collider[] _hitColliders;
    private int _detectionCount;
    public static event Action OnAttack;
    private Vector3 _startPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        if(_animator == null)
        {
            Debug.LogError("PlayerAnimatorController component not found on this GameObject or its children.");
            gameObject.SetActive(false);
            return;
        }

        if (_attackPoint == null)
        {
            Debug.LogWarning("Attack point transform is not assigned.");
            _attackPoint = transform;
        }
        if (_movement == null)
        {
            Debug.LogError("PlayerMovement component not found on this GameObject or its children.");
            gameObject.SetActive(false);
            return;
        }
        _movement.OnMovement += MoveAttackPoint;
        _attackInput.action.performed += Attack;
        _hitColliders = new Collider[20];
        _startPosition = _attackPoint.localPosition;
    }

    // Update is called once per frame

    private void OnDestroy()
    {
        _attackInput.action.performed -= Attack;
        if (_movement != null) _movement.OnMovement -= MoveAttackPoint;
    }

    private void Attack(InputAction.CallbackContext ctx) => Attack();
    private void Attack()
    {
        if(Time.time <= _lastAttackTime) return;
        _lastAttackTime = Time.time + _cooldown;
        DetectEnemies();
        _animator.PlayStated(PlayerAnimationsNames.AttackAnimHash, _cooldown - (Time.deltaTime * 5f), true);
        _audio?.PlayWithVaryingPitch(_attackSounds?.GetRandom());
        OnAttack?.Invoke();
        if (_detectionCount == 0) return;
        for (int i = 0; i < _detectionCount; i++)
        {
            var hitCollider = _hitColliders[i];
            if (hitCollider == null) continue;
            var enemy = hitCollider.GetComponent<IDamageable>();
            YYExtensions.Instance.ExecuteMethodAfterTime(_attackDelay, () =>
            {
                enemy?.TakeDamage(_damage);
            });
        }
        PullPlayerToEnemy();
        //FreezeTime();
        YYExtensions.Instance.ExecuteMethodAfterTime(_attackDelay, DoShake);
        YYExtensions.Instance.ExecuteMethodAfterTime(_attackDelay, FreezeTime, useUnscaledTime:true);
    }

    private void DetectEnemies()
    {
        _detectionCount = Physics.OverlapSphereNonAlloc(_attackPoint.position, _attackRange, _hitColliders, _enemyLayer);
    }

    private void DoShake()
    {
        CameraEffects.Shake(_shakeStrength, _shakeDuration);
    }

    private void FreezeTime()
    {
        if (!_freezeTime) return;
        Time.timeScale = 0f;
        YYExtensions.Instance.ExecuteMethodAfterTime(_freezeDuration, UnfreezeTime, useUnscaledTime:true);
        
        void UnfreezeTime()
        {
            Time.timeScale = 1f;
        }
    }

    private void PullPlayerToEnemy()
    {
        if(_detectionCount == 0) return;
        var startPosition = _movement.transform.position;
        var closestEnemy = HelperMethods.GetClosestCollider(_hitColliders, startPosition, _detectionCount);
        if (closestEnemy == null) return;
        _movement.enabled = false;
        var endPosition = closestEnemy.ClosestPointOnBounds(startPosition) - startPosition;
        //endPosition.Normalize();
        endPosition *= Mathf.Clamp(_pullInSpeed / _detectionCount, 0.35f, 1f);
        endPosition += startPosition;
        HelperMethods.ScaledTimeTweenAnimator.TweenFloatValue(0f, 1f, _pullDuration, f =>
        {
            if(Vector3.Distance(_movement.Rb.position, endPosition) < 0.3f)
            {
                return;
            }
            _movement.Rb?.MovePosition(Vector3.Lerp(startPosition, endPosition, f));
        }, _pullInCurve  ,onComplete: () =>
        {
            _movement.enabled = true;
        });
    }
    
    private void MoveAttackPoint(Vector3 dir)
    {
        if (_movement == null || _attackPoint == null) return;
        dir.y = 0f;
        dir.Normalize();
        var sign = _movementListener.IsFlipped ? -1f : 1f;
        var movementDirection = dir;
        var newPosition = _movement.transform.position + (_startPosition * sign) + movementDirection * (_maxPointDistance);
        _attackPoint.position = newPosition;
    }

    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null) return;
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
        Gizmos.DrawLine(_attackPoint.position, _attackPoint.position + new Vector3(1f, 0f, 1f) * _maxPointDistance);
    }
}
