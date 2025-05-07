using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialItemScript : MonoBehaviour
{
    [Header("戦士の証"), SerializeField] GameObject _changeToWarrior = default;
    private const int WARRIOR_PROFESSION_NUMBER = 1;

    [Header("魔術師の証"), SerializeField] GameObject _changeToWizard = default;
    private const int WIZARD_PROFESSION_NUMBER = 2;

    [SerializeField] private GameObject _StartFlor = default;

    private int _itemHP = 5;
    public void GiveToPlayer(PlayerAttack playerAttackScript)
    {

        if (_changeToWarrior != null)
        {
            playerAttackScript.ChangeForProffession(WARRIOR_PROFESSION_NUMBER);
            //始まりの場所にあるアイテムだったら
            if (_StartFlor != null)
            {
                //入口の動く床を動かす
                _StartFlor.GetComponent<StartFlorMove>().FlorMove(true);
            }
        }
        else if (_changeToWizard != null)
        {
            playerAttackScript.ChangeForProffession(WIZARD_PROFESSION_NUMBER);

            //始まりの場所にあるアイテムだったら
            if (_StartFlor != null)
            {
                //入口の動く床を動かす
                _StartFlor.GetComponent<StartFlorMove>().FlorMove(true);
            }

        }
    }

    public void BreakItem()
    {
        _itemHP--;
        print(_itemHP);
        if (_itemHP == 0)
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
            this.GetComponent<CircleCollider2D>().enabled = false;
        }
    }
}
