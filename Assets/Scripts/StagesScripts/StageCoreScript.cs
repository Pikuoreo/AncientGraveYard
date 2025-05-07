using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StageCoreScript : MonoBehaviour
{
    [SerializeField] List<SummonEnemyScript> _summonEnemyScripts = new List<SummonEnemyScript>();//フェーズ１の敵召喚スクリプト
    [SerializeField] List<SummonEnemyScript> _summonEnemyScripts2= new List<SummonEnemyScript>();//フェーズ2の敵召喚スクリプト
    [SerializeField] List<summonBossScript> _summonBossScript = new List<summonBossScript>();//通常一体だが、２体同時ボスも考慮してリスト型にする

    [SerializeField]private GameObject _entranceDoor = default;//入口
    [SerializeField] private GameObject _exitDoor = default;//出口

    private BoxCollider2D _entranceDoorCollider = default;//入口のボックスコライダー
    private BoxCollider2D _exitDoorCollider = default;//出口のボックスコライダー

    private SpriteRenderer _entranceDoorSpriteRendere = default;//入口の見た目
    private SpriteRenderer _exitDoorSpriteRendere = default;//出口の見た目

    [SerializeField] TresureScript _tresureBox = default;

    private bool _isButtleStart = false;//プレイヤーが戦闘領域に入ったか
    private bool _isButtleEnd = false;//戦闘が終わったか
    private bool _isFase2 = false;//二回目の敵召喚があるか
    [SerializeField] private bool _isBossFloor = false;

    private bool _isEntranceRock = false;//入口のロック
    private bool _isExitRock = false;//出口のロック

    private int _lestenemynumber = 0;//残りの敵の数
    private int _lestbossNumber = 0;//残りのボスの数

    private const int TIME_DELTATIME = 750;

    private float _hitPointIncreased = 1;//敵の体力を上げる値
    private float _attackPowerIncreased = 1;//敵の攻撃力を上げる値

    private float _colorChangeSpeed = 0.001f;
    

    // Start is called before the first frame update
    void Start()
    {
        //入口のコンポーネントと取得
        _entranceDoorCollider = _entranceDoor.GetComponent<BoxCollider2D>();
        _entranceDoorSpriteRendere = _entranceDoor.GetComponent<SpriteRenderer>();

        //出口のコンポーネント取得
        _exitDoorCollider = _exitDoor.GetComponent<BoxCollider2D>();
        _exitDoorSpriteRendere = _exitDoor.GetComponent<SpriteRenderer>();

        //残りの敵の数は入ってるスクリプトの数
        _lestenemynumber = _summonEnemyScripts.Count;
        _lestbossNumber = _summonBossScript.Count;
    }

    // Update is called once per frame
    void Update()
    {
        //戦闘開始した時
        if (_isButtleStart)
        {
            EntranceAndExitRock();
        }

        //戦闘が終わった時
        if (_isButtleEnd)
        {
            EntranceAndExitAnRock();
        }

    }

    private void EntranceAndExitRock()
    {
        int maxColorA = 1;

        //入口と出口を塞ぐ
        if (_entranceDoorSpriteRendere.color.a <= maxColorA)
        {
            _entranceDoorSpriteRendere.color += new Color(0, 0, 0, _colorChangeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }
        else
        {
            _isEntranceRock = true;
        }

        if (_exitDoorSpriteRendere.color.a <= maxColorA)
        {
            _exitDoorSpriteRendere.color += new Color(0, 0, 0, _colorChangeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }
        else
        {
            _isExitRock = true;
        }

        if (_isEntranceRock && _isExitRock)
        {
            _isButtleStart = false;
        }
    }

    private void EntranceAndExitAnRock()
    {
        //入口と出口を開放する
        if (_entranceDoorSpriteRendere.color.a >= 0)
        {
            _entranceDoorSpriteRendere.color -= new Color(0, 0, 0, _colorChangeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }

        if (_exitDoorSpriteRendere.color.a >= 0)
        {
            _exitDoorSpriteRendere.color -= new Color(0, 0, 0, _colorChangeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }
    }

    public void GetIncreasedvalue(float hitPointIncreasedValue , float attackPowerIncreasedValue)
    {
        _hitPointIncreased = hitPointIncreasedValue;
        _attackPowerIncreased = attackPowerIncreasedValue;
    }

    public void ButtleStart()
    {
        //ボス階層だったら
        if (_isBossFloor)
        {
            //このスクリプトの取得
            StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

            for (int _summonNum = 0; _summonNum < _lestbossNumber; _summonNum++)
            {
                //敵を召喚するスクリプトのメソッドを呼び出す
                _summonBossScript[_summonNum].SummonBossPreparation();
            }
            
        }
        //ボス階層以外だったら
        else
        {
            //入口、出口を塞ぐ
            _isButtleStart = true;
            _entranceDoorCollider.enabled = true;
            _exitDoorCollider.enabled = true;

            //このスクリプトの取得
            StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

            for (int _summonNum = 0; _summonNum < _lestenemynumber; _summonNum++)
            {
                //敵を召喚するスクリプトのメソッドを呼び出す
                _summonEnemyScripts[_summonNum].SummonEnemyPreparation(_thisScript,_hitPointIncreased,_attackPowerIncreased);

            }
        }
       

    }

    public void ButtleEnd()
    {
        //ボス階層だったら
        if (_isBossFloor)
        {
            _lestbossNumber--;
            if (_lestbossNumber == 0)
            {
                _tresureBox.RandomItem(this.gameObject);
                _isButtleEnd = true;
                _entranceDoorCollider.enabled = false;
                _exitDoorCollider.enabled = false;
            }
        }
        //ボス階層以外だったら
        else
        {
            //残りの敵の数
            _lestenemynumber--;

            if (_lestenemynumber == 0 && _summonEnemyScripts2.Count > 0 && !_isFase2)
            {
                _lestenemynumber = _summonEnemyScripts2.Count;
                //このスクリプトの取得
                StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

                for (int _summonNum = 0; _summonNum < _lestenemynumber; _summonNum++)
                {
                    //敵を召喚するスクリプトのメソッドを呼び出す
                    _summonEnemyScripts2[_summonNum].SummonEnemyPreparation(_thisScript, _hitPointIncreased, _attackPowerIncreased);
                }
                _isFase2 = true;
            }

            //ゼロになったら入口、出口の開放
            if (_lestenemynumber == 0)
            {
                _tresureBox.RandomItem(this.gameObject);
                _isButtleEnd = true;
                _entranceDoorCollider.enabled = false;
                _exitDoorCollider.enabled = false;
            }
        }
       
    }
}
