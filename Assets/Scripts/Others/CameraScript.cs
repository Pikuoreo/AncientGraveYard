using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject _player = default;
    private GameObject _spotTarget = default;//�J���������Ώ�

    private float _spotTime = 0f;
    private float _elapsedTime = 0f;//�o�ߎ���

    private bool _isSpot = false;//true�ŃJ�������ΏۂɊ��

    [SerializeField] private GameObject _backGroundObject=default;//�w�i

    [SerializeField] private PlayerMoveControll _playerControllScript = default;

    [SerializeField] private BGMChangeScript _bgmChange = default;

    private int BossBGMValue = 0;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {

        //����̃I�u�W�F�N�g�ɃJ�������񂹂����Ƃ�
        if (_isSpot)
        {
            _elapsedTime += Time.deltaTime;

            //spottime���A�I�u�W�F�N�g�ɃJ�����𓖂Ă�
            if (_elapsedTime < _spotTime)
            {
                SpotCameraControll();
            }
            //���Ԓ��߂�����A���̏�Ԃɖ߂�
            else
            {
                EndSpotCameraControll();
            }
        }
        //�ʏ�̓v���C���[�ɃJ������ǔ�������
        else
        {
            defaultCameraControll();
        }
    }

    public void defaultCameraControll()
    {
        float adjustmentY = 1.75f;
        float adjustmentZ = -10;

        //�v���C���[�ɃJ������Ǐ]������
        this.transform.position = _player.transform.position + new Vector3(0, adjustmentY, adjustmentZ);
    }

    private void SpotCameraControll()
    {
        float spotSpeed = 0.05f;
        float adjustmentZ = 10;

        //�ΏۂɃ^�[�Q�b�g�����
        this.transform.position -= (this.transform.position - _spotTarget.transform.position)*spotSpeed+new Vector3(0,0,adjustmentZ);
    }

    private void EndSpotCameraControll()
    {
        //�ΏۂɊ��̂���߂�
        _isSpot = false;
        _spotTime = 0;
        _elapsedTime = 0;

        //�{�X�̍U�����J�n����
        _spotTarget.GetComponent<IBossStatus>().HealthBarSet();


        _spotTarget = default;

        //�v���C���[��������悤�ɂ���
        _playerControllScript.ControllOn();

        //���̃{�X��ŗ����a�f�l�����炩���߃Z�b�g���Ă���
        _bgmChange.ChangeBossBGM(BossBGMValue);
        BossBGMValue++;
    }

    public void SpotCamera(GameObject spotObject,float spotTime)
    {
        //�J������spotobject�Ɋ񂹂�
        _isSpot = true;
        //BGM���~�߂�
        _bgmChange.BGMStop();

        _spotTarget = spotObject;
        _spotTime = spotTime;
        //�v���C���[�𓮂����Ȃ�����
        _playerControllScript.ControllOff();
    }

    public void ReMatchBossBattle()
    {
        //�Q�[���I�[�o�[�ɂȂ�����A�{�X�ƍđI���Ă�����

        //BGM�̎Q�ƃi���o�[���P�߂�
        BossBGMValue--;
    }

    public void Backgroundchange()
    {
        //�w�i��ς���
        _backGroundObject.GetComponent<SpriteRenderer>().color = Color.red;
    }
}
