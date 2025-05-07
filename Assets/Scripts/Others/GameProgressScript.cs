using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameProgressScript : MonoBehaviour
{
    private bool _isFadeOut = false;//�t�F�[�h�A�E�g����
    private bool _isWhiteFadeOut = false;//true�ɂȂ�Ɖ�ʂ������Ȃ��Ă���
    private bool _isBlackFadeOut = false;//true�ɂȂ�Ɖ�ʂ������Ȃ��Ă���
    private bool _isGameStart = false;//�Q�[���J�n������Ă��邩
    private bool _isEvent = false;//��O���Q�[���C�x���g������t���A��
    private bool _isPreBossFlor = false;//��O���{�X�t���A��

    private float _stageLevel = 1;//�K�w�̃��x��

    private float _hitPointpStrength = 0f;//�オ��̗͂̔{��
    private float _hitPointpStrengthUpValue = 1.5f;//�オ���Ă����̗̗͂�
    private float _attackPowerStrength = 0f;//�オ��U���͂̔{��
    private float _attackPowerStrengthUpValue = 1.35f;//�オ���Ă����U���̗͂�
  
    private const float DEPTH_STATUS_UP_VALUE = 0.5f;//�[�x�ɂ���ēG�̃X�e�[�^�X������ɏグ��

    private int _eventCount = 0;//�ʉ߂����Q�[���C�x���g�̉�
    private int _currentFloor = 0;//���݂̊K�w
    private int _bossFloor = 10;//���̃{�X�K�w
    private int _changeStageBGMValue = 0;//�ς���ʏ퉹�y�̃i���o�[

    private const int CLEAR_STAGE_FLOOR = 30;//�N���A�K�w
    private const int START_ENEMY_STATUS_INCREASED_FLOOR = 11;

    private string _floorString = "B";

    [SerializeField] private GameObject _startStage = default;//��ԍŏ��̃X�e�[�W
    [SerializeField] private GameObject _gameOverStage = default;
    [SerializeField] private GameObject _clearStage = default;
    [SerializeField] private GameObject _player = default;

    private GameObject _preStage = default;//��O�ɐ��������X�e�[�W
    private GameObject _restartStageParts = default;//������x�����Ƃ���Ń��X�^�[�g�������ۂɏo���X�e�[�W

    [Header("�X�e�[�W���x���P�̃p�[�c"), SerializeField] private List<GameObject> _stagePartsLv1 = new List<GameObject>();
    [Header("�X�e�[�W���x��2�̃p�[�c"), SerializeField] private List<GameObject> _stagePartsLv2 = new List<GameObject>();
    [Header("�{�X�t���A�p�[�c"), SerializeField] private List<GameObject> _bossFlorParts = new List<GameObject>();
    [Header("�C�x���g�t���A�̃p�[�c"), SerializeField] private List<GameObject> _eventFlorParts = new List<GameObject>();

    [SerializeField] private Image _blackFadeOutPanel = default;
    [SerializeField] private Image _whiteFadeOutPanel = default;

    [SerializeField] private TextMeshProUGUI _floorText = default;//�K�w�̃e�L�X�g

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
        {//��ʂ����F�ɂ��Ă���
            BlackFadeOut();
        }

        if (_isWhiteFadeOut)
        {
            //��ʂ𔒐F�ɂ��Ă���
            WhiteFade(true);
        }
        else if (_whiteFadeOutPanel.color.a >= 0)
        {
            //��ʂ����X�Ɍ��ɖ߂�
            WhiteFade(false);
        }
        //��x�t�F�[�h�A�E�g���Ă��āA��ʂ����ɖ߂�����
        else if (_isFadeOut)
        {
            //�X�e�[�W����
            _isFadeOut = false;
            NextStage();
        }
    }
    public void FadeOut()
    {
        //��ʂ����F�ɂ���
        _isBlackFadeOut = true;
    }

    /// <summary>
    /// ��ʂ𔒐F�ɂ���A�������͉�ʂ�߂�
    /// </summary>
    /// <param name="fadeJudge">false�̓t�F�[�h�A�E�g�Atrue�̓t�F�[�h�C��</param>
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

        //��ʂ��Â����Ă���
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

        //��ԍŏ��̃X�e�[�W������
        _startStage.SetActive(false);

        NextStage();

        //�v���C���[�̈ʒu��0,0,0�Ƀ��Z�b�g
        _player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        _player.transform.position = Vector3.zero;

        //��ʂ����ɖ߂�
        _blackFadeOutPanel.color = Color.clear;

        //BGM��ύX
        _changeStageBGMValue=1;
        _bgmChangeScript.changeStageBGM(_changeStageBGMValue);

    }

    public void Worp()
    {
        _isWhiteFadeOut = true;
    }

    private void NextStage()
    {
        //�v���C���[�̗̑́AMp�X�e�[�^�X���}�b�N�X�ɖ߂�
        _playerStatusChange.HitPointAndMagicPointReset();
        int randomValue = default;

        //���̊K�w�ɐi�߂�
        _currentFloor++;
        _floorText.text = _floorString + _currentFloor.ToString();

        //�����K�w�e�L�X�g�������Ȃ���Ԃ�������
        if (!_floorText.enabled)
        {
            //�������Ԃɂ���
            _floorText.enabled = true;
            _cameraScript.Backgroundchange();
        }

        const int FIRST_EVENT_FLOR = 21;
        const int SECOND_EVENT_FLOR = 41;
        const int THIRD_EVENT_FLOR = 61;
        const int FORTH_EVENT_FLOR = 81;
        const int FIFTH_EVENT_FLOR = 91;


        //20F,40F,,50F,71f,91F�̃{�X��|�����ゾ������Q�[���C�x���g����
        if ((_currentFloor == FIRST_EVENT_FLOR || 
            _currentFloor == SECOND_EVENT_FLOR || 
            _currentFloor == THIRD_EVENT_FLOR ||
           _currentFloor == FORTH_EVENT_FLOR || 
           _currentFloor == FIFTH_EVENT_FLOR) && 
           !_isEvent)
        {
            //BGM���~�߂�
            _bgmChangeScript.BGMStop();
            //�K�w�e�L�X�g�������Ȃ�����
            _floorText.enabled = false;
            //���̊K�w���Q�PF�ɂ��邽�߁A�P������
            _currentFloor--;
            //�C�x���g�g���K�[��true�ɂ���
            _isEvent = true;

            DestroyStage();
            _preStage = Instantiate(_eventFlorParts[_eventCount], Vector3.zero, Quaternion.identity);

            //�v���C���[�̈ʒu���O-�O-�O��
            _player.transform.position = Vector3.zero;

            _changeStageBGMValue++;
            _eventCount++;
            _stageLevel++;
        }

        //���݂̊K�w���ō��K�w��������
        else if(_currentFloor==CLEAR_STAGE_FLOOR)
        {
            //�O�̃X�e�[�W���폜
            DestroyStage();

            //�N���A�K�w�𐶐�
            Instantiate(_clearStage, Vector3.zero, Quaternion.identity);
            
            //�v���C���[�̈ʒu���Z�b�g
            _player.transform.position= Vector3.zero;

            _bgmChangeScript.BGMStop();
        }

        //currentFloor��10�A20�A30�E�E�E�ɂȂ�����
        else if (_currentFloor == _bossFloor)
        {
            //���̃{�X�t���A����
            int _nextBossflor = 10;
            _bossFloor += _nextBossflor;

            int bossFlorAdjustment = 10;
            int listAdjustment = 1;

            //�O�̃X�e�[�W���폜
            DestroyStage();
            //�{�X�t���A����
            _preStage = Instantiate(_bossFlorParts[_currentFloor / bossFlorAdjustment - listAdjustment], Vector3.zero, Quaternion.identity);

            //���X�^�[�g�������̃X�e�[�W�����炩���ߊi�[
            _restartStageParts = _bossFlorParts[_currentFloor / bossFlorAdjustment - listAdjustment];

            //�v���C���[�̈ʒu���O-�O-�O��
            _player.transform.position = Vector3.zero;

            _isPreBossFlor = true;
            
        }

        //���ʂ̃X�e�[�W����
        else
        {
            if (_isPreBossFlor)
            {
                _bgmChangeScript.changeStageBGM(_changeStageBGMValue);
            }

            switch (_stageLevel)
            {


                case 1://�X�e�[�W���x���P�̎�
                    #region �X�e�[�W��������
                    randomValue = Random.Range(0, _stagePartsLv1.Count);

                    DestroyStage();
                    _preStage = Instantiate(_stagePartsLv1[randomValue], Vector3.zero, Quaternion.identity);
                    
                    _player.transform.position = Vector3.zero;

                    //���X�^�[�g�������̃X�e�[�W�����炩���ߊi�[
                    _restartStageParts = _stagePartsLv1[randomValue];

                    //�O�񂪃{�X�t���A�������瑝����{�����グ�Ă���
                    if (_isPreBossFlor)
                    {

                        //HP�ɔ{��������
                        _hitPointpStrength += _hitPointpStrengthUpValue;

                        //�[�x�ɂ���Ĕ{�����グ��
                        _hitPointpStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        //�U���͂ɔ{��������
                        _attackPowerStrength += _attackPowerStrengthUpValue;

                        //�[�x�ɂ���Ĕ{�����グ��
                        _attackPowerStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        _isPreBossFlor = false;
                    }


                    //�O�񂪃{�X�t���A��������G��HP�{���A�U���{�����グ�Ă���
                    if (_currentFloor >= START_ENEMY_STATUS_INCREASED_FLOOR)
                    {
                        _preStage.GetComponent<StageCoreScript>().GetIncreasedvalue(_hitPointpStrength, _attackPowerStrength);
                    }


                    #endregion
                    break;

                case 2://�X�e�[�W���x���Q�̎�
                    #region �X�e�[�W��������
                    randomValue = Random.Range(0, _stagePartsLv2.Count);

                    DestroyStage();
                    _preStage = Instantiate(_stagePartsLv2[randomValue], Vector3.zero, Quaternion.identity);
                    _player.transform.position = Vector3.zero;

                    //���X�^�[�g�������̃X�e�[�W�����炩���ߊi�[
                    _restartStageParts = _stagePartsLv2[randomValue];


                    //�O�񂪃{�X�t���A�������瑝����{�����グ�Ă���
                    if (_isPreBossFlor)
                    {

                        //HP�ɔ{��������
                        _hitPointpStrength += _hitPointpStrengthUpValue;

                        //�[�x�ɂ���Ĕ{�����グ��
                        _hitPointpStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        //�U���͂ɔ{��������
                        _attackPowerStrength += _attackPowerStrengthUpValue;

                        //�[�x�ɂ���Ĕ{�����グ��
                        _attackPowerStrengthUpValue += DEPTH_STATUS_UP_VALUE;

                        _isPreBossFlor = false;
                    }


                    //�P�P�K�w�ȍ~��������G�̃X�e�[�^�X���グ��
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
        //�O�̃X�e�[�W������
        DestroyStage();

        //BGM��ς���
        _bgmChangeScript.changeStageBGM(_changeStageBGMValue);

        _preStage =Instantiate(_restartStageParts, Vector3.zero, Quaternion.identity);
        _player.transform.position = Vector3.zero;
        _player.GetComponent<PlayerMoveControll>().Retry();

        //�P�P�K�w�ȍ~��������G�̃X�e�[�^�X���グ��
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
        //��O�ɂ���X�e�[�W������
        Destroy(_preStage);
    }
}
