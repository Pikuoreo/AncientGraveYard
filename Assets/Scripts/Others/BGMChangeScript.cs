using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMChangeScript : MonoBehaviour
{
    private bool _isFadeOut = false;

    private AudioSource _backGroundBGM = default;
    [Header("流れるBGM集"), SerializeField] private List<AudioClip> _changeStageBGMs = new List<AudioClip>();
    [Header("流れるボスBGM集"), SerializeField] private List<AudioClip> _changeBossBGMs = new List<AudioClip>();
    [Header("流れるゲームオーバーのBGM"), SerializeField] private AudioClip _changeGameOverBGM = default;
    // Start is called before the first frame update
    void Start()
    {
        _backGroundBGM=this.GetComponent<AudioSource>();
        changeStageBGM(0);
    }
    private void Update()
    {
        if (_isFadeOut)
        {
            DownVolume();
        }
    }

    private void DownVolume()
    {
        _backGroundBGM.volume -= Time.deltaTime / 2.5f;
    }

    public void changeStageBGM(int changeBGMValue)
    {
        _backGroundBGM.Stop();
        _backGroundBGM.clip = _changeStageBGMs[changeBGMValue];
        _backGroundBGM.Play();
    }

    public void ChangeBossBGM( int changeBGMValue)
    {
        _backGroundBGM.Stop();
        _backGroundBGM.clip = _changeBossBGMs[changeBGMValue];
        _backGroundBGM.Play();
    }

    public void ChangeGameOverBGM()
    {
        _backGroundBGM.Stop();
        _backGroundBGM.clip = _changeGameOverBGM;
        _backGroundBGM.Play();
    }

    public void BGMFadeout()
    {
        _isFadeOut = true;
    }

    public void BGMOn()
    {
        _isFadeOut = false;
        _backGroundBGM.Play();
        float bgmVolume = 0.75f;
        _backGroundBGM.volume = bgmVolume;
    }
    public void BGMStop()
    {
        _backGroundBGM.Stop();
    }
}
