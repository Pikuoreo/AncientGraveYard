using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summonBossScript : MonoBehaviour
{
    [Header("召喚するボス"),SerializeField] private GameObject _bossObject = default;

    [SerializeField] List <SpriteRenderer> _mimicSprite=new List<SpriteRenderer>();

    private bool _isSummon = false;//召喚したか

    private CameraScript _cameraScript = default;

    [SerializeField] private ParticleSystem _summonEffect = default;//召喚時のパーティクル

    private void Start()
    {
        _cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        _summonEffect.Stop();
    }
    private void Update()
    {
        if (_isSummon)
        {
            BossColorMaterialization();
        }
    }

    private void BossColorMaterialization()
    {
        foreach(SpriteRenderer sprits in _mimicSprite)
        {
            if (sprits.color.a <= 1)
            {
                float colorChangeValue = 0.25f;

                sprits.color += new Color(0, 0, 0, colorChangeValue) * Time.deltaTime;
            }
            else
            {
                _isSummon = false;
                _summonEffect.Stop();
            }
        }
       
    }

    public void SummonBossPreparation()
    {
        _summonEffect.Play();
        _isSummon = true;
        _cameraScript.SpotCamera(_bossObject, 5f);
    }
}
