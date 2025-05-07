using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressScript : MonoBehaviour
{
    private bool _isFadeOut = false;//フェードアウト中か
    private bool _isWhiteFadeOut = false;//trueになると画面が白くなっていく
    private bool _isBlackFadeOut = false;//trueになると画面が黒くなっていく
    private bool _isGameStart = false;//ゲーム開始がされているか
    private bool _isEvent = false;//一つ前がゲームイベントがあるフロアか
    private bool _isPreBossFlor = false;//一つ前がボスフロアか

    private float _stageLevel = 1;//階層のレベル

    private float _hitPointpStrength = 0f;//上がる体力の倍率
    private float _hitPointpStrengthUpValue = 1.5f;//上がっていく体力の量
    private float _attackPowerStrength = 0f;//上がる攻撃力の倍率
    private float _attackPowerStrengthUpValue = 1.35f;//上がっていく攻撃力の量
  
    private const float DEPTH_STATUS_UP_VALUE = 0.5f;//深度によって敵のステータスをさらに上げる

    private int _eventCount = 0;//通過したゲームイベントの回数
    private int _currentFloor = 0;//現在の階層
    private int _bossFloor = 10;//次のボス階層
    private int _changeStageBGMValue = 0;//変える通常音楽のナンバー

    private const int CLEAR_STAGE_FLOOR = 30;//クリア階層
    private const int START_ENEMY_STATUS_INCREASED_FLOOR = 11;

    private string _floorString = "B";

    [SerializeField] private GameObject _startStage = default;//一番最初のステージ
    [SerializeField] private GameObject _gameOverStage = default;
    [SerializeField] private GameObject _clearStage = default;
    [SerializeField] private GameObject _player = default;

    private GameObject _preStage = default;//一つ前に生成したステージ
    private GameObject _restartStageParts = default;//もう一度同じところでリスタートをした際に出すステージ

    [Header("ステージレベル１のパーツ"), SerializeField] private List<GameObject> _stagePartsLv1 = new List<GameObject>();
    [Header("ステージレベル2のパーツ"), SerializeField] private List<GameObject> _stagePartsLv2 = new List<GameObject>();
    [Header("ボスフロアパーツ"), SerializeField] private List<GameObject> _bossFlorParts = new List<GameObject>();
    [Header("イベントフロアのパーツ"), SerializeField] private List<GameObject> _eventFlorParts = new List<GameObject>();

    [SerializeField] private Image _blackFadeOutPanel = default;
    [SerializeField] private Image _whiteFadeOutPanel = default;

    [SerializeField] private TextMeshProUGUI _floorText = default;//階層のテキスト

    [SerializeField] private CameraScript _cameraScript = default;
    [SerializeField] private BGMChangeScript _bgmChangeScript = default;
    [SerializeField] private PlayerStatusChange _playerStatusChange = default;

    void Start()
    {
        _floorText.text = _floorString+ _currentFloor.ToString();
    }
    void Update()
    {
        if (_isBlackFadeOut)
        {//画面を黒色にしていく
            BlackFadeOut();
        }

        if (_isWhiteFadeOut)
        {
            //画面を白色にしていく
            WhiteFade(true);
        }
        else if (_whiteFadeOutPanel.color.a >= 0)
        {
            //画面を徐々に元に戻す
            WhiteFade(false);
        }
        //一度フェードアウトしていて、画面が元に戻ったら
        else if (_isFadeOut)
        {
            //ステージ生成
            _isFadeOut = false;
            NextStage();
        }
    }
    public void FadeOut()
    {
        //画面を黒色にする
        _isBlackFadeOut = true;
    }

    /// <summary>
    /// 画面を白色にする、もしくは画面を戻す
    /// </summary>
    /// <param name="fadeJudge">falseはフェードアウト、trueはフェードイン</param>
    private void WhiteFade(bool fadeJudge)
    {
        float fadeSpeed = 0.007f;

        const int TIME_DELTATIME = 1000;
        if (fadeJudge)
        {
            if (!_isFadeOut)
            {
                _isFadeOut = true;
            }
            _whiteFadeOutPanel.color += new Color(0, 0, 0, fadeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }
        else
        {
            _whiteFadeOutPanel.color -= new Color(0, 0, 0, fadeSpeed) * Time.deltaTime * TIME_DELTATIME;
        }

        if (_whiteFadeOutPanel.color.a >= 1)
        {
            _isWhiteFadeOut = false;
        }
    }

    private void BlackFadeOut()
    {
        float fadeOutSpeeD = 0.001f;
        const int TIME_DELTATIME = 500;

        //画面を暗くしていく
        _blackFadeOutPanel.color += new Color(0, 0, 0, fadeOutSpeeD) * Time.deltaTime * TIME_DELTATIME;

        if (_blackFadeOutPanel.color.a >= 1 && !_isGameStart)
        {
            _isGameStart = true;
            _isBlackFadeOut = false;
            StartCoroutine(GameStart());
        }
    }
    private IEnumerator GameStart()
    {
        int waitTime = 3;
        yield return new WaitForSeconds(waitTime);

        //一番最初のステージを消す
        _startStage.SetActive(false);

        NextStage();

        //プレイヤーの位置を0,0,0にリセット
        _player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _player.transform.position = Vector3.zero;

        //画面を元に戻す
        _blackFadeOutPanel.color = Color.clear;

        //BGMを変更
        _changeStageBGMValue=1;
        _bgmChangeScript.changeStageBGM(_changeStageBGMValue);

    }

    public void Worp()
    {
        _isWhiteFadeOut = true;
    }

    private void NextStage()
    {
        //プレイヤーの体力、Mpステータスをマックスに戻す
        _playerStatusChange.HitPointAndMagicPointReset();
        int randomValue = default;

        //次の階層に進める
        _currentFloor++;
        _floorText.text = _floorString + _currentFloor.ToString();

        //もし階層テキストが見えない状態だったら
        if (!_floorText.enabled)
        {
            //見える状態にする
            _floorText.enabled = true;
            _cameraScript.Backgroundchange();
        }

        const int FIRST_EVENT_FLOR = 21;
        const int SECOND_EVENT_FLOR = 41;
        const int THIRD_EVENT_FLOR = 61;
        const int FORTH_EVENT_FLOR = 81;
        const int FIFTH_EVENT_FLOR = 91;


        //20F,40F,,50F,71f,91Fのボスを倒した後だったらゲームイベント発生
        if ((_currentFloor == FIRST_EVENT_FLOR || 
            _currentFloor == SECOND_EVENT_FLOR || 
            _currentFloor == THIRD_EVENT_FLOR ||
           _currentFloor == FORTH_EVENT_FLOR || 
           _currentFloor == FIFTH_EVENT_FLOR) && 
           !_isEvent)
        {
            //BGMを止める
            _bgmChangeScript.BGMStop();
            //階層テキストを見えなくする
            _floorText.enabled = false;
            //次の階層を２１Fにするため、１下げる
            _currentFloor--;
            //イベントトリガーをtrueにする
            _isEvent = true;

            DestroyStage();
            _preStage = Instantiate(_eventFlorParts[_eventCount], Vector3.zero, Quaternion.identity);

            //プレイヤーの位置を０-０-０に
            _player.transform.position = Vector3.zero;

            _changeStageBGMValue++;
            _eventCount++;
            _stageLevel++;
        }

        //現在の階層が最高階層だったら
        else if(_currentFloor==CLEAR_STAGE_FLOOR)
        {
            //前のステージを削除
            DestroyStage();

            //クリア階層を生成
            Instantiate(_clearStage, Vector3.zero, Quaternion.identity);
            
            //プレイヤーの位置リセット
            _player.transform.position= Vector3.zero;

            _bgmChangeScript.BGMStop();
        }

        //currentFloorが10、20、30・・・になったら
        else if (_currentFloor == _bossFloor)
        {
            //次のボスフロアを代入
            int _nextBossflor = 10;
            _bossFloor += _nextBossflor;

            int bossFlorAdjustment = 10;
            int listAdjustment = 1;

            //前のステージを削除
            DestroyStage();
            //ボスフロア生成
            _preStage = Instantiate(_bossFlorParts[_currentFloor / bossFlorAdjustment - listAdjustment], Vector3.zero, Quaternion.identity);

            //リスタートした時のステージをあらかじめ格納
            _restartStageParts = _bossFlorParts[_currentFloor / bossFlorAdjustment - listAdjustment];

            //プレイヤーの位置を０-０-０に
            _player.transform.position = Vector3.zero;

            _isPreBossFlor = true;
            
        }

        //普通のステージ生成
        else
        {
            if (_isPreBossFlor)
            {
                _bgmChangeScript.changeStageBGM(_changeStageBGMValue);
            }

            switch (_stageLevel)
            {


                case 1://ステージレベル１の時
                    #region ステージ生成処理
                    randomValue = Random.Range(0, _stagePartsLv1.Count);

                    DestroyStage();
                    _preStage = Instantiate(_stagePartsLv1[randomValue], Vector3.zero, Quaternion.identity);
                    
                    _player.transform.position = Vector3.zero;

                    //リスタートした時のステージをあらかじめ格納
                    _restartStageParts = _stagePartsLv1[randomValue];

                    //前回がボスフロアだったら増える倍率を上げていく
                    if (_isPreBossFlor)
                    {

                        //HPに倍率をつける
                        _hitPointpStrength += _hitPointpStrengthUpValue;

                        //深度によって倍率を上げる
                        _hitPointpStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        //攻撃力に倍率をつける
                        _attackPowerStrength += _attackPowerStrengthUpValue;

                        //深度によって倍率を上げる
                        _attackPowerStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        _isPreBossFlor = false;
                    }


                    //前回がボスフロアだったら敵のHP倍率、攻撃倍率を上げていく
                    if (_currentFloor >= START_ENEMY_STATUS_INCREASED_FLOOR)
                    {
                        _preStage.GetComponent<StageCoreScript>().GetIncreasedvalue(_hitPointpStrength, _attackPowerStrength);
                    }


                    #endregion
                    break;

                case 2://ステージレベル２の時
                    #region ステージ生成処理
                    randomValue = Random.Range(0, _stagePartsLv2.Count);

                    DestroyStage();
                    _preStage = Instantiate(_stagePartsLv2[randomValue], Vector3.zero, Quaternion.identity);
                    _player.transform.position = Vector3.zero;

                    //リスタートした時のステージをあらかじめ格納
                    _restartStageParts = _stagePartsLv2[randomValue];


                    //前回がボスフロアだったら増える倍率を上げていく
                    if (_isPreBossFlor)
                    {

                        //HPに倍率をつける
                        _hitPointpStrength += _hitPointpStrengthUpValue;

                        //深度によって倍率を上げる
                        _hitPointpStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        //攻撃力に倍率をつける
                        _attackPowerStrength += _attackPowerStrengthUpValue;

                        //深度によって倍率を上げる
                        _attackPowerStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        _isPreBossFlor = false;
                    }


                    //１１階層以降だったら敵のステータスを上げる
                    if (_currentFloor >= START_ENEMY_STATUS_INCREASED_FLOOR)
                    {
                        _preStage.GetComponent<StageCoreScript>().GetIncreasedvalue(_hitPointpStrength, _attackPowerStrength);
                    }
                    #endregion
                    break;

            }
            _isEvent = false;
        }
    }

    public void RestartStage()
    {
        //前のステージを消す
        DestroyStage();

        //BGMを変える
        _bgmChangeScript.changeStageBGM(_changeStageBGMValue);

        _preStage =Instantiate(_restartStageParts, Vector3.zero, Quaternion.identity);
        _player.transform.position = Vector3.zero;
        _player.GetComponent<PlayerMoveControll>().Retry();

        //１１階層以降だったら敵のステータスを上げる
        if (_currentFloor >= START_ENEMY_STATUS_INCREASED_FLOOR)
        {
            _preStage.GetComponent<StageCoreScript>().GetIncreasedvalue(_hitPointpStrength, _attackPowerStrength);
        }
    }
    public void GameOver() 
    {
        if (_isPreBossFlor)
        {
            _cameraScript.ReMatchBossBattle();
        }
        DestroyStage();
        _preStage = Instantiate(_gameOverStage,Vector3.zero,Quaternion.identity);
        _player.transform.position = Vector3.zero;
       
    }
    private void DestroyStage()
    {
        //一つ前にあるステージを消す
        Destroy(_preStage);
    }
}
