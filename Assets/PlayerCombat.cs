using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private PlayerAnimatorController _animator;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private PlayerMovementListener _movementListener;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _attackRange;
    [SerializeField] private int _damage;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private AudioClip[] _attackSound;
    [SerializeField] private bool _movePointWithMovement;
    [SerializeField] private float _maxPointDistance = 5f;
    [SerializeField] private float _pullDuration = 0.1f;
    [SerializeField,Range(0f,1f)] private float _pullInSpeed = 0.6f;
    [SerializeField] private CurveTypes _pullInCurve = CurveTypes.EaseInOutExpo;
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
        _hitColliders = new Collider[20];
        _startPosition = _attackPoint.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void OnDestroy()
    {
        if (_movement != null) _movement.OnMovement -= MoveAttackPoint;
    }

    private void Attack()
    {
        if(Time.time <= _lastAttackTime) return;
        _lastAttackTime = Time.time + _cooldown;
        DetectEnemies();
        OnAttack?.Invoke();
        for (int i = 0; i < _detectionCount; i++)
        {
            var hitCollider = _hitColliders[i];
            if (hitCollider == null) continue;
            var enemy = hitCollider.GetComponent<IDamageable>();
            enemy?.TakeDamage(_damage); // Assuming TakeDamage method exists in Enemy class
        }
        _animator.PlayStated(PlayerAnimationsNames.AttackAnimHash, _cooldown - Time.deltaTime, true);
        HelperMethods.PlaySfx(_attackSound.GetRandom());
        PullPlayerToEnemy();
    }

    private void DetectEnemies()
    {
        _detectionCount = Physics.OverlapSphereNonAlloc(_attackPoint.position, _attackRange, _hitColliders, _enemyLayer);
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
