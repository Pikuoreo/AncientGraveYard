using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StageCoreScript : MonoBehaviour
{
    [SerializeField] List<SummonEnemyScript> _summonEnemyScripts = new List<SummonEnemyScript>();//�t�F�[�Y�P�̓G�����X�N���v�g
    [SerializeField] List<SummonEnemyScript> _summonEnemyScripts2= new List<SummonEnemyScript>();//�t�F�[�Y2�̓G�����X�N���v�g
    [SerializeField] List<summonBossScript> _summonBossScript = new List<summonBossScript>();//�ʏ��̂����A�Q�̓����{�X���l�����ă��X�g�^�ɂ���

    [SerializeField]private GameObject _entranceDoor = default;//����
    [SerializeField] private GameObject _exitDoor = default;//�o��

    private BoxCollider2D _entranceDoorCollider = default;//�����̃{�b�N�X�R���C�_�[
    private BoxCollider2D _exitDoorCollider = default;//�o���̃{�b�N�X�R���C�_�[

    private SpriteRenderer _entranceDoorSpriteRendere = default;//�����̌�����
    private SpriteRenderer _exitDoorSpriteRendere = default;//�o���̌�����

    [SerializeField] TresureScript _tresureBox = default;

    private bool _isButtleStart = false;//�v���C���[���퓬�̈�ɓ�������
    private bool _isButtleEnd = false;//�퓬���I�������
    private bool _isFase2 = false;//���ڂ̓G���������邩
    [SerializeField] private bool _isBossFloor = false;

    private bool _isEntranceRock = false;//�����̃��b�N
    private bool _isExitRock = false;//�o���̃��b�N

    private int _lestenemynumber = 0;//�c��̓G�̐�
    private int _lestbossNumber = 0;//�c��̃{�X�̐�

    private const int TIME_DELTATIME = 750;

    private float _hitPointIncreased = 1;//�G�̗̑͂��グ��l
    private float _attackPowerIncreased = 1;//�G�̍U���͂��グ��l

    private float _colorChangeSpeed = 0.001f;
    

    // Start is called before the first frame update
    void Start()
    {
        //�����̃R���|�[�l���g�Ǝ擾
        _entranceDoorCollider = _entranceDoor.GetComponent<BoxCollider2D>();
        _entranceDoorSpriteRendere = _entranceDoor.GetComponent<SpriteRenderer>();

        //�o���̃R���|�[�l���g�擾
        _exitDoorCollider = _exitDoor.GetComponent<BoxCollider2D>();
        _exitDoorSpriteRendere = _exitDoor.GetComponent<SpriteRenderer>();

        //�c��̓G�̐��͓����Ă�X�N���v�g�̐�
        _lestenemynumber = _summonEnemyScripts.Count;
        _lestbossNumber = _summonBossScript.Count;
    }

    // Update is called once per frame
    void Update()
    {
        //�퓬�J�n������
        if (_isButtleStart)
        {
            EntranceAndExitRock();
        }

        //�퓬���I�������
        if (_isButtleEnd)
        {
            EntranceAndExitAnRock();
        }

    }

    private void EntranceAndExitRock()
    {
        int maxColorA = 1;

        //�����Əo�����ǂ�
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
        //�����Əo�����J������
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
        //�{�X�K�w��������
        if (_isBossFloor)
        {
            //���̃X�N���v�g�̎擾
            StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

            for (int _summonNum = 0; _summonNum < _lestbossNumber; _summonNum++)
            {
                //�G����������X�N���v�g�̃��\�b�h���Ăяo��
                _summonBossScript[_summonNum].SummonBossPreparation();
            }
            
        }
        //�{�X�K�w�ȊO��������
        else
        {
            //�����A�o�����ǂ�
            _isButtleStart = true;
            _entranceDoorCollider.enabled = true;
            _exitDoorCollider.enabled = true;

            //���̃X�N���v�g�̎擾
            StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

            for (int _summonNum = 0; _summonNum < _lestenemynumber; _summonNum++)
            {
                //�G����������X�N���v�g�̃��\�b�h���Ăяo��
                _summonEnemyScripts[_summonNum].SummonEnemyPreparation(_thisScript,_hitPointIncreased,_attackPowerIncreased);

            }
        }
       

    }

    public void ButtleEnd()
    {
        //�{�X�K�w��������
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
        //�{�X�K�w�ȊO��������
        else
        {
            //�c��̓G�̐�
            _lestenemynumber--;

            if (_lestenemynumber == 0 && _summonEnemyScripts2.Count > 0 && !_isFase2)
            {
                _lestenemynumber = _summonEnemyScripts2.Count;
                //���̃X�N���v�g�̎擾
                StageCoreScript _thisScript = this.GetComponent<StageCoreScript>();

                for (int _summonNum = 0; _summonNum < _lestenemynumber; _summonNum++)
                {
                    //�G����������X�N���v�g�̃��\�b�h���Ăяo��
                    _summonEnemyScripts2[_summonNum].SummonEnemyPreparation(_thisScript, _hitPointIncreased, _attackPowerIncreased);
                }
                _isFase2 = true;
            }

            //�[���ɂȂ���������A�o���̊J��
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
