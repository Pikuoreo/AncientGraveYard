using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ChaserBurretScript : MonoBehaviour
{
    private float _burretPower = 0f;//弾の威力
    private float _burretAliveTime = 3f;//弾の持続時間
    private float _moveSpeed = 10f;

    private int _weaponEvolution = default;//武器の進化形態
    private int DamageSeNumber = default;
    private int NumberOfAttacks = 0;

    private List<GameObject> _chaseEnemys = new List<GameObject>();//追跡対象
    private GameObject _worldSpaceCanvas = default;//ダメージテキスト表示のためのキャンバス

    private bool _isChase = false;//追跡中かそうじゃないか
    private bool _isCritical = false;//クリティカルかどうか

    private Vector3 _comparisonPosition = new Vector3();

    [SerializeField] private TextMeshProUGUI _damageText = default;//ダメージテキスト

    [SerializeField] private List<AudioClip> enemyDamageSE = new List<AudioClip>();//攻撃した時のサウンド

    private ChaserCaneAttack ParentWeaponscript = default;//杖についてるスクリプト

    private const int ADDITIONAL_ATTACK_VALUE = 1;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeUp());

        string findTag = "WorldSpaceCanvas";
        _worldSpaceCanvas = GameObject.FindGameObjectWithTag(findTag);

        //弾出現位置を代入
        _comparisonPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isChase && _chaseEnemys[0] !=null)
        {
            Chase();
        }
        else
        {
            NotChase();
        }
       
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        int stagePartsLayer = 12;
        int florLayer = 18;

        //ステージに当たったら
        if (collision.gameObject.layer == stagePartsLayer || collision.gameObject.layer == florLayer)
        {
            //弾を消す
            AdditionalAttackCheck(false,collision.gameObject.tag);
        }

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        //敵に攻撃が当たったら
        if(collision.gameObject.layer==enemyLayer ||
            collision.gameObject.layer==donthitEnemyLayer||
            collision.gameObject.layer == FlyEnemyLayer)
        {
            _isChase = false;
            //ダメージをテキスト表示
            DamageTextDisplay(collision.contacts[0].point);

            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_burretPower, 0,false);
                DamageSeNumber = 1;
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_burretPower);
                DamageSeNumber = 2;
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_burretPower, 0, false);
                DamageSeNumber = 3;
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_burretPower, true);
                DamageSeNumber = 3;
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_burretPower);
                DamageSeNumber = 3;
            }
            ParentWeaponscript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
            //武器が進化していたら
            AdditionalAttackCheck(true,collision.gameObject.tag);
        }
    }

    private void Chase()
    {
        //敵を追尾
        this.transform.localRotation = Quaternion.FromToRotation(Vector3.up, _chaseEnemys[0].transform.position - this.transform.position);
        this.transform.position += transform.up * Time.deltaTime * _moveSpeed;
    }

    private void NotChase()
    {
        //まっすぐ飛んでいく
        this.transform.position += transform.up * Time.deltaTime * _moveSpeed;
    }

    /// <summary>
    /// 弾の追加攻撃
    /// </summary>
    /// <param name="CollisionObjectIsEnemy">敵かステージパーツか　trueはステージパーツ、falseはステージパーツ</param>
    private void AdditionalAttackCheck(bool CollisionObjectIsEnemy,string collisionObjectTag)
    {
        if (CollisionObjectIsEnemy)
        {
            //武器が一度でも進化済みかつ、攻撃回数が０だったら
            if (_weaponEvolution >= ADDITIONAL_ATTACK_VALUE&&NumberOfAttacks==0)
            {
                AdditionalAttack();
            }
            //進化していなかったら
            else
            {
                DestroyObject();
            }
        }
        else
        {
            //武器が一度でも進化済みだったら
            if (_weaponEvolution >= ADDITIONAL_ATTACK_VALUE)
            {
                CollisionStageParts(collisionObjectTag);
            }
            //進化していなかったら
            else
            {
                DestroyObject();
            }
        }
    }

    private void AdditionalAttack()
    {
        switch (_weaponEvolution)
        {
            //進化レベル１の時
            case 1:
                WeaponEvolution1AdditionalAttack();
                break;
        }
    }

   
    private void WeaponEvolution1AdditionalAttack()
    {
        NumberOfAttacks++;
        this.transform.Rotate(0, 0, 180);
        _comparisonPosition=this.transform.position;

        //追尾する敵をリセット
        _chaseEnemys.Clear();
    }

    private void CollisionStageParts(string stagePartsTag)
    {
        string cellingTag = "Celling";
        string florTag = "Flor";
        string wallTag = "Wall";
        //床、天井に当たっていたら
        if (stagePartsTag == cellingTag || stagePartsTag == florTag)
        {
            FlorOrCellingCollision();
        }
        //壁に当たっていたら
        else if (stagePartsTag == wallTag)
        {
            WallCollision();
        }
    }

    private void FlorOrCellingCollision()
    {
        int clockWise = -90;//時計回り
        int counterClockWise = 90;//反時計回り

        //前回曲がった場所から進んだ方向を計算
        Vector3 _burretDerectionFacing = this.transform.position - _comparisonPosition;
        //曲がった場所の更新
        _comparisonPosition = this.transform.position;
        //右上移動
        if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y > 0)
        {
            //時計回りに９０度回転
            this.transform.Rotate(0, 0, clockWise);
        }
        //右下移動
        else if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y < 0)
        {
            //反時計回りに９０度回転
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //左上移動
        else if (_burretDerectionFacing.x < 0 && _burretDerectionFacing.y > 0)
        {
            //反時計回りに９０度回転
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //左下移動
        else
        {
            //時計回りに９０度回転
            this.transform.Rotate(0, 0, clockWise);
        }
    }

    private void WallCollision()
    {
        int clockWise = -90;//時計回り
        int counterClockWise = 90;//反時計回り

        //前回曲がった場所から進んだ方向を計算
        Vector3 _burretDerectionFacing = this.transform.position - _comparisonPosition;

        //曲がった場所の更新
        _comparisonPosition = this.transform.position;
        //右上移動
        if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y > 0)
        {
            //反時計回りに９０度回転
            this.transform.Rotate(0, 0, counterClockWise);
        }
        //右下移動
        else if (_burretDerectionFacing.x > 0 && _burretDerectionFacing.y < 0)
        {
            //時計回りに９０度回転
            this.transform.Rotate(0, 0, clockWise);
        }
        //左上移動
        else if (_burretDerectionFacing.x < 0 && _burretDerectionFacing.y > 0)
        {
            //時計回りに９０度回転
            this.transform.Rotate(0, 0, clockWise);
        }
        //左下移動
        else
        {
            //反時計回りに９０度回転
            this.transform.Rotate(0, 0, counterClockWise);
        }
    }

  
    private void DamageTextDisplay(Vector3 DisplayPoint)
    {
        //トリガーが当たった位置に生成する
        TextMeshProUGUI _damageTextDummy = Instantiate(_damageText, DisplayPoint, Quaternion.identity);
        //キャンバスを親にする
        _damageTextDummy.transform.SetParent(_worldSpaceCanvas.transform, true);

        //ダメージ値をテキストに入れる
        _damageTextDummy.text = (_burretPower).ToString();

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        int enemyLayer = 10;
        int dontHitEnemyLayer = 20;
        int flyEnemyLayer = 22;

        //トリガーしたものが敵だったら
        if (collision.gameObject.layer == enemyLayer|| 
            collision.gameObject.layer == dontHitEnemyLayer||
            collision.gameObject.layer == flyEnemyLayer)
        {
            //追尾する敵リストに追加
            _chaseEnemys.Add(collision.gameObject);
            //追尾開始
            _isChase = true;
        }
    }

    public void GetPower(float AttackPower, ChaserCaneAttack weaponScript,bool isCriticalJudge, int weaponEvolutionNumber)
    {
        _burretPower = AttackPower;
        ParentWeaponscript = weaponScript;
        _isCritical = isCriticalJudge;

        _weaponEvolution = weaponEvolutionNumber;
        
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }

    private IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(_burretAliveTime);
        DestroyObject();
        DamageSeNumber = 0;
        ParentWeaponscript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
    }
}
