using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = " PlayerStatusManegement", menuName = "ScriptableObjects/ PlayerStatusManegement")]

public class PlayerStatusManegement : ScriptableObject
{
    [Header("プレイヤーの現在のHP"), SerializeField] private float _hitPoint;
    [SerializeField] private float _hitPointInitialValue;//Hpの初期値

    [Header("プレイヤーの最大HP"), SerializeField] private float _maxHitPoint ;

    [Header("プレイヤーの現在のMP"), SerializeField] private float _magicPoint;
    [SerializeField] private float _magicPointInitialValue;//Mpの初期値

    [Header("プレイヤーの最大MP"), SerializeField] private float _maxMagicPoint;

    [Header("プレイヤーの基本攻撃力"), SerializeField] private float _basicPower;
    [SerializeField] private float _basicPowerInitialValue;//プレイヤーの基本攻撃力の初期値

    [Header("プレイヤーの近接攻撃力"), SerializeField] private float _meleeAttackPower;
    [SerializeField] private float _meleeAttackPowerInitialValue;//近接の追加攻撃力の初期値

    [Header("プレイヤーの魔法攻撃力"), SerializeField] private float _magicattackPower;
    [SerializeField] private float _magicAttackPowerInitialValue;//魔法の追加攻撃力の初期値

    [Header("プレイヤーのクリティカル率"), SerializeField] private float _criticalChance;
    [SerializeField] private float _criticalChanceInitialValue;//クリティカル率の初期値

    [Header("プレイヤーの最大移動速度"), SerializeField] private float _maxMovementSpeed;
    [SerializeField] private float _maxMovementspeedInitialValue;//プレイヤーの最大移動速度の初期値

    [Header("プレイヤーのクリティカル倍率"), SerializeField] private float _criticalMultiple;
    [SerializeField] private float _criticalMultipleInitialValue;//プレイヤーのクリティカル倍率の初期値

    [Header("プレイヤーの防御力"), SerializeField] private float _defensePower;
    [SerializeField] private float _defencePowerInitialValue;//プレイヤーの防御力の初期値

    public void OnEnable()
    {

     _hitPoint=_hitPointInitialValue;//プレイヤーの現在のHP

     _maxHitPoint=_hitPoint;//プレイヤーの最大体力

     _magicPoint=_magicPointInitialValue;//プレイヤーのMP

     _maxMagicPoint=_magicPoint;//プレイヤーの最大MP

     _basicPower = _basicPowerInitialValue;//プレイヤーの基本攻撃力

     _meleeAttackPower = _meleeAttackPowerInitialValue;//近接の追加攻撃力

     _magicattackPower = _magicAttackPowerInitialValue;//魔法の追加攻撃力

     _criticalChance = _criticalChanceInitialValue;//クリティカル率

     _maxMovementSpeed = _maxMovementspeedInitialValue;//プレイヤーの最大移動速度

        _criticalMultiple = _criticalMultipleInitialValue;//クリティカル時の倍率

      _defensePower = _defencePowerInitialValue;//プレイヤーの防御力
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
