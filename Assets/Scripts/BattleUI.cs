using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public ActionSystem pc1, pc2;
    public PlayerUI p1, p2;

    [System.Serializable]
    public struct PlayerUI
    {
        public RectTransform hp, hpLose, sta, btr, skill;
        public Vector2 hpV, staV, btrV, skillV;
    }

    void Start()
    {
        pc1 = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>();
        pc2 = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>();
    }

    void Update()
    {
        p1.hp.anchoredPosition = new Vector2(Mathf.Lerp(p1.hpV.x, p1.hpV.y, pc1.hp.x / pc1.hp.y), p1.hp.anchoredPosition.y);
        p1.hpLose.anchoredPosition = new Vector2(Mathf.Lerp(p1.hpLose.anchoredPosition.x, p1.hp.anchoredPosition.x, .05f), p1.hpLose.anchoredPosition.y);
        p2.hp.anchoredPosition = new Vector2(Mathf.Lerp(p2.hpV.x, p2.hpV.y, pc2.hp.x / pc2.hp.y), p2.hp.anchoredPosition.y);
        p2.hpLose.anchoredPosition = new Vector2(Mathf.Lerp(p2.hpLose.anchoredPosition.x, p2.hp.anchoredPosition.x, .05f), p2.hpLose.anchoredPosition.y);
        p2.sta.anchoredPosition = new Vector2(Mathf.Lerp(p2.staV.x, p2.staV.y, pc2.sta.x / pc2.sta.y), p2.sta.anchoredPosition.y);
        p2.sta.anchoredPosition = new Vector2(Mathf.Lerp(p2.staV.x, p2.staV.y, pc2.sta.x / pc2.sta.y), p2.sta.anchoredPosition.y);
    }
}
