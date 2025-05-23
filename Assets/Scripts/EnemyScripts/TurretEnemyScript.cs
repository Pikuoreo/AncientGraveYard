using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TurretEnemyScript : MonoBehaviour
{
    [Header("発射する弾"),SerializeField] private GameObject _burret = default;
    [Header("Mp回復アイテム"), SerializeField] private GameObject _magicPointStar = default;
    [SerializeField] private GameObject _player = default;
  
    private GameObject _burretClone = default;//発射した弾

    private float _fireTimeValue = 0; //何秒たったか
    private float _fireTime = 5;//発射までの時間
    private float _burretRange = 15f;//射程距離

    [Header("敵のHP"),SerializeField] private float _hitPoint = 210;
    [Header("敵の攻撃力の最低値"), SerializeField] private float _attackMinPower = 0f;
    [Header("敵の攻撃力の最高値"), SerializeField] private float _attackMaxPower = 0f;
    [Header("耐えてほしい攻撃回数"), SerializeField] private float NumberOfAttacks = default;

    [SerializeField] private AudioSource _turretAudio = default;
    [SerializeField] private AudioClip _burretSE = default;

    private PlayerStatusChange _playerStatusScript = default;

    private void Start()
    {
        _turretAudio=this.GetComponent<AudioSource>();
    }
    void Update()
    {
        float playerdistanceX = Mathf.Abs(_player.transform.position.x - this.transform.position.x);
        float playerdistanceY = Mathf.Abs(_player.transform.position.y - this.transform.position.y);

        if (_fireTimeValue >= _fireTime&&playerdistanceX<_burretRange&& playerdistanceY < _burretRange)
        {
            Fire();
        }
        else if(_fireTimeValue < _fireTime)
        {
            _fireTimeValue += Time.deltaTime;

        }

        if (_hitPoint <= 0)
        {
            Death();
        }
    }

    private void Fire()
    {
        _turretAudio.PlayOneShot(_burretSE);
        _fireTimeValue = 0;
        _burretClone = Instantiate(_burret, this.transform.position, Quaternion.FromToRotation(Vector3.up, this.transform.position - _player.transform.position));
        _burretClone.GetComponent<TurrentBurretScript>().GetAttackPower(_attackMinPower, _attackMaxPower);
    }

    public void BeAttacked(float Damage)
    {
        _hitPoint -= Damage;
    }

    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        //深度にあわせてステータスを強化
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();
        StatusAdjustment();
    }

    private void StatusAdjustment()
    {
        //プレイヤーの実質HP 
        float _playerHP = _playerStatusScript.EnemyAttackPowerAdjustment();

        if (_playerHP < _attackMaxPower * NumberOfAttacks)
        {
            //最大攻撃力はプレイヤーの体力の何％分か
            float firstCalculation = _attackMaxPower / _playerHP;

            //最大攻撃力にfirstCalculationで出した値を掛ける
            float secondCalculation = _attackMaxPower * firstCalculation;

            //secondCalculationで出した値を耐えてほしい攻撃回数で割る
            float thirdCalculation = Mathf.Floor(secondCalculation / NumberOfAttacks);

            //自身の攻撃力をthirdCalculationで出した分引く
            _attackMinPower -= thirdCalculation;
            _attackMaxPower -= thirdCalculation;

        }


        //プレイヤーの攻撃力を取得
        float playerAttackvalue = _playerStatusScript.EnemyHitPointAdjustment().Item1;

        //プレイヤーの今の職業を取得
        int playerProfession = _playerStatusScript.EnemyHitPointAdjustment().Item2;

        switch (playerProfession)
        {
            //剣士の時
            case 1:

                //プレイヤーが６回攻撃しても倒せないHPなら
                if (playerAttackvalue * 6 < _hitPoint)
                {

                    float lestHitPoint = default;

                    //６回攻撃した分の残りHPを計算
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //計算結果の値÷２引く
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;

            //魔法使いの時
            case 2:

                //プレイヤーが8回攻撃しても倒せないHPなら
                if (playerAttackvalue * 8 < _hitPoint)
                {
                    float lestHitPoint = default;

                    //６回攻撃した分の残りHPを計算
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //計算結果の値÷２引く
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;
        }

    }
    public void Death()
    {
        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue = Random.Range(minRandomValue, maxRandomValue);

        //3分の一でMp回復アイテムを出す
        if (randomValue <= maxRandomValue / halfValue)
        {
            Instantiate(_magicPointStar, this.transform.transform.position, Quaternion.identity);
        }

        //死んだことをステージコアに伝える
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }
}
