using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationControll : MonoBehaviour
{
    private Animator _playerAnimator = default;

    PlayerAnimations _playerAnimation = default;
    private void Start()
    {
        _playerAnimator = this.GetComponent<Animator>();
    }

    private enum PlayerAnimations
    {
        Idle,
        Walk,
        Run,
        jump,
        Damage,
        Death

    }

    private void Update()
    {
        switch (_playerAnimation)
        {
            case PlayerAnimations.Idle:
                _playerAnimator.SetInteger("State", 0);
                break;

            case PlayerAnimations.Walk:
                _playerAnimator.SetInteger("State", 1);
                break;

            case PlayerAnimations.Run:
                _playerAnimator.SetInteger("State", 2);
                break;

            case PlayerAnimations.jump:
                _playerAnimator.SetInteger("State", 3);
                break;

            case PlayerAnimations.Damage:

                break;

                case PlayerAnimations.Death:
                _playerAnimator.SetInteger("State", 7);
                break;
        }
    }

    public void AnimationChange_Idle()
    {
        _playerAnimation = PlayerAnimations.Idle;
    }

    public void AnimationChange_Walk()
    {
        _playerAnimation = PlayerAnimations.Walk;
    }


    public void AnimationChange_Run()
    {
        _playerAnimation = PlayerAnimations.Run;
    }

    public void AnimationChange_Jump()
    {
        _playerAnimation = PlayerAnimations.jump;
    }

    public void AnimationChange_Damage()
    {
        _playerAnimation = PlayerAnimations.Damage;
    }

    public void AnimationChange_Death()
    {
        _playerAnimation = PlayerAnimations.Death;
    }


    public void ReadyMeleeAttackAnimation()
    {
        _playerAnimator.SetBool("Ready", true);
        _playerAnimator.SetBool("isAttack", true);
    }

    public void StartMeleeAttackAnimation()
    {
        _playerAnimator.SetBool("Ready", false);
        _playerAnimator.SetBool("isAttack", false);
    }

    public void EndMeleeAttackAnimation()
    {
        _playerAnimator.SetBool("Ready", false);
        _playerAnimator.SetBool("isAttack", false);
    }

    public void MagicAttackAnimation()
    {
        _playerAnimator.SetTrigger("ChargeAttack2H");
    }

}
