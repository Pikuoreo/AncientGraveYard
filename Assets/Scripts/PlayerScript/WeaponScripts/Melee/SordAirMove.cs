using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SordAirMove : MonoBehaviour
{
    private List<GameObject> _attackedenemy = new List<GameObject>();//攻撃が当たった敵を格納

    [Header("敵のダメージ音"), SerializeField] private List<AudioClip> _enemydamageSEs = new List<AudioClip>();

    [SerializeField] private TextMeshProUGUI _damageText = default;


    private bool _isHit = false;//攻撃がない頭に当たったか
    private bool _isReturn = false;//当たった敵はすでに攻撃を食らっているか

    private float _playerMeleeAttackPower = default;//プレイヤーの近接攻撃力

    private int _enemyJudge = default;//リストの見る番号
    private int _enemyDamageSeValue = default;//敵を攻撃したときに流すSeのナンバー

    private GameObject _worldSpaceCanvas = default;

    private SpriteRenderer _sordAirSprite = default;
    private BoxCollider2D _sordAirCollider = default;
    private Animator _sordAirMoveAnime = default;

    private PlayerAttack _playerAttack = default;

    private AudioSource _gameSE = default;

    // Start is called before the first frame update
    void Awake()
    {
        _gameSE = this.GetComponent<AudioSource>();

        _sordAirSprite = this.GetComponentInParent<SpriteRenderer>();
        _sordAirCollider = this.GetComponent<BoxCollider2D>();
        _sordAirMoveAnime = this.GetComponent<Animator>();

        string _findtag = "Player";

        _playerAttack=GameObject.FindGameObjectWithTag(_findtag).GetComponent<PlayerAttack>();

        _findtag = "WorldSpaceCanvas";

        _worldSpaceCanvas = GameObject.FindGameObjectWithTag(_findtag);
    }

    /// <summary>
    /// 剣気を動かし始める
    /// </summary>
    /// <param name="MoveDirection">falseは左向き、trueは右向き</param>
    public void MoveStart(bool MoveDirection)
    {
        //見た目、当たり判定表示
        _sordAirSprite.enabled = true;
        _sordAirCollider.enabled = true;

        //現在のプレイヤーの近接攻撃力参照
        _playerMeleeAttackPower = _playerAttack.GiveMeleeAttackPower();

        //剣が右攻撃なら
        if (MoveDirection)
        {
            //右を向かせる
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        //剣が左攻撃なら
        else
        {
            int flipValue = 180;
            //左を向かせる
            this.transform.rotation = new Quaternion(0, flipValue, 0, 0);
        }


        string animationName = "SordAirMove";
        _sordAirMoveAnime.PlayInFixedTime(animationName, 0, 0f);

    }

    public void endAnimation()
    {
        //移動アニメーション終了
        _sordAirSprite.enabled = false;
        _sordAirCollider.enabled = false;
        _attackedenemy.Clear();
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
                while (_enemyJudge < _attackedenemy.Count)
                {
                    //すでにその敵に攻撃が当たっていたら
                    if (_attackedenemy[_enemyJudge] == collision.gameObject)
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
            _attackedenemy.Add(collision.gameObject);

            //攻撃が当たった
            _isHit = true;

            //トリガーが当たった位置に生成する
            TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, collision.transform.position + collision.bounds.extents, Quaternion.identity);

            //キャンバスを親にする
            _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

            //ダメージ値をテキストに入れる
            _damageTextDummy.text = _playerMeleeAttackPower.ToString();

            //ダメージテキスト表示
            _damageTextDummy.GetComponent<DamageTextScript>().TextDefaultMove();

            //敵にダメージを与える
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                _enemyDamageSeValue = 0;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_playerMeleeAttackPower,0, false);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                _enemyDamageSeValue = 1;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);

                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_playerMeleeAttackPower);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {

                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_playerMeleeAttackPower,0,false);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_playerMeleeAttackPower, false);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                _enemyDamageSeValue = 2;
                _gameSE.PlayOneShot(_enemydamageSEs[_enemyDamageSeValue]);

                IBossStatus bossInterface=collision.gameObject.GetComponent<IBossStatus>();

                bossInterface.TakeDamage(_playerMeleeAttackPower);

                //collision.gameObject.GetComponent<FirstBossAttackControll>().TakeDamage(_playerMeleeAttackPower);
            }

        }
    }
}
