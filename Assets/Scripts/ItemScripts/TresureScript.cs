using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresureScript : MonoBehaviour
{
    [SerializeField] private List<GameObject> _commonItems = new List<GameObject>();
    [SerializeField] private List<GameObject> _rareItems = new List<GameObject>();
    [SerializeField] private List<GameObject> _legendItems = new List<GameObject>();

    private GameObject _item = default;
    public void RandomItem(GameObject stageParent)
    {
        int itemRarity = Random.Range(1, 6000);

        //60&でコモンアイテム
        if (itemRarity < 6001)
        {
            int randomCommonItem = Random.Range(0, _commonItems.Count);
            _item = Instantiate(_commonItems[randomCommonItem], this.transform.position, Quaternion.identity);
            _item.transform.parent = stageParent.transform;
        }

        //30%でレアアイテム
        else if (itemRarity < 9001)
        {
            int randomRareItem = Random.Range(0, _rareItems.Count);
            switch (randomRareItem)
            {
                case 0:

                    print("力の実");

                    break;


                case 1:

                    print("耐久の実");

                    break;
            }
        }

        //10%でレジェンドアイテム
        else
        {
            int randomLegendItem = Random.Range(0, _legendItems.Count);
            switch (randomLegendItem)
            {
                case 0:
                    
                    break;

                case 1:

                    break;
            }
        }
    }


}
