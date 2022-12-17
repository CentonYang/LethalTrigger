using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public ActionSystem pc1, pc2;
    public PlayerUI p1, p2;
    public Sprite[] stab1, stab2;

    [System.Serializable]
    public struct PlayerUI
    {
        public RectTransform hp, hpLose, sta, btr, skill;
        public Text combo;
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
        p1.sta.anchoredPosition = new Vector2(Mathf.Lerp(p1.staV.x, p1.staV.y, pc1.sta.x / pc1.sta.y), p1.sta.anchoredPosition.y);
        p2.sta.anchoredPosition = new Vector2(Mathf.Lerp(p2.staV.x, p2.staV.y, pc2.sta.x / pc2.sta.y), p2.sta.anchoredPosition.y);
        p1.sta.GetComponent<Image>().sprite = pc1.staCD ? stab1[1] : stab1[0];
        p2.sta.GetComponent<Image>().sprite = pc2.staCD ? stab2[1] : stab2[0];
        if (p1.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hide")) p1.combo.text = "00";
        if (pc1.combo > 1 && pc1.combo < 100 && p1.combo.text != pc1.combo.ToString())
        { p1.combo.GetComponent<Animator>().Play("FadeIn", 0, 0); p1.combo.text = pc1.combo.ToString(); }
        if (pc1.combo < 1 && p1.combo.text != pc1.combo.ToString() && p1.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FadeIn")) p1.combo.GetComponent<Animator>().Play("FadeOut");
        if (p2.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hide")) p2.combo.text = "00";
        if (pc2.combo > 1 && pc2.combo < 100 && p2.combo.text != pc2.combo.ToString())
        { p2.combo.GetComponent<Animator>().Play("FadeIn", 0, 0); p2.combo.text = pc2.combo.ToString(); }
        if (pc2.combo < 1 && p2.combo.text != pc2.combo.ToString() && p2.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FadeIn")) p2.combo.GetComponent<Animator>().Play("FadeOut");
    }
}
