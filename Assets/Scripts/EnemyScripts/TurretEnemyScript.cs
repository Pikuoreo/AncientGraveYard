using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TurretEnemyScript : MonoBehaviour
{
    [Header("”­Ë‚·‚é’e"),SerializeField] private GameObject _burret = default;
    [Header("Mp‰ñ•œƒAƒCƒeƒ€"), SerializeField] private GameObject _magicPointStar = default;
    [SerializeField] private GameObject _player = default;
  
    private GameObject _burretClone = default;//”­Ë‚µ‚½’e

    private float _fireTimeValue = 0; //‰½•b‚½‚Á‚½‚©
    private float _fireTime = 5;//”­Ë‚Ü‚Å‚ÌŠÔ
    private float _burretRange = 15f;//Ë’ö‹——£

    [Header("“G‚ÌHP"),SerializeField] private float _hitPoint = 210;
    [Header("“G‚ÌUŒ‚—Í‚ÌÅ’á’l"), SerializeField] private float _attackMinPower = 0f;
    [Header("“G‚ÌUŒ‚—Í‚ÌÅ‚’l"), SerializeField] private float _attackMaxPower = 0f;
    [Header("‘Ï‚¦‚Ä‚Ù‚µ‚¢UŒ‚‰ñ”"), SerializeField] private float NumberOfAttacks = default;

    [SerializeField] private AudioSource _turretAudio = default;
    [SerializeField] private AudioClip _burretSE = default;

    private PlayerStatusChange _playerStatusScript = default;

    private void Start()
    {
        _turretAudio=this.GetComponent<AudioSource>();
    }
    void Update()
    {
        float playerdistanceX = Mathf.Abs(_player.transform.position.x - this.transform.position.x);
        float playerdistanceY = Mathf.Abs(_player.transform.position.y - this.transform.position.y);

        if (_fireTimeValue >= _fireTime&&playerdistanceX<_burretRange&& playerdistanceY < _burretRange)
        {
            Fire();
        }
        else if(_fireTimeValue < _fireTime)
        {
            _fireTimeValue += Time.deltaTime;

        }

        if (_hitPoint <= 0)
        {
            Death();
        }
    }

    private void Fire()
    {
        _turretAudio.PlayOneShot(_burretSE);
        _fireTimeValue = 0;
        _burretClone = Instantiate(_burret, this.transform.position, Quaternion.FromToRotation(Vector3.up, this.transform.position - _player.transform.position));
        _burretClone.GetComponent<TurrentBurretScript>().GetAttackPower(_attackMinPower, _attackMaxPower);
    }

    public void BeAttacked(float Damage)
    {
        _hitPoint -= Damage;
    }

    public void StatusIncrease(float hitPointincreasedValue, float attackPowerIncreasedValue)
    {
        //[“x‚É‚ ‚í‚¹‚ÄƒXƒe[ƒ^ƒX‚ğ‹­‰»
        _hitPoint *= hitPointincreasedValue;
        _attackMinPower *= attackPowerIncreasedValue;
        _attackMaxPower *= attackPowerIncreasedValue;

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerStatusScript = _player.GetComponent<PlayerStatusChange>();
        StatusAdjustment();
    }

    private void StatusAdjustment()
    {
        //ƒvƒŒƒCƒ„[‚ÌÀ¿HP 
        float _playerHP = _playerStatusScript.EnemyAttackPowerAdjustment();

        if (_playerHP < _attackMaxPower * NumberOfAttacks)
        {
            //Å‘åUŒ‚—Í‚ÍƒvƒŒƒCƒ„[‚Ì‘Ì—Í‚Ì‰½“•ª‚©
            float firstCalculation = _attackMaxPower / _playerHP;

            //Å‘åUŒ‚—Í‚ÉfirstCalculation‚Åo‚µ‚½’l‚ğŠ|‚¯‚é
            float secondCalculation = _attackMaxPower * firstCalculation;

            //secondCalculation‚Åo‚µ‚½’l‚ğ‘Ï‚¦‚Ä‚Ù‚µ‚¢UŒ‚‰ñ”‚ÅŠ„‚é
            float thirdCalculation = Mathf.Floor(secondCalculation / NumberOfAttacks);

            //©g‚ÌUŒ‚—Í‚ğthirdCalculation‚Åo‚µ‚½•ªˆø‚­
            _attackMinPower -= thirdCalculation;
            _attackMaxPower -= thirdCalculation;

        }


        //ƒvƒŒƒCƒ„[‚ÌUŒ‚—Í‚ğæ“¾
        float playerAttackvalue = _playerStatusScript.EnemyHitPointAdjustment().Item1;

        //ƒvƒŒƒCƒ„[‚Ì¡‚ÌE‹Æ‚ğæ“¾
        int playerProfession = _playerStatusScript.EnemyHitPointAdjustment().Item2;

        switch (playerProfession)
        {
            //Œ•m‚Ì
            case 1:

                //ƒvƒŒƒCƒ„[‚ª‚U‰ñUŒ‚‚µ‚Ä‚à“|‚¹‚È‚¢HP‚È‚ç
                if (playerAttackvalue * 6 < _hitPoint)
                {

                    float lestHitPoint = default;

                    //‚U‰ñUŒ‚‚µ‚½•ª‚Ìc‚èHP‚ğŒvZ
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //ŒvZŒ‹‰Ê‚Ì’l€‚Qˆø‚­
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;

            //–‚–@g‚¢‚Ì
            case 2:

                //ƒvƒŒƒCƒ„[‚ª8‰ñUŒ‚‚µ‚Ä‚à“|‚¹‚È‚¢HP‚È‚ç
                if (playerAttackvalue * 8 < _hitPoint)
                {
                    float lestHitPoint = default;

                    //‚U‰ñUŒ‚‚µ‚½•ª‚Ìc‚èHP‚ğŒvZ
                    lestHitPoint = _hitPoint - playerAttackvalue * 6;

                    //ŒvZŒ‹‰Ê‚Ì’l€‚Qˆø‚­
                    _hitPoint -= Mathf.Floor(lestHitPoint / 2);
                }

                break;
        }

    }
    public void Death()
    {
        float minRandomValue = 0;
        float maxRandomValue = 101;
        float halfValue = 3;

        float randomValue = Random.Range(minRandomValue, maxRandomValue);

        //3•ª‚Ìˆê‚ÅMp‰ñ•œƒAƒCƒeƒ€‚ğo‚·
        if (randomValue <= maxRandomValue / halfValue)
        {
            Instantiate(_magicPointStar, this.transform.transform.position, Quaternion.identity);
        }

        //€‚ñ‚¾‚±‚Æ‚ğƒXƒe[ƒWƒRƒA‚É“`‚¦‚é
        this.GetComponentInParent<SummonEnemyScript>().BeDefeated();
        Destroy(this.gameObject);
    }
}
