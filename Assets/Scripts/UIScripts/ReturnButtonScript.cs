using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnButtonScript : MonoBehaviour
{

    [SerializeField] private GameObject _statusList = default;
    [SerializeField] private GameObject _itemList = default;
    [SerializeField] private GameObject _weaponlist = default;

    /// <summary>
    /// ボタンがクリックされたときに呼び出す
    /// </summary>
    public void OnClick()
    {
        _statusList.SetActive(true);
        _itemList.SetActive(false);
        _weaponlist.SetActive(false);
    }
}
