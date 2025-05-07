using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class FirstBossAttackControll : BossStatus,IBossStatus
{
    private const int MAX_HEALTH = 7500;//ボスの体力定義
    private const int BODY_CONTACT_DAMAGE = 35;//ボスの接触ダメージ定義
    private const int MAX_ATTACK_NUMBER = 6;//最大の攻撃番号+1

    private int _attackNumber = 0;
    private int _attackObjectNumber = 0;


    private bool _isPositionAdjustment = false;//trueでアニメーションでずれたポジション調整
    private bool _isFixed = false; //物の固定
    private bool _isLeftMove = false;
    private bool _isRightMove = false;
    private bool _isDeath = false;

    private float _stompposition = default;//ストンプする位置
    private float _elapsedTime = 0;//経過時間
    private float _burretAttackWaittime = 2f; //弾突進攻撃の待ち時間

    private string _animationName = default;

    [SerializeField] private List<GameObject> _wallTentacle = new List<GameObject>();//攻撃番号１のオブジェクト
    [SerializeField] private List<GameObject> _cellingTentacle = new List<GameObject>();//攻撃番号２のオブジェクト
    [SerializeField] private List<GameObject> _magicBurret = new List<GameObject>(); //攻撃番号３のオブジェクト
    [SerializeField] private GameObject _chaserBossBurret = default;//攻撃番号４のオブジェクト

    [SerializeField] private GameObject _exitHoleObject = default;//ステージ上の出口オブジェクト
    [SerializeField] private GameObject _healthBarObject = default;//ステージ上のボスの体力バーオブジェクト

    [SerializeField] private GameObject _rightWall = default;//ステージ上の天井
    [SerializeField] private GameObject _celling = default;//ステージ上の右の壁

    private GameObject _player = default;
    private GameObject _stageflor = default;

   

    private List<GameObject> _attackObject = new List<GameObject>();//攻撃するオブジェクトを入れる
    private List<SpriteRenderer> _attackObjectSprite = new List<SpriteRenderer>();//攻撃するオブジェクトのSpriteRenderer
    private List<BoxCollider2D> _attackObjectBoxCollider = new List<BoxCollider2D>();//攻撃するオブジェクトのBoxCollider
    private List<CircleCollider2D> _attackObjectCircleCollider = new List<CircleCollider2D>();//攻撃するオブジェクトのCircleCollider

    [SerializeField] LayerMask _groundLayer = default;//地面オブジェクトのレイヤー

    private Animator _bossAnime = default;

    private void Start()
    {
        Health = MAX_HEALTH;
        BodyContactDamage = BODY_CONTACT_DAMAGE;

        string findTag = "Player";
        _player = GameObject.FindGameObjectWithTag(findTag);

        _bossAnime=this.GetComponent<Animator>();

       
      
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

            //壁から触手攻撃
            case 1:
                AttackNumber1();
                break;

            //天井から触手攻撃
            case 2:
                AttackNumber2();
                break;

            //弾突進攻撃
            case 3:
                AttackNumber3();
                break;

            //追尾弾攻撃
            case 4:
                AttackNumber4();
                break;

            //ストンプ攻撃
            case 5:
                AttackNumber5();
                break;
               
        }

        if (Health <= 0&&!_isDeath)
        {
            _isDeath = true;
            Death();
        }
    }

    private void DecisionAttack()
    {
        //リストの初期化
        _attackObject.Clear();
        _attackObjectBoxCollider.Clear();
        _attackObjectCircleCollider.Clear();
        _attackObjectSprite.Clear();

        int chooseAttackNumber = ChooseAttack(MAX_ATTACK_NUMBER);
        switch (chooseAttackNumber)
        {
            //壁から触手攻撃
            case 1:
                //伸ばす触手の数を取得
                for (int addAmount = 0; addAmount < _wallTentacle.Count; addAmount++)
                {
                    //攻撃するオブジェクト入れに入れる
                    _attackObject.Add(_wallTentacle[addAmount]);
                    _attackObjectSprite.Add(_wallTentacle[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectBoxCollider.Add(_wallTentacle[addAmount].GetComponent<BoxCollider2D>());
                }
                _attackNumber = chooseAttackNumber;
                break;

            //天井から触手攻撃
            case 2:

                //伸ばす触手の数を取得
                for (int addAmount = 0; addAmount < _cellingTentacle.Count; addAmount++)
                {
                    _attackObject.Add(_cellingTentacle[addAmount]);
                    _attackObjectSprite.Add(_cellingTentacle[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectBoxCollider.Add(_cellingTentacle[addAmount].GetComponent<BoxCollider2D>());
                }
                _attackNumber = chooseAttackNumber;

                break;

            //弾突進攻撃
            case 3:

                _bossAnime.enabled = true;

                for (int addAmount = 0; addAmount < _magicBurret.Count; addAmount++)
                {
                    _attackObject.Add(_magicBurret[addAmount]);
                    _attackObjectSprite.Add(_magicBurret[addAmount].GetComponent<SpriteRenderer>());
                    _attackObjectCircleCollider.Add(_magicBurret[addAmount].GetComponent<CircleCollider2D>());
                }

                _animationName = "BurretAnimation";

                _bossAnime.Play(_animationName, 0, 0f);

                _attackNumber = chooseAttackNumber;

                break;

            //追尾弾攻撃
            case 4:

                _attackNumber = chooseAttackNumber;
                break;

            //ストンプ攻撃
            case 5:

                _bossAnime.enabled = true;
                _animationName = "StompJumpAnimation";

                //上に上がるアニメーション起動
                _bossAnime.Play(_animationName, 0, 0f);

                //踏みつける位置を代入
                _stompposition = _player.transform.position.x;

                _attackNumber = chooseAttackNumber;
                break;
        }
    }

    #region 攻撃メソッド
    private void AttackNumber1()
    {
        if (!_isFixed)
        {
            float objectscaleX = 0.01f;
            float objectscaleY = 0.45f;

            //スケールの設定
            _attackObject[_attackObjectNumber].transform.localScale = new Vector3(objectscaleX, objectscaleY);
            //最初に触手の位置を固定
            _isFixed = true;
            _attackObject[_attackObjectNumber].transform.position = new Vector3(_rightWall.transform.position.x, _player.transform.position.y);

            //見た目をonにする
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            //当たり判定をonにする
            _attackObjectBoxCollider[_attackObjectNumber].enabled = true;
        }

        const float MAX_EXTEND = 0.3f;
        //左端に到達するまで
        if (_attackObject[_attackObjectNumber].transform.localScale.x < MAX_EXTEND)
        {
            float extendSpeed = 0.05f;
            //左に触手を伸ばす
            _attackObject[_attackObjectNumber].transform.localScale += new Vector3(extendSpeed * Time.deltaTime, 0);
        }
        else if (_attackObjectNumber < _attackObject.Count - 1)
        {

            //消えるアニメーションを流す
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);

            //次の触手を伸ばし始める
            _isFixed = false;
            _attackObjectNumber++;
        }
        else
        {
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);
            //攻撃を止める
            _attackObjectNumber = 0;
            _isFixed = false;

            int attackwaitTime = 2;
            StartCoroutine(AttackCoolDown(attackwaitTime));

            _attackNumber = 0;
        }
       
    }

    private void AttackNumber2()
    {
        if (!_isFixed)
        {
            float objectscaleX = 0.01f;
            float objectscaleY = 0.45f;

            //スケールの設定
            _attackObject[_attackObjectNumber].transform.localScale = new Vector3(objectscaleX, objectscaleY);

            //最初に触手の位置を固定
            _isFixed = true;
            _attackObject[_attackObjectNumber].transform.position = new Vector3(_player.transform.position.x, _celling.transform.position.y);

            //見た目をonにする
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            //当たり判定をonにする
            _attackObjectBoxCollider[_attackObjectNumber].enabled = true;

        }
        const float MAX_EXTEND = 0.16f;

        //一番下に到達するまで
        if (_attackObject[_attackObjectNumber].transform.localScale.x < MAX_EXTEND)
        {
            float extendSpeed = 0.05f;
            //下に触手を伸ばす
            _attackObject[_attackObjectNumber].transform.localScale += new Vector3(extendSpeed * Time.deltaTime, 0);
        }
        else if (_attackObjectNumber < _attackObject.Count - 1)
        {

            //消えるアニメーションを流す
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);

            //次の触手を伸ばし始める
            _isFixed = false;
            _attackObjectNumber++;
        }
        else
        {
            _attackObject[_attackObjectNumber].GetComponent<Animator>().SetBool("isLeave", true);
            //攻撃を止める
            _attackNumber = 0;
            _attackObjectNumber = 0;
            _isFixed = false;

            //クールダウン開始
            int attackwaitTime = 2;
            StartCoroutine(AttackCoolDown(attackwaitTime));
        }

        
    }

    private void AttackNumber3()
    {
        //最初は攻撃まで２秒待つ
        if (_elapsedTime > _burretAttackWaittime)
        {
            //弾発射開始

            _attackObjectCircleCollider[_attackObjectNumber].enabled = true;
            _attackObjectSprite[_attackObjectNumber].enabled = true;

            _attackObject[_attackObjectNumber].GetComponent<BossBurretScript>().Attack(_player);
            _attackObjectNumber++;
            _elapsedTime = 0;
            //一度攻撃を始めたら１秒待つようにする
            _burretAttackWaittime = 1;
        }
        else if (_attackObjectNumber < _attackObject.Count)
        {
            _elapsedTime += Time.deltaTime;
        }
        else
        {
            //待ち時間を２秒に戻す
            _burretAttackWaittime = 2;

            _elapsedTime = 0;

            //リスト参照変数を０にする
            _attackObjectNumber = 0;

            _attackNumber = 0;

            int attackwaitTime = 3;
            StartCoroutine(AttackCoolDown(attackwaitTime));
        }

       
    }

    private void AttackNumber4()
    {
       _attackObject.Add(Instantiate(_chaserBossBurret, this.transform.position, Quaternion.identity));

        int attackwaitTime = 7;
        StartCoroutine(AttackCoolDown(attackwaitTime));
        _attackNumber = 0;
    }

    private void AttackNumber5()
    {
        float moveSpeed = 25f;
        //プレイヤーが自分より右にいたら
        if (_stompposition >= this.transform.position.x && !_isLeftMove)
        {
            this.transform.position += new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
            _isRightMove = true;
        }
        else if (_isRightMove && _isFixed)
        {
            //踏みつけアニメーションを始める
            StartCoroutine(StompfallAnimation());

            //攻撃処理終了
            _attackNumber = 0;
            _isRightMove = false;
            _isFixed = false;
        }

        //プレイヤーが自分より左にいたら
        if (_stompposition <= this.transform.position.x && !_isRightMove)
        {
            this.transform.position -= new Vector3(moveSpeed, 0, 0) * Time.deltaTime;
            _isLeftMove = true;
        }
        else if (_isLeftMove && _isFixed)
        {
            //踏みつけアニメーションを開始する
            StartCoroutine(StompfallAnimation());

            //処理終了
            _attackNumber = 0;
            _isLeftMove = false;
            _isFixed = false;
        }

        //次の攻撃までのクールダウンはコルーチンメソッドのStompfallAnimationにて制御

    }

    public void PositionAdjustment()
    {
        _bossAnime.enabled = false;
        _stageflor = Physics2D.OverlapBox(this.transform.position - new Vector3(0, 2f, 0), new Vector2(0.1f, 2f), 0, _groundLayer).gameObject;
        _isPositionAdjustment = true;

        //次の攻撃までのクールダウン
        int attackwaitTime = 3;

        StartCoroutine(AttackCoolDown(attackwaitTime));

    }
    #endregion

    /// <summary>
    /// 攻撃番号3の攻撃アニメーション終了後に呼び出す
    /// </summary>
    public void EndBurretAttackAnimation()
    {
        for (int FixedAmmount = 0; FixedAmmount < _attackObject.Count; FixedAmmount++)
        {
            _attackObject[FixedAmmount].transform.position = _attackObject[FixedAmmount].transform.position;
        }
        _bossAnime.enabled = false;

    }



    /// <summary>
    /// 攻撃番号５のジャンプアニメーション終了時から呼び出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator StompfallAnimation()
    {
        //待ち時間
        float waitTime = 0.35f;
        yield return new WaitForSeconds(waitTime);
        {
            //踏みつけアニメーション開始
            string animationName = "StompFallAnimation";
            _bossAnime.Play(animationName, 0, 0f);
        }
    }

   

    /// <summary>
    /// 攻撃番号５の攻撃アニメーション終了後から呼び出す
    /// </summary>
    public void StompJumpFixed()
    {
        _isFixed = true;
    }

    public override void Death()
    {
        for (int attackObjectAmmount = 0; attackObjectAmmount < _attackObject.Count; attackObjectAmmount++)
        {
            Destroy(_attackObject[attackObjectAmmount]);
        }

        _exitHoleObject.SetActive(true);
        base.Death();
    }

    public void HealthBarSet()
    {
        BossHealthBar = _healthBarObject;

        HealthBarDisplay();
    }

    public override IEnumerator AttackCoolDown(float coolDowntime)
    {
        if (_isPositionAdjustment)
        {
            this.transform.position = new Vector3(this.transform.position.x, _stageflor.transform.position.y + 3, this.transform.position.z);
            _isPositionAdjustment = false;
        }
        return base.AttackCoolDown(coolDowntime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string playerTag = "Player";

        if (collision.gameObject.CompareTag(playerTag) && BodyContactDamage > 0)
        {
            BeAttacked(collision.gameObject);
        }
    }
}
