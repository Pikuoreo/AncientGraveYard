using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearScreenText : MonoBehaviour
{
    [SerializeField] private GameObject _nextScreenAnimation = default;//���Ɏ��s����A�j���[�V�����̃I�u�W�F�N�g

    [SerializeField] private GameObject _reTryButton = default;//������x����̃{�^��
    
    [SerializeField] private GameObject _exitButton = default;//�^�C�g���֖߂�{�^��

    [SerializeField] private AudioClip _clearSe = default;
    // Start is called before the first frame update
    public void NextAnimation()
    {
        if (_clearSe != null)
        {
            //�N���ASe�𗬂��I�u�W�F�N�g�Ȃ�A�N���ASe�𗬂�
            this.GetComponent<AudioSource>().PlayOneShot(_clearSe);
        }
        //���̃A�j���[�V�����𗬂�
        _nextScreenAnimation.GetComponent<Animator>().enabled = true;
    }

    public void SelectButtonDisplay()
    {
        //�I���{�^�����o��
        _reTryButton.SetActive(true);
        _exitButton.SetActive(true);
    }
}
