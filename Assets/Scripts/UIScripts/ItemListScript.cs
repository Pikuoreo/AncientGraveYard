using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListScript : MonoBehaviour
{
    [SerializeField] private GameObject _textList = default; 
    [SerializeField] private GameObject _itemList = default;
    [SerializeField] private GameObject _weaponListParent = default;
    [SerializeField] private WeaponDescription _weaponDescription = default;
    // Start is called before the first frame update


    public void ItemButtonOnClick()
    {
        //アイテムリストを見えるようにする
        _weaponListParent.SetActive(true);
        _textList.SetActive(false);
        _itemList.SetActive(true);
    }

    public void WeaponListButtonOnClick()
    {
        //武器リストを見えるようにする
        _weaponListParent.SetActive(true);
        _weaponDescription.ProfessionDescriptionDisplay();
        _textList.SetActive(false);
        _itemList.SetActive(false);
    }
}
