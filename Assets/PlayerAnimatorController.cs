using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField]Animator _animator;
    int _currentAnimation;
    float _lockedTime;
    Dictionary<int, float> _durations;

    /*float _attackDuration;
    //bool _action;

    public float AtkDuration => _attackDuration;*/

    void Awake()
    {
        GameObject thisGO = gameObject;
        thisGO.CheckComponent(ref _animator);
    }

    private IEnumerator Start() {
        _durations = new()
        {
            {PlayerAnimationsNames.IdleAnimHash, GetAnimationDuration(PlayerAnimationsNames.IdleAnimHash)},
            {PlayerAnimationsNames.MoveAnimHash, GetAnimationDuration(PlayerAnimationsNames.MoveAnimHash)},
            {PlayerAnimationsNames.AttackAnimHash, GetAnimationDuration(PlayerAnimationsNames.AttackAnimHash)},
        };
        yield return null;
        //Debug.Log("Forward dash duration:  " + _durations[PlayerAnimationsNames.ForwardDash] + "\n Back Dash: " + _durations[PlayerAnimationsNames.BackDash]);
    }

    private void Update() {
    }

    public void PlayStated(int animationHash, float lockDuration = -0.05f, bool replay = false)
    {
        if(Time.time < _lockedTime) return;
        if(animationHash == _currentAnimation && !replay)return;
        LockAnimator(lockDuration);
        _currentAnimation = animationHash;
        _animator.Play(animationHash, -1, 0f);
    }
    public void PlayWithDuration(int animHash, bool replay = false)
    {
        _durations.TryGetValue(animHash, out var duration);
        PlayStated(animHash, duration - Time.deltaTime, replay);
    }

    void LockAnimator(float time) => _lockedTime = Time.time + time;
    public void LockAnimator() => _lockedTime = Time.time + 500f;
    public void UnlockAnimator() => _lockedTime = 0f;
    

    public void ChangeAnimator(AnimatorOverrideController animator)
    {
        _animator.runtimeAnimatorController = animator;
    }

    float GetAnimationDuration(int animHash)
    {
        var originalClips = _animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < originalClips.Length; i++)
        {
            var clip = originalClips[i];
            var originalClip = originalClips[i];
            int hash = Animator.StringToHash(originalClip.name);
            if (hash == animHash)
            {
                return clip.averageDuration;
            }
        }
        Debug.Log("Didnt found anim duration");
        return 0.2f;
    }
    
    public void SetParameter(string parameterName, float value)
    {
        if (_animator == null) return;
        _animator.SetFloat(parameterName, value);
    }
}

public static class PlayerAnimationsNames
{
    public static readonly int IdleAnimHash = Animator.StringToHash("Idle");
    public static readonly int MoveAnimHash = Animator.StringToHash("Move");
    public static readonly int AttackAnimHash = Animator.StringToHash("Attack");
}
