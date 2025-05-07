using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyScript : MonoBehaviour
{
    [Header("敵の攻撃力の最低値"), SerializeField] private float _attackMinPower = 0f;
    [Header("敵の攻撃力の最高値"), SerializeField] private float _attackMaxPower = 0f;
    [Header("体力"), SerializeField] private float _hitPoint = default;
    [Header("ノックバックの大きさ"), SerializeField] private float _knockBackValue = 0f;
    [Header("耐えてほしい攻撃回数"), SerializeField] private float NumberOfAttacks = default;

    [Header("Mp回復アイテム"), SerializeField] private GameObject _magicPointStar = default;

    private GameObject _player = default;

    private PlayerStatusChange _playerStatusScript = default;

    private Rigidbody2D _flyEnemyRb = default;//自身のリジットボディ

    private float _horizontalSpeed = 0.1f;//横のスピード
    private float _varticalSpeed = 0.1f;//縦のスピード

    private bool _isLeftRight = true;//falseは右移動、trueは左移動
    private bool _isUpDown = true;//falseは上移動、trueは下移動
    private bool _isStun = false;
    private bool _isRightKnockBack = false;//右ノックバック中か
    private bool _isLeftKnockBack = false;//左ノックバック中か

    private const float TIME_DELTATIME = 500f;
    private const float MAX_HORIZONTAL_SPEED = 5f;//横の最大スピード
    private const float MAX_VARTICAL_SPEED = 1.5f;//縦の最大スピード

    // Start is called before the first frame update
    void Start()
    {
        _flyEnemyRb = this.GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        #region 移動処理
        if (_isStun)
        {
            NotMoveProcess();
        }
        else if (_player != null)
        {
            //通常の移動
            DefaultMoveProcess();
        }

        #endregion

        //体力が０になったら死亡判定
        if (_hitPoint <= 0)
        {
            Death();
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string _playerTag = "Player";
        //プレイヤーに当たったら
        if (collision.gameObject.CompareTag(_playerTag))
        {
            //乱数ダメージを与える
            float attackPower = Random.Range(_attackMinPower, _attackMaxPower);
            collision.gameObject.GetComponent<PlayerAttack>().TakeDamage(attackPower, this.gameObject, true, true);
        }

        int StagePartsLayer = 12;
        int FloorLayer = 18;

        //ステージの枠組みに当たったら
        if(collision.gameObject.layer==StagePartsLayer|| collision.gameObject.layer == FloorLayer)
        {
            StagePartsCollision(collision.gameObject);
        }
      
    }

    /// <summary>
    /// 上下左右移動メソッド
    /// </summary>
    private void DefaultMoveProcess()
    {
        #region 左右移動
        //自分よりも右にいたら
        if (_player.transform.position.x >= this.transform.position.x)
        {
            //変数を右向き用にする
            if (_isLeftRight)
            {
                _isLeftRight = false;
                _horizontalSpeed /= 2;
                this.transform.rotation = new Quaternion(0, 180, 0, 0);
            }


            //移動の速さが一定の速さまで達していたら
            if (_flyEnemyRb.velocity.x >= MAX_HORIZONTAL_SPEED)
            {
                //一定の速度で右に移動
                _flyEnemyRb.velocity = new Vector2(MAX_HORIZONTAL_SPEED, _flyEnemyRb.velocity.y);
            }
            else
            {
                //徐々に右移動のスピードを上げる
                _flyEnemyRb.velocity = new Vector2(_horizontalSpeed, _flyEnemyRb.velocity.y);
                _horizontalSpeed += Time.deltaTime * 7.5f;
            }
        }
        //自分より左にいたら
        else
        {
            //変数を左向き用にする
            if (!_isLeftRight)
            {
                _horizontalSpeed /= 2;
                _isLeftRight = true;
                this.transform.rotation = new Quaternion(0, 0, 0, 0);
            }

            //移動の速さが一定の速さまで達していたら
            if (_flyEnemyRb.velocity.x <= -MAX_HORIZONTAL_SPEED)
            {
                //一定の速度で左に移動
                _flyEnemyRb.velocity = new Vector2(-MAX_HORIZONTAL_SPEED, _flyEnemyRb.velocity.y);
            }
            else
            {
                //徐々に左移動のスピードを上げる
                _flyEnemyRb.velocity = new Vector2(_horizontalSpeed, _flyEnemyRb.velocity.y);
                _horizontalSpeed -= Time.deltaTime * 7.5f;
            }
        }
        #endregion

        #region 上下移動
        //自分よりも上にいたら
        if (_player.transform.position.y >= this.transform.position.y)
        {
            //変数を上向き用にする
            if (_isUpDown)
            {
                _isUpDown = false;
                _varticalSpeed /= 1.5f;
            }

            //移動の速さが一定の速さまで達していたら
            if (_flyEnemyRb.velocity.y >= MAX_VARTICAL_SPEED)
            {
                //一定の速度で上に移動
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, MAX_VARTICAL_SPEED);
            }
            else
            {

                //徐々に上移動のスピードを上げる
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, _varticalSpeed);
                _varticalSpeed += Time.deltaTime * 3.5f;
            }
        }
        //自分より下にいたら
        else
        {
            //変数を下向き用にする
            if (!_isUpDown)
            {
                _varticalSpeed /= 1.5f;
                _isUpDown = true;
            }

            //移動の速さが一定の速さまで達していたら
            if (_flyEnemyRb.velocity.y <= -MAX_VARTICAL_SPEED)
            {
                //一定の速度で上に移動
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, -MAX_VARTICAL_SPEED);
            }
            else
            {
                //徐々に上移動のスピードを上げる
                _flyEnemyRb.velocity = new Vector2(_flyEnemyRb.velocity.x, _varticalSpeed);
                _varticalSpeed -= Time.deltaTime * 3.5f;
            }
        }
        #endregion
    }

    private void NotMoveProcess()
    {
        //移動していないかつ、右にノックバック中
        if (_isRightKnockBack)
        {
            if (_flyEnemyRb.velocity.x > 0)
            {
                //速度を徐々に戻す
                _flyEnemyRb.AddForce(new Vector2(MAX_HORIZONTAL_SPEED * Time.deltaTime * TIME_DELTATIME, 0));
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
            if (_flyEnemyRb.velocity.x < 0)
            {
                //速度を徐々に戻す
                _flyEnemyRb.AddForce(new Vector2(MAX_HORIZONTAL_SPEED * Time.deltaTime * TIME_DELTATIME, 0));
            }
            else
            {
                //ノックバック終了
                _isLeftKnockBack = false;
            }
        }
    }

    /// <summary>
    /// 何らかのステージパーツに当たった際の移動反転
    /// </summary>
    /// <param name="collisionParts"></param>
    private void StagePartsCollision(GameObject collisionParts)
    {
        float BoundValue = -0.75f;

        //壁のオブジェクトに当たったら
        string wallTag = "Wall";
        if (collisionParts.CompareTag(wallTag))
        {
            //横に向いてる方向の反対側にバウンドさせる
            _horizontalSpeed *= BoundValue;
        }

        //床、天井のオブジェクトに当たったら
        string cellingTag = "Celling";
        string floorTag = "Flor";
        if (collisionParts.CompareTag(cellingTag) || collisionParts.CompareTag(floorTag))
        {
            //横に向いてる方向の反対側にバウンドさせる
            _varticalSpeed *= BoundValue;
        }
    }

    /// <summary>
    /// 被ダメージ処理
    /// </summary>
    /// <param name="Damage">喰らうダメージ量</param>
    /// <param name="isKnockBackAttack">ノックバックする攻撃か</param>
    public void BeAttacked(float Damage , bool isKnockBackAttack)
    {
        //hpを減らす
        _hitPoint -= Damage;

        if (isKnockBackAttack)
        {
            //横に向いてる方向の反対側にバウンドさせる
            _flyEnemyRb.velocity = Vector2.zero;
            float BoundValue = -0.75f;
            _horizontalSpeed *= BoundValue;
        }
       
    }

    /// <summary>
    /// 死亡したときの処理
    /// </summary>
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

        //死んだことをステージコアに伝える
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);

    }

    /// <summary>
    /// 気絶デバフが付与された時の処理
    /// </summary>
    public void Stun()
    {
        //一定時間行動不能
        _isStun = true;
        _flyEnemyRb.velocity = Vector2.zero;
        _flyEnemyRb.AddForce(transform.right * _knockBackValue, ForceMode2D.Impulse);
        StartCoroutine(EndStun());

        //プレイヤーに当たらなくする
        int DontHitEnemyLayer = 20;
        this.gameObject.layer = DontHitEnemyLayer;
    }


    /// <summary>
    /// 階層によるステータスの上昇
    /// </summary>
    /// <param name="hitPointincreasedValue">体力が増える倍率</param>
    /// <param name="attackPowerIncreasedValue">攻撃力が増える倍率</param>
    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _player = GameObject.FindGameObjectWithTag("Player");

        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();

        StatusAdjustment();
    }


    /// <summary>
    /// プレイヤーとのステータスに差があったとき、自身のステータスを若干下げる
    /// </summary>
    private void StatusAdjustment()
    {
        #region 攻撃力調整
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
        #endregion

        #region　体力調整
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
        #endregion
    }


    /// <summary>
    /// スタン終了の処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndStun()
    {
        _horizontalSpeed = 0;
        _varticalSpeed = 0;
        float stunTime = 2f;
        yield return new WaitForSeconds(stunTime);
        _isStun = false;

        //プレイヤーに当たるようにする
        int EnemyLayer = 22;
        this.gameObject.layer = EnemyLayer;

    }
}
