using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemyScript : MonoBehaviour
{
    [Header("敵のHP "), SerializeField] private float _hitPoint = 0f;
    [Header("敵の攻撃力の最低値"), SerializeField] private float _attackMinPower = 0f;
    [Header("敵の攻撃力の最高値"), SerializeField] private float _attackMaxPower = 0f;
    [Header("ノックバックの大きさ"), SerializeField] private float _knockBackValue = 0f;
    [Header("耐えてほしい攻撃回数"), SerializeField] private float NumberOfAttacks = default;

    [Header("Mp回復アイテム"), SerializeField] private GameObject _magicPointStar = default;

    private GameObject _playerPosition = default; //プレイヤーのポジションをとる
    private PlayerStatusChange _playerStatusScript = default;

    private Rigidbody2D _enemyRB = default;//敵のリジットボディ

    private bool _isTurn = false;//falseなら左向き、trueなら右向き
    private bool _isStartCoroutine = false;
    private bool _isRightKnockBack = false;//右ノックバック中か
    private bool _isLeftKnockBack = false;//左ノックバック中か

    private bool _isStun = false;

    private int _turnTime = 0;


    private float _moveSpeed = 2f;//移動の速さ

    private const float MAX_MOVEMENT_SPEED = 2.5f;//敵の最大移動速度
    private const float TIME_DELTATIME = 500f; //Time.Deltatimeに掛ける値

    private const int FLIP_VALUE = 180;
    // Start is called before the first frame update
    void Start()
    {
        
        _enemyRB = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //スタン中じゃなかったらかつ、プレイヤーを見つけていたら
        if (!_isStun&&_playerPosition!=null)
        {
            Move();
        }
        //スタンしていたら
        else
        {
            StunKnockBackControl();
        }
        //体力が０になったら
        if (_hitPoint < 0)
        {
            Death();
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        string PlayerTag = "Player";
        //プレイヤーに当たったら
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            //ダメージ値の乱数取得
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);

            //ダメージを与える
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

     
    }
    private void Death()
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

        //死んだことをスてージコアに伝える
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }
    private void Move()
    {

        int maxTurnNumberOfTime = 7;//最大反転回数

        //７回振り向いたら追尾を一時中断する
        if (_turnTime < maxTurnNumberOfTime)
        {
            //プレイヤーを追尾させる
            if (_playerPosition.transform.position.x <= this.transform.position.x)
            {
                //右向きだったら
                if (_isTurn)
                {
                    //左に向かせる
                    _isTurn = false;
                    _turnTime++;

                    //反対にする
                    this.transform.rotation = new Quaternion(0, 0, 0, 0);
                }

                //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
                if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    //print("←"+ this._playerRB.velocity.x);
                    _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
            else
            {
                //左向きだったら
                if (!_isTurn)
                {
                    //右に向かせる
                    _isTurn = true;
                    _turnTime++;
                    //反対にする
                    this.transform.rotation = new Quaternion(0, FLIP_VALUE, 0, 0);
                }
              

                //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
                if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
                {

                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if(_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    //print("→"+this._playerRB.velocity.x);
                    _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
        }
        else
        {
            //左向きなら
            if (!_isTurn)
            {
                //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
                if (_enemyRB.velocity.x > -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x < -MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    _enemyRB.velocity = new Vector2(-MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }
            //右向きなら
            else
            {
                //敵の移動速度をMAXMOVEMENTSPEEDまでに制限する
                if (_enemyRB.velocity.x < MAX_MOVEMENT_SPEED)
                {

                    _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else if (_enemyRB.velocity.x > MAX_MOVEMENT_SPEED)
                {
                    _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
                }
                else
                {
                    _enemyRB.velocity = new Vector2(MAX_MOVEMENT_SPEED, this._enemyRB.velocity.y);
                }
            }

            if (!_isStartCoroutine)
            {
                _isStartCoroutine = true;
                StartCoroutine(ResetTurnTime());
            }

        }

    }

    private void StunKnockBackControl()
    {
        //移動していないかつ、右にノックバック中
        if (_isRightKnockBack)
        {
            if (_enemyRB.velocity.x > 0)
            {
                //速度を徐々に戻す
                _enemyRB.AddForce(new Vector2(-_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
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
                _enemyRB.AddForce(new Vector2(_moveSpeed * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isLeftKnockBack = false;
            }
        }
    }

    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        //深度によってステータスを上げる
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _playerPosition = GameObject.FindGameObjectWithTag("Player");

        _playerStatusScript = _playerPosition.GetComponent<PlayerStatusChange>();

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

    public void BeAttacked(float Damage, float knockBackPower , bool isKnockBackAttack)
    {
        //hpを減らす
        _hitPoint -= Damage;

        if (isKnockBackAttack)
        {
            //プレイヤーが自分より右にいたら
            if (_playerPosition.transform.position.x > this.transform.position.x)
            {
                //左向きだったら
                if (_isTurn)
                {
                    //自分からみてまっすぐにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
                else
                {
                    //自分から見て後ろにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }

            }
            //プレイヤーが自分より左にいたら
            else
            {
                //左向きだったら
                if (_isTurn)
                {
                    //自分からみて後ろにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(-transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
                else
                {
                    //自分から見てまっすぐにノックバック
                    _enemyRB.velocity = Vector2.zero;
                    _enemyRB.AddForce(transform.right * knockBackPower, ForceMode2D.Impulse);
                    _knockBackDirection();
                }
            }
        }
       
    }

    private void _knockBackDirection()
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
        //一定時間行動不能
        _isStun = true;
        _enemyRB.velocity = Vector2.zero;
        _enemyRB.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(WakeUp());
        //プレイヤーに当たらなくする
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }

    private IEnumerator WakeUp()
    {
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //プレイヤーに当たるようにする
        int EnemyLayer = 10;
        this.gameObject.layer = EnemyLayer;
    }
    private IEnumerator ResetTurnTime()
    {
        //５秒後に追尾を再開させる
        float waitTime = 5f;
        yield return new WaitForSeconds(waitTime);
        _turnTime = 0;
        _isStartCoroutine = false;
    }


}
