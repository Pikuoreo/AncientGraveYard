using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = " PlayerStatusManegement", menuName = "ScriptableObjects/ PlayerStatusManegement")]

public class PlayerStatusManegement : ScriptableObject
{
    [Header("�v���C���[�̌��݂�HP"), SerializeField] private float _hitPoint;
    [SerializeField] private float _hitPointInitialValue;//Hp�̏����l

    [Header("�v���C���[�̍ő�HP"), SerializeField] private float _maxHitPoint ;

    [Header("�v���C���[�̌��݂�MP"), SerializeField] private float _magicPoint;
    [SerializeField] private float _magicPointInitialValue;//Mp�̏����l

    [Header("�v���C���[�̍ő�MP"), SerializeField] private float _maxMagicPoint;

    [Header("�v���C���[�̊�{�U����"), SerializeField] private float _basicPower;
    [SerializeField] private float _basicPowerInitialValue;//�v���C���[�̊�{�U���͂̏����l

    [Header("�v���C���[�̋ߐڍU����"), SerializeField] private float _meleeAttackPower;
    [SerializeField] private float _meleeAttackPowerInitialValue;//�ߐڂ̒ǉ��U���͂̏����l

    [Header("�v���C���[�̖��@�U����"), SerializeField] private float _magicattackPower;
    [SerializeField] private float _magicAttackPowerInitialValue;//���@�̒ǉ��U���͂̏����l

    [Header("�v���C���[�̃N���e�B�J����"), SerializeField] private float _criticalChance;
    [SerializeField] private float _criticalChanceInitialValue;//�N���e�B�J�����̏����l

    [Header("�v���C���[�̍ő�ړ����x"), SerializeField] private float _maxMovementSpeed;
    [SerializeField] private float _maxMovementspeedInitialValue;//�v���C���[�̍ő�ړ����x�̏����l

    [Header("�v���C���[�̃N���e�B�J���{��"), SerializeField] private float _criticalMultiple;
    [SerializeField] private float _criticalMultipleInitialValue;//�v���C���[�̃N���e�B�J���{���̏����l

    [Header("�v���C���[�̖h���"), SerializeField] private float _defensePower;
    [SerializeField] private float _defencePowerInitialValue;//�v���C���[�̖h��͂̏����l

    public void OnEnable()
    {

     _hitPoint=_hitPointInitialValue;//�v���C���[�̌��݂�HP

     _maxHitPoint=_hitPoint;//�v���C���[�̍ő�̗�

     _magicPoint=_magicPointInitialValue;//�v���C���[��MP

     _maxMagicPoint=_magicPoint;//�v���C���[�̍ő�MP

     _basicPower = _basicPowerInitialValue;//�v���C���[�̊�{�U����

     _meleeAttackPower = _meleeAttackPowerInitialValue;//�ߐڂ̒ǉ��U����

     _magicattackPower = _magicAttackPowerInitialValue;//���@�̒ǉ��U����

     _criticalChance = _criticalChanceInitialValue;//�N���e�B�J����

     _maxMovementSpeed = _maxMovementspeedInitialValue;//�v���C���[�̍ő�ړ����x

        _criticalMultiple = _criticalMultipleInitialValue;//�N���e�B�J�����̔{��

      _defensePower = _defencePowerInitialValue;//�v���C���[�̖h���
}
    public float HitPoint
    {
        get => _hitPoint;
        set => _hitPoint = value;
    }

    public float MaxHitPoint
    {
        get => _maxHitPoint;
        set => _maxHitPoint = value;
    }

    public float MagicPoint
    {
        get => _magicPoint;
        set => _magicPoint = value;
    }

    public float MaxMagicPoint
    {
        get => _maxMagicPoint;
        set => _maxMagicPoint = value;
    }

    public float BasicPower
    {
        get => _basicPower;
        set => _basicPower=value;
    }

    public float MeleeAttackPower
    {
        get => _meleeAttackPower;
        set => _meleeAttackPower = value;
    }

    public float MagicattackPower
    {
        get => _magicattackPower;
        set => _magicattackPower = value;
    }

    public float CriticalChance
    {
        get => _criticalChance;
        set => _criticalChance = value;
    }

    public float MaxMovementSpeed
    {
        get => _maxMovementSpeed;
        set => _maxMovementSpeed = value;
    }

    public float CriticalMultiple
    {
        get => _criticalMultiple;
        set => _criticalMultiple = value;
    }

    public float DefensePower
    {
        get => _defensePower;
        set => _defensePower = value;
    }
}
