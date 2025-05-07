using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEvolutionButton : MonoBehaviour
{
    [SerializeField] WeaponEvolution _parentScript = default;

    public void SordEvolution()
    {
        string findTag = "SordWeapon";
        //剣のレベルを上げる
        GameObject.FindGameObjectWithTag(findTag).GetComponent<SordAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }
    public void ShieldEvolution()
    {
        string findTag = "ShieldWeapon";
        //盾のレベルを上げる
        GameObject.FindGameObjectWithTag(findTag).GetComponent<ShieldWeapon>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }

    public void MagicStickEvolution()
    {
        string findTag = "MagicStickWeapon";

        //直進魔法のレベルを上げる
        GameObject.FindGameObjectWithTag(findTag).GetComponent<MagicStickAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }

    public void ChaserCaneEvolution()
    {
        string findTag = "ChaserCaneWeapon";

        //追跡魔法のレベルを上げる
        GameObject.FindGameObjectWithTag(findTag).GetComponent<ChaserCaneAttack>().WeaponLevelUp();
        _parentScript.EndEvolutionChoice();
    }
}
