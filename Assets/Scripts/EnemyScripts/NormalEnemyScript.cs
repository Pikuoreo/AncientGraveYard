using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NormalEnemyScript : MonoBehaviour
{
    private GameObject _player = default;
    private PlayerStatusChange _playerStatusScript=default;

    [Header("敵のHP "), SerializeField] private float _hitPoint = 0f;
    [Header("敵の攻撃力の最低値"), SerializeField] private float _attackMinPower = 0f;
    [Header("敵の攻撃力の最高値"), SerializeField] private float _attackMaxPower = 0f;
    [Header("ノックバックの大きさ"), SerializeField] private float _knockBackValue = 0f;
    [Header("耐えてほしい攻撃回数"),SerializeField] private float NumberOfAttacks = default;

    [Header("Mp回復アイテム"),SerializeField] private GameObject _magicPointStar = default;

    private Rigidbody2D _enemyRB = default;

    private float _moveX = 2f;

    private bool _isMoveDirection = false; //trueは左向き、falseは右向き
    private bool _isStun = false;
    private bool _isRightKnockBack = false;//右ノックバック中か
    private bool _isLeftKnockBack = false;//左ノックバック中か

    private const float MAX_MOVEMENT_SPEED = 2.5f;//プレイヤーの最大移動速度
    private const float TIME_DELTATIME = 500f; //Time.Deltatimeに掛ける値

    private const int FLIP_VALUE = 180;

    // Start is called before the first frame update
    void Start()
    {
        _enemyRB = this.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //スタンしていない
        if (!_isStun)
        {
            DefaultMove();
        }
        //スタンしていたら
        else
        {
            StunProcess();
        }

        if (this._hitPoint <= 0)
        {
            Death();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //壁に当たったら
        if (collision.gameObject.CompareTag("Wall"))
        {
            //左向きだったら
            if (_isMoveDirection)
            {
                //右向きにする
                _isMoveDirection = false;

                //反対にする
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            //右向きだったら
            else
            {
                //左向きにする
                _isMoveDirection = true;

                //反対にする
                this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
            }
        }

        string _playerTag = "Player";
        if (collision.gameObject.CompareTag(_playerTag))
        {
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TransparentWall"))
        {
            //左向きだったら
            if (_isMoveDirection)
            {
                //右向きにする
                _isMoveDirection = false;

                //反対にする
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }
            //右向きだったら
            else
            {
                //左向きにする
                _isMoveDirection = true;

                //反対にする
                this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
            }
        }
    }
    

    private void DefaultMove()
    {
        //左移動
        if (_isMoveDirection)
        {
            //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
            if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
            {

                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            //
            else if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }

            //通常移動
            else
            {
                _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
            }
        }
        //右移動
        else
        {
            //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
            if (_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
            {
                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }

            //通常移動
            else
            {
                _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
            }
        }
        //this._enemyRB.velocity = new Vector2(_moveX, this._enemyRB.velocity.y);
    }

    private void StunProcess()
    {
        //移動していないかつ、右にノックバック中
        if (_isRightKnockBack)
        {
            if (_enemyRB.velocity.x > 0)
            {
                //速度を徐々に戻す
                _enemyRB.AddForce(new Vector2(-_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isRightKnockBack = false;
            }
        }
        //移動していないかつ、左にノックバック中
        else if (_isLeftKnockBack)
        {
            if (_enemyRB.velocity.x < 0)
            {
                //速度を徐々に戻す
                _enemyRB.AddForce(new Vector2(_moveX * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isLeftKnockBack = false;
            }
        }
    }
    /// <summary>
    /// 攻撃を食らった時の処理
    /// </summary>
    /// <param name="damage">喰らうダメージ量</param>
    /// <param name="knockBackPower">ノックバックの強さ</param>
    public void BeAttacked(float damage, float knockBackPower,bool isKnockBackAttack)
    {
        //HPを減らす
        _hitPoint -= damage;

        //ノックバック

        if (isKnockBackAttack)
        {
            //プレイヤーが自分より右にいたら
            if (_player.transform.position.x > this.transform.position.x)
            {
                //左向きだったら
                if (_isMoveDirection)
                {

                    //自分からみてまっすぐにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                    //右向きにする
                    _isMoveDirection = false;
                    //反転させる
                    this.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                else
                {
                    //自分から見て後ろにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                }

            }
            //プレイヤーが自分より左にいたら
            else
            {
                //左向きだったら
                if (_isMoveDirection)
                {
                    //自分からみて後ろにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                }
                //右向きだったら
                else
                {
                    //自分から見てまっすぐにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _KnockBackDirection();

                    //左向きにする
                    _isMoveDirection = true;

                    //反転させる
                    this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
                }
            }
        }
    }

    private void _KnockBackDirection()
    {
        //ノックバックの向きを見る

        //右にノックバックしていたら
        if (_enemyRB.velocity.x > 0)
        {
            _isRightKnockBack = true;
        }
        //左にノックバックしていたら
        else
        {
            _isLeftKnockBack = true;
        }

    }

    /// <summary>
    /// ノックバックして、一定時間行動不可
    /// </summary>
    public void Stun()
    {
        //ノックバック
        _isStun = true;
        _enemyRB.velocity = Vector2.zero;
        _enemyRB.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(StunRecover());
        //プレイヤーに当たらなくする
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }

   

    public void StatusIncrease(float hitPointIncreasedValue,float attackPowerIncreasedValue)
    {

        //自身の基礎HPに倍率をかける
        _hitPoint *= hitPointIncreasedValue;

        //自身の攻撃力（最低値、最高値）に倍率をかける
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        //プレイヤーのスクリプト取得
        string playerTag = "Player";
        _player = GameObject.FindGameObjectWithTag(playerTag);
        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();

        //プレイヤーのステータスによって自身のステータス変化をするメソッドを呼び出す
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
                if (playerAttackvalue*6 < _hitPoint)
                {
                    
                    float lestHitPoint = default;

                    //６回攻撃した分の残りHPを計算
                   lestHitPoint=_hitPoint - playerAttackvalue * 6;

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

    private IEnumerator StunRecover()
    {
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //プレイヤーに当たるようにする
        int EnemyLayer = 10;
        this.gameObject.layer = EnemyLayer;
    }

    public void Death()
    {

        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue =Random.Range(minRandomValue, maxRandomValue);

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
