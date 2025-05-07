using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetItemScript : MonoBehaviour
{
    private PlayerMoveControll _playerGetItem = default;
    private GameObject _currentItem = default;
    [SerializeField] private  GameObject _xButton = default;
    [SerializeField] private GameObject _wButton = default;

    [SerializeField] private GameObject _nextStage = default;

    private const int COMMON_ITEM_LAYER = 6; //�R�����A�C�e���̃��C���[
    private const int RARE_ITEM_LAYER = 7;�@//���A�A�C�e���̃��C���[
    private const int LEGEND_ITEM_LAYER = 8;�@//���W�F���h�A�C�e���̃��C���[
    private const int SPECIAL_ITEM_LAYER = 9;�@//����A�C�e���̃��C���[

    private const int WORP_POSITION_LAYER = 15;

    //[SerializeField] private TresureScript __tresureScript = default;

    [SerializeField] GameProgressScript _gameManagerScript = default;

    private AudioSource _seAudio = default;

    [Header("���[�v����SE"), SerializeField] private AudioClip _worpSE = default;
    [Header("�A�C�e���擾����SE"), SerializeField] private AudioClip _getItemSE = default;
    private void Start()
    {
        _seAudio=this.GetComponent<AudioSource>();
        _playerGetItem = this.GetComponentInParent<PlayerMoveControll>();
    }
    private void Update()
    {

        if (_currentItem != null && Input.GetKeyDown(KeyCode.X))
        {
            _seAudio.PlayOneShot(_getItemSE);
            _playerGetItem.GetItem(_currentItem);
        }

        if(_nextStage!=null && Input.GetKeyDown(KeyCode.W))
        {
            _seAudio.PlayOneShot(_worpSE);
            _gameManagerScript.Worp();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _seAudio.PlayOneShot(_worpSE);
            _gameManagerScript.Worp();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        float buttonImagePositionY = 1.5f;

        //�g���K�[����obj�̃��C���[���U�C�V�C�W�C�X�̂����ꂩ��������
        if (collision.gameObject.layer == COMMON_ITEM_LAYER||
            collision.gameObject.layer == RARE_ITEM_LAYER || 
            collision.gameObject.layer == LEGEND_ITEM_LAYER || 
            collision.gameObject.layer == SPECIAL_ITEM_LAYER )
        {
            _currentItem = collision.gameObject;
            _xButton.SetActive(true);
            _xButton.transform.position = collision.gameObject.transform.position + new Vector3(0, buttonImagePositionY, 0);
        }

        if (collision.gameObject.layer == WORP_POSITION_LAYER)
        {
            _nextStage = collision.gameObject;
            _wButton.SetActive(true);
            _wButton.transform.position = collision.gameObject.transform.position + new Vector3(0, buttonImagePositionY, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�g���K�[���痣�ꂽobj�̃��C���[���U�C�V�C�W�C�X�̂����ꂩ��������
        if (collision.gameObject.layer == COMMON_ITEM_LAYER ||
            collision.gameObject.layer == RARE_ITEM_LAYER ||
            collision.gameObject.layer == LEGEND_ITEM_LAYER ||
            collision.gameObject.layer == SPECIAL_ITEM_LAYER)
        {

            _xButton.SetActive(false);
            _currentItem = null;
        }

        if(collision.gameObject.layer == WORP_POSITION_LAYER)
        {
            _wButton.SetActive(false);
            _nextStage = null;
        }
    }
}
