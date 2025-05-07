using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondBossAttackControll : BossStatus,IBossStatus
{
    private const int MAX_HEALTH = 10000;//ボスの体力定義
    private const int BODY_CONTACT_DAMAGE = 40;//ボスの接触ダメージ定義
    private const int MAX_ATTACK_NUMBER = 4;//最大の攻撃番号+1

    [SerializeField] private GameObject _exitHoleObject = default;//ステージ上の出口オブジェクト
    [SerializeField] private GameObject _healthBarObject = default;//ステージ上のボスの体力バーオブジェクト

    private GameObject _player = default;

    private Animator _bossAnime = default;

    [Header("ボスの剣"), SerializeField] private GameObject _bossSword = default;
    [SerializeField] private List<SummonEnemyScript> _summonEnemyPoint = new List<SummonEnemyScript>();

    private float _rushspeed = 10f;

    private bool _isFacingRight = false;//右向き
    private bool _isFacingLeft = false;//左向き
    private bool _isDirection = false;
    private bool _isLeftRush = false;
    private bool _isRightRush = false;
    private bool _isDistanceAdjustment = false;
    private bool _isDeath = false;

    private float _startPositionX = default;
    private float _hitPointMultipleValue = 1.2f;
    private float _attackPowerMultipleValue = 0.9f;
    private float _playerDistance = default;

    private int _attackNumber = 0;

    private float _coolDownTime = default; //攻撃後のクールタイム

    private string _animationName = default;

    private void Start()
    {

        Health = MAX_HEALTH;
        BodyContactDamage = BODY_CONTACT_DAMAGE;

        string findTag = "Player";
        _player = GameObject.FindGameObjectWithTag(findTag);

        _bossAnime = this.GetComponent<Animator>();

        _startPositionX = this.transform.position.x;
    }

    private void Update()
    {

        if (!IsAttackCoolDown)
        {
            DecisionAttack();
            IsAttackCoolDown = true;
        }

        switch (_attackNumber)
        {
            //突進攻撃
            case 1:
                FirstAttackNumber();
                break;

            //3連撃
            case 2:
                SecondAttackNumber();
                break;

            //雑魚召喚
            case 3:
                ThirdAttackNumber();
                break;
        }

        if (Health <= 0&&!_isDeath)
        {
            _isDeath = true;
            _exitHoleObject.SetActive(true);
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            _isLeftRush = false;
            _isRightRush = false;
        }

        string playerTag = "Player";

        if (collision.gameObject.CompareTag(playerTag) && BodyContactDamage > 0)
        {
            BeAttacked(collision.gameObject);
        }
    }

    #region 攻撃メソッド
    private void DecisionAttack()
    {
        int chooseAttackNumber = ChooseAttack(MAX_ATTACK_NUMBER);

        switch (chooseAttackNumber)
        {
            //突進攻撃
            case 1:
                //自分より右にいる
                if (_player.transform.position.x >= this.transform.position.x)
                {
                    _isRightRush = true;
                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    _isLeftRush = true;
                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                _attackNumber = chooseAttackNumber;
                break;

            //3連撃攻撃
            case 2:

                //自分がプレイヤーより右にいたら
                if (_player.transform.position.x < this.transform.position.x)
                {
                    //左を向かせる
                    _isFacingLeft = true;

                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                //自分が真ん中より左にいたら
                else if (_player.transform.position.x > this.transform.position.x)
                {

                    //右を向かせる
                    _isFacingRight = true;

                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }

                _isDistanceAdjustment = true;

                _attackNumber = chooseAttackNumber;

                break;

            //雑魚召喚
            case 3:

                //自分が真ん中より右にいたら
                if (_startPositionX < this.transform.position.x)
                {
                    //左を向かせる
                    _isFacingLeft = true;

                    this.transform.localRotation = new Quaternion(0, 180, 0, 0);
                }
                //自分が真ん中より左にいたら
                else if (_startPositionX > this.transform.position.x)
                {

                    //右を向かせる
                    _isFacingRight = true;

                    this.transform.localRotation = new Quaternion(0, 0, 0, 0);
                }



                _attackNumber = chooseAttackNumber;

                break;
        }
    }

    private void FirstAttackNumber()
    {
        _bossAnime.enabled = true;

        if (_isRightRush)
        {
            this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
        }
        else if (_isLeftRush)
        {
            this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
        }
        else
        {
            _attackNumber = 0;
            _coolDownTime = 3f;

            _animationName = "RushEndAnimation";
            _bossAnime.PlayInFixedTime(_animationName, 0, 0f);

            _isLeftRush = false;
            _isRightRush = false;


            StartCoroutine(AttackCoolDown(_coolDownTime));
        }
    }

    private void SecondAttackNumber()
    {
        if (_isDistanceAdjustment)
        {
            //一定距離にいない場合はプレイヤーと自分の距離を計算し続ける
            if (_isFacingLeft || _isFacingRight)
            {
                _playerDistance = this.transform.position.x - _player.transform.position.x;
            }

            float stopAdjustmentDistance = 3;

            //右向きでかつ、自分のポジションとプレイヤーとの距離が3以下だったら
            if (_isFacingRight && _playerDistance < -stopAdjustmentDistance)
            {
                //右に移動
                this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
            }
            //左向きで勝つ、自分のポジションとプレイヤーとの距離が3以上だったら
            else if (_isFacingLeft && _playerDistance > stopAdjustmentDistance)
            {
                //左に移動
                this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
            }
            else
            {
                _isDistanceAdjustment = false;
                _isFacingLeft = false;
                _isFacingRight = false;

                _bossAnime.enabled = true;
                _bossSword.GetComponent<BoxCollider2D>().enabled = true;

                _animationName = "SordAttackAnimation";
                _bossAnime.PlayInFixedTime(_animationName, 0, 0f);

            }
        }


        if (_isDirection)
        {
            DirectionChange();
        }


        //クールダウンはメソッドのEndSordAttack()にて制御
    }

    private void ThirdAttackNumber()
    {
        //右向きでかつ、自分のポジションが真ん中より左だったら
        if (_isFacingRight && _startPositionX >= this.transform.position.x)
        {
            //右に移動
            this.transform.position += Vector3.right * _rushspeed * Time.deltaTime;
        }
        //左向きで勝つ、自分のポジションが真ん中より右だったら
        else if (_isFacingLeft && _startPositionX <= this.transform.position.x)
        {
            //左に移動
            this.transform.position -= Vector3.right * _rushspeed * Time.deltaTime;
        }
        else
        {

            _attackNumber = 0;
            for (int summonAmmount = 0; summonAmmount < _summonEnemyPoint.Count; summonAmmount++)
            {
                _summonEnemyPoint[summonAmmount].SummonEnemyPreparation(null, _hitPointMultipleValue, _attackPowerMultipleValue);
            }

            _isFacingLeft = false;
            _isFacingRight = false;

            _coolDownTime = 5f;
            StartCoroutine(AttackCoolDown(_coolDownTime));

        }
    }

    #endregion
    public void StartDirectionChange()
    {
        _isDirection = true;
    }

    public void EndDirectionChange()
    {
        _isDirection = false;
    }

    public void DirectionChange()
    {
        //方向転換
        //自分よりプレイヤーが右にいたら
        if (_player.transform.position.x >= this.transform.position.x && !_isFacingRight)
        {
            //反転させる
            _isFacingRight = true;
            _isFacingLeft = false;
            this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }
        //自分よりプレイヤーが左にいたら
        else if (_player.transform.position.x <= this.transform.position.x && !_isFacingLeft)
        {
            _isFacingRight = false;
            _isFacingLeft = true;
            this.transform.localRotation = new Quaternion(0, 180, 0, 0);
        }
    }

    public void EndSordAttack()
    {
        _attackNumber = 0;
        _bossSword.GetComponent<BoxCollider2D>().enabled = false;
        _isFacingLeft = false;
        _isFacingRight = false;


        _coolDownTime = 2.5f;
        StartCoroutine(AttackCoolDown(_coolDownTime));

    }

    public void HealthBarSet()
    {
        BossHealthBar = _healthBarObject;
        print("a");
        HealthBarDisplay();

    }


}
