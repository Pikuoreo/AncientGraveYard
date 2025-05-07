using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearStageScript : MonoBehaviour
{
    private GameObject _overRaycanvas = default;//シーン依存のキャンバス

    [SerializeField] private GameObject _whiteOutPanel = default;

    private void Start()
    {
        string canvasTag = "OverRayCanvas";
        _overRaycanvas = GameObject.FindGameObjectWithTag(canvasTag);
        _overRaycanvas.SetActive(false);
    }
    public void GameClear()
    {
        _whiteOutPanel.GetComponent<Animator>().SetBool("isFadeIn", true);
    }
}
