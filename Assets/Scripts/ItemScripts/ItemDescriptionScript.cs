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
            #region�@�R�����A�C�e��������
            switch (itemID)
            {
                case 0:

                    Description = "�v���C���[�̊�b�U���͂�25�グ��";
                    _itemDescriptionText.text = Description;

                    break;

                case 1:

                    Description = "�v���C���[�̍ő�̗͂�25�グ��";
                    _itemDescriptionText.text = Description;

                    break;
                case 2:

                    Description = "�ߐڂ̃`���[�W�U���̈З͂�+0.1�{�グ��";
                    _itemDescriptionText.text = Description;

                    break;
                case 3:

                    Description = "�N���e�B�J������2���グ��";
                    _itemDescriptionText.text = Description;

                    break;
                case 4:

                    Description = "�N���e�B�J���{����10���グ��";
                    _itemDescriptionText.text = Description;

                    break;

                case 5:
                    Description = "�h��͂�5�グ��";
                    _itemDescriptionText.text = Description;
                    break;

                case 6:
                    Description = "Mp��10�グ��";
                    _itemDescriptionText.text = Description;
                    break;
            }
            #endregion
        }
        else if (_isRareItem)
        {
            #region�@���A�A�C�e��������

            #endregion
        }
        else if (_isLegendItem)
        {
            #region�@���W�F���h�A�C�e��������

            #endregion
        }
        else if (_isSpecialItem)
        {
            #region�@�X�y�V�����A�C�e��������

            #endregion
        }

        _isCommonItem = false;
        _isRareItem = false;
        _isLegendItem = false;
        _isSpecialItem = false;
    }

    public void RarityJudge(int rarity)
    {
        //�{�^���������ꂽ��A�C�e���̃��A�x���f
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
