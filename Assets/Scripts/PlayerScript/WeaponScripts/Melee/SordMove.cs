using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SordMove : MonoBehaviour
{
    private bool _isAttack = false;　//攻撃をしたか
    private bool _isPreAttack = false;//攻撃をためてる最中か
    private bool _isHit = false;//攻撃がない頭に当たったか
    private bool _isReturn = false;//当たった敵はすでに攻撃を食らっているか
    private bool _isCritical = false;//クリティカルしたかどうか
    private bool _isPlayAnimation = false;//trueでアニメーション再生

    private int _enemyJudge = default;//リストの見る番号
   

    private float _playerAttackPower = 0;//プレイヤーの攻撃力
    private float _knockBackPower = 7.5f;

    private GameObject _player = default;　//プレイヤー参照
    private GameObject _worldSpaceCanvas = default;

    private List<GameObject> _attackedEnemy = new List<GameObject>();//攻撃が当たった敵を格納

    private SpriteRenderer WeaponSprite = default;　//自身の見た目
    private BoxCollider2D WeaponCollider = default;　//自身の当たり判定
    private Animator _weaponAnim = default;　//自身のアニメーション

    [SerializeField] private TextMeshProUGUI _damageText = default;

    private PlayerAttack _playerAttackScript = default;

    private AudioSource _gameSE = default;

    private int _enemyDamageSeValue = default;

    [Header("剣を振った時の音"), SerializeField] private AudioClip _sordSE = default;
    [Header("敵のダメージ音"), SerializeField] private List<AudioClip> _enemyDamageSEs = new List<AudioClip>(); 

    // Start is called before the first frame update
    void Start()
    {
        _gameSE=this.GetComponent<AudioSource>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerAttackScript = _player.GetComponent<PlayerAttack>();
        WeaponCollider = this.GetComponent<BoxCollider2D>();
        WeaponSprite = this.GetComponent<SpriteRenderer>();

        _weaponAnim = this.GetComponent<Animator>();

        _worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPreAttack)
        {
            //自分のポジションを常にプレイヤーの少し横に合わせる
            this.transform.position = _player.transform.position;
        }

        if (_isAttack&&!_isPlayAnimation)
        {
           
            if (this.transform.localRotation.z >=0)
            {
                StartSordAnimation();
            }
            else
            {
                StartSordAnimation();
            } 
        }
    }

    private void StartSordAnimation()
    {
        string animationName = "isAttack";
        //剣を振るアニメーションを流す
        _weaponAnim.SetBool(animationName, true);
        _isPlayAnimation = true;
    }

    public void AttackStart(bool isStart , bool isAnimationDirection)
    {
        _isPreAttack = isStart;

        //trueだったら右向きのアニメーション
        if (isAnimationDirection)
        {
            _weaponAnim.SetBool("isRight", true);
        }
        //falseだったら左向きのアニメーション
        else
        {
            _weaponAnim.SetBool("isLeft", true);
        }
    }

    /// <summary>
    /// アニメーションが終わった後に流す
    /// </summary>
    public void AttackEnd()
    {
        //攻撃終わり後、すべて最初に戻す
        _isHit = false;
        _attackedEnemy.Clear();

        string animationName = "isAttack";

        _weaponAnim.SetBool(animationName, false);
        _isAttack = false;
        _isPlayAnimation = false;

        //見た目を見えなくする
        WeaponCollider.enabled = false;
        WeaponSprite.enabled = false;

        //攻撃終了
        _isPreAttack = false;
        _playerAttackScript.MeleeAttackEnd();
    }

    public void GetAttackPower(float playerPower, bool isCritical)
    {
        //攻撃力、クリティカル判定を取得
        _playerAttackPower = playerPower;

        //攻撃判定ON
        WeaponCollider.enabled = true;
        _isAttack = true;
        _isCritical = isCritical;
        _gameSE.PlayOneShot(_sordSE);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        if (collision.gameObject.layer == enemyLayer || 
            collision.gameObject.layer == donthitEnemyLayer || 
            collision.gameObject.layer == FlyEnemyLayer)
        {
            //何かしらに攻撃が当たっていたら
            if (_isHit)
            {
                while (_enemyJudge < _attackedEnemy.Count)
                {
                    //すでにその敵に攻撃が当たっていたら
                    if (_attackedEnemy[_enemyJudge] == collision.gameObject)
                    {
                        _isReturn = true;
                    }
                    _enemyJudge++;
                }
                _enemyJudge = 0;
            }

            //すでに攻撃が当たっている敵ならなにもしない
            if (_isReturn)
            {
                _isReturn = false;
                return;

            }

            //攻撃が当たった敵をリストに入れる
            _attackedEnemy.Add(collision.gameObject);

            //攻撃が当たった
            _isHit = true;

            //トリガーが当たった位置に生成する
            TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, collision.transform.position+collision.bounds.extents, Quaternion.identity);

            //キャンバスを親にする
            _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

            //ダメージ値をテキストに入れる
            _damageTextDummy.text = _playerAttackPower.ToString();
            //クリティカルが発生したら
            if (_isCritical)
            {
                //テキストを赤にする
                _damageTextDummy.GetComponent<DamageTextScript>().TextCriticalMove();
            }
            //しなかったら
            else
            {
                //テキストを白にする
                _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();
            }

            //敵にダメージを与える
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                _enemyDamageSeValue = 0;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_playerAttackPower, _knockBackPower,true);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                _enemyDamageSeValue = 1;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked( _playerAttackPower);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {

                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_playerAttackPower, _knockBackPower,true);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_playerAttackPower,true);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemyDamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_playerAttackPower);
            }

        }
    }

    

   
}
