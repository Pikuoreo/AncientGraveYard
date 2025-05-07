using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtleStartScript : MonoBehaviour
{
    StageCoreScript _stageCore = default;//�X�e�[�W�̈�Ԃ̐e
    BoxCollider2D[] _boxColliders = default;//�o�g���J�n�g���K�[

    private bool _isOnes = false;//��x�������������邽��

    [SerializeField] private AudioClip _buttleStartSe = default;
    private AudioSource _seAudio = default;
    // Start is called before the first frame update
    void Start()
    {
        _stageCore = this.GetComponentInParent<StageCoreScript>();

        _boxColliders = this.GetComponents<BoxCollider2D>();
        _seAudio = this.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string playerTag = "Player";

        if (collision.gameObject.CompareTag(playerTag)&&!_isOnes)
        {
            //�퓬�J�n
            _isOnes = true;
            _stageCore.ButtleStart();

            for (int colliderCount = 0; colliderCount > _boxColliders.Length; colliderCount++)
            {
                //�X�e�[�W�ɂ���퓬�J�n�g���K�[�����ׂď���
                _boxColliders[colliderCount].enabled = false;
            }

            _seAudio.PlayOneShot(_buttleStartSe);
        }
    }
}
