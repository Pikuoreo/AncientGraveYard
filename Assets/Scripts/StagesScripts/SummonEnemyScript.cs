using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonEnemyScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> _enemys = new List<GameObject>();//召喚する敵
    [SerializeField] private ParticleSystem _summonEffect = default;//召喚エフェクト

    private StageCoreScript _coreScript = default;
    // Start is called before the first frame update
    void Start()
    {
        _summonEffect.Stop();
    }
    public void SummonEnemyPreparation(StageCoreScript _stageCore ,float hitPointMultipleValue ,float AttackPowerMultipleValue)
    {
        _coreScript = _stageCore;

        //数秒後に敵召喚
        StartCoroutine(SummonEnemy(hitPointMultipleValue,AttackPowerMultipleValue));
    }

    public void BeDefeated()
    {
        if (_coreScript != null)
        {
            _coreScript.ButtleEnd();
        }
    
    }

    private IEnumerator SummonEnemy(float hitPointIncreasedValue,float attackPowerIncreasedValue)
    {
        string nomalEnemyTag = "NormalEnemy";
        string turretEnemyTag = "TurretEnemy";
        string chaserEnemytag = "ChaserEnemy";
        string flyEnemyTag = "FlyEnemy";


        _summonEffect.Play();

        float summonEnemyTime = 2f;

        int chooseEnemys = 0;
        chooseEnemys = Random.Range(0, _enemys.Count);
        
        yield return new WaitForSeconds(summonEnemyTime);
        _summonEffect.Stop();
        GameObject enemy=Instantiate(_enemys[chooseEnemys], this.transform.position, Quaternion.identity);
        enemy.transform.parent = this.transform;

        //単純型の敵だったら
        if (enemy.gameObject.CompareTag(nomalEnemyTag))
        {
            enemy.GetComponent<NormalEnemyScript>().StatusIncrease(hitPointIncreasedValue, attackPowerIncreasedValue);
        }
        //タレット型の敵だったら
        else if (enemy.gameObject.CompareTag(turretEnemyTag))
        {
            enemy.GetComponent<TurretEnemyScript>().StatusIncrease(hitPointIncreasedValue, attackPowerIncreasedValue);
        }
        //追尾型の敵だったら
        else if (enemy.gameObject.CompareTag(chaserEnemytag))
        {
            enemy.GetComponent<ChaserEnemyScript>().StatusIncrease(hitPointIncreasedValue, attackPowerIncreasedValue);
        }
        //飛翔型の敵だったら
        else if (enemy.gameObject.CompareTag(flyEnemyTag))
        {
            enemy.GetComponent<FlyEnemyScript>().StatusIncrease(hitPointIncreasedValue, attackPowerIncreasedValue);
        }
        
    }

    
}
