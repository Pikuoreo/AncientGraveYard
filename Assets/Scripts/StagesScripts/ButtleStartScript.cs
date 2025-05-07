using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ButtleStartScript : MonoBehaviour
{
    StageCoreScript _stageCore = default;//ステージの一番の親
    BoxCollider2D[] _boxColliders = default;//バトル開始トリガー

    private bool _isOnes = false;//一度だけ処理させるため

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
            //戦闘開始
            _isOnes = true;
            _stageCore.ButtleStart();

            for (int colliderCount = 0; colliderCount > _boxColliders.Length; colliderCount++)
            {
                //ステージにある戦闘開始トリガーをすべて消す
                _boxColliders[colliderCount].enabled = false;
            }

            _seAudio.PlayOneShot(_buttleStartSe);
        }
    }
}
