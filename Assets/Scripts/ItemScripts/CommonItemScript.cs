using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommonItemScript : MonoBehaviour
{
    [Header("アイテム番号"), SerializeField] private int _itemID = 0;

    private float _powerUp = 25f;//近接攻撃力アップ
    private float _hitPointUp = 25f;//魔法攻撃力アップ
    private float _chargeMultipleUp = 0.1f;//チャージ攻撃の倍率アップ
    private float _criticalChanceUp = 2f;//クリティカル率アップ
    private float _criticalMultipleUp = 10;//クリティカル倍率アップ
    private float _defencePointUp = 5f;//防御力アップ
    private float _magicPointUp = 10f;

    private float _itemEffectMultiple = 1f; //アイテム効果の倍率

    private commonItemCountScript _countScript = default;

    private void Start()
    {
        string findTag = "StatusUI";
        _countScript = GameObject.FindGameObjectWithTag(findTag).GetComponent<commonItemCountScript>();
    }
    public void GiveToPlayer(PlayerStatusChange playerController)
    {
        switch (_itemID)
        {
            case 0://攻撃力upアイテム

                playerController.AttackPowerUP(_powerUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 1://体力upアイテム

                playerController.hitPointUP(_hitPointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 2://チャージ力upアイテム        

                playerController.ChargeMultipleUP(_chargeMultipleUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 3://会心率upアイテム

                playerController.CriticalChanceUP(_criticalChanceUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 4://会心威力upアイテム

                playerController.CriticalMultipleUP(_criticalMultipleUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 5://防御力upアイテム

                playerController.DefencePowerUP(_defencePointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;

            case 6://魔力upアイテム

                playerController.MagicPointUp(_magicPointUp * _itemEffectMultiple);
                _countScript.ItemCount(_itemID);
                break;
        }
    }
}
