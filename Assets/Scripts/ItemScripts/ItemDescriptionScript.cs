using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionScript : MonoBehaviour
{
    private bool _isCommonItem;
    private bool _isRareItem;
    private bool _isLegendItem;
    private bool _isSpecialItem;

    private Text _itemDescriptionText = default;

    private void Start()
    {
        _itemDescriptionText=this.GetComponent<Text>();
    }
    public void DescriptionChange(int itemID)
    {
        string Description = default;

        if (_isCommonItem)
        {
            #region　コモンアイテム説明文
            switch (itemID)
            {
                case 0:

                    Description = "プレイヤーの基礎攻撃力を25上げる";
                    _itemDescriptionText.text = Description;

                    break;

                case 1:

                    Description = "プレイヤーの最大体力を25上げる";
                    _itemDescriptionText.text = Description;

                    break;
                case 2:

                    Description = "近接のチャージ攻撃の威力を+0.1倍上げる";
                    _itemDescriptionText.text = Description;

                    break;
                case 3:

                    Description = "クリティカル率を2％上げる";
                    _itemDescriptionText.text = Description;

                    break;
                case 4:

                    Description = "クリティカル倍率を10％上げる";
                    _itemDescriptionText.text = Description;

                    break;

                case 5:
                    Description = "防御力を5上げる";
                    _itemDescriptionText.text = Description;
                    break;

                case 6:
                    Description = "Mpを10上げる";
                    _itemDescriptionText.text = Description;
                    break;
            }
            #endregion
        }
        else if (_isRareItem)
        {
            #region　レアアイテム説明文

            #endregion
        }
        else if (_isLegendItem)
        {
            #region　レジェンドアイテム説明文

            #endregion
        }
        else if (_isSpecialItem)
        {
            #region　スペシャルアイテム説明文

            #endregion
        }

        _isCommonItem = false;
        _isRareItem = false;
        _isLegendItem = false;
        _isSpecialItem = false;
    }

    public void RarityJudge(int rarity)
    {
        //ボタンが押されたらアイテムのレア度判断
        switch (rarity)
        {
            case 1:
                _isCommonItem = true;
                break;
            case 2:
                _isRareItem = true;
                break;
            case 3:
                _isLegendItem = true;
                break;
            case 4:
                _isSpecialItem = true;
                break;
        }
    }

    public void Textreset()
    {
        if (_itemDescriptionText != null)
        {
            _itemDescriptionText.text = default;
        }
        
    }
}
