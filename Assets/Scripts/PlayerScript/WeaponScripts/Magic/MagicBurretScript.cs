using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagicBurretScript : MonoBehaviour
{
    private float _burretPower = 0;//攻撃力
    private float _knockBackPower = 6.5f;//ノックバックの大きさ
    private float _moveSpeed = 10f;//移動するスピード

    private int _burretLevel = default;//弾のレベル（杖のレベルと一緒）

    private bool _isCritical = false;

    [SerializeField] private List<AudioClip> enemyDamageSE = new List<AudioClip>();

    private MagicStickAttack _parentWeaponScript = default;

    [SerializeField] private TextMeshProUGUI _damageText = default;//ダメージテキスト

    private GameObject _worldSpaceCanvas = default;//ダメージテキスト表示のためのキャンバス

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeUp());
        _worldSpaceCanvas = GameObject.FindGameObjectWithTag("WorldSpaceCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += transform.up*Time.deltaTime*_moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        int stagePartsLayer = 12;
        int florLayer = 18;
        
        //壁、床、天井のいずれかに当たったら＆武器の進化形態が０だったら
        if (_burretLevel<=0&&(collision.gameObject.layer == stagePartsLayer||collision.gameObject.layer==florLayer))
        {
            //球を消す
            DestroyObject(0);
        }

        int enemyLayer = 10;
        int donthitEnemyLayer = 20;
        int FlyEnemyLayer = 22;

        //敵に当たったら
        if(collision.gameObject.layer == enemyLayer || collision.gameObject.layer == donthitEnemyLayer || 
            collision.gameObject.layer == FlyEnemyLayer)
        {
            DamageTextDisplay(collision.contacts[0].point);
            if (collision.gameObject.CompareTag("NormalEnemy"))
            {
                collision.gameObject.GetComponent<NormalEnemyScript>().BeAttacked(_burretPower, _knockBackPower,true);
                DestroyObject(1);
            }
            else if (collision.gameObject.CompareTag("TurretEnemy"))
            {
                collision.gameObject.GetComponent<TurretEnemyScript>().BeAttacked(_burretPower);
                DestroyObject(2);
            }
            else if (collision.gameObject.CompareTag("ChaserEnemy"))
            {
                collision.gameObject.GetComponent<ChaserEnemyScript>().BeAttacked(_burretPower, _knockBackPower,true);
                DestroyObject(3);
            }
            else if (collision.gameObject.CompareTag("FlyEnemy"))
            {
                collision.gameObject.GetComponent<FlyEnemyScript>().BeAttacked(_burretPower,true);
                DestroyObject(3);
            }
            else if (collision.gameObject.CompareTag("Boss"))
            {
                collision.gameObject.GetComponent<IBossStatus>().TakeDamage(_burretPower);
                DestroyObject(3);
            }
        }
       
    }
    public void DamageTextDisplay(Vector3 DisplayPoint)
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

    public void GetPower(float AttackPower, MagicStickAttack weaponScript,bool isCriticalJudge,int weaponEvolution)
    {
        //攻撃力、クリティカル判定を取得
        if (weaponEvolution > 0)
        {
            int DontStagePartsHitLayer = 26;
            this.gameObject.layer = DontStagePartsHitLayer;
        }

        _burretPower = AttackPower;
        _parentWeaponScript = weaponScript;
        _isCritical = isCriticalJudge;
    }

    private void DestroyObject( int DamageSeNumber)
    {
        if(_parentWeaponScript != null)
        {
            _parentWeaponScript.ReProductionSE(enemyDamageSE[DamageSeNumber]);
        }
     
        Destroy(this.gameObject);
    }

    private IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(3);
        DestroyObject(0);
    }
}
