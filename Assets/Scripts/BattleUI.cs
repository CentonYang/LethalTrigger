using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BattleUI : MonoBehaviour
{
    public bool practiceMode;
    public Menu battleMenu;
    public Text timerTxt;
    public AnimationCurve hpCurve;
    public ActionSystem pc1, pc2;
    public PlayerUI p1, p2;
    public Sprite[] stab1, stab2;
    [System.Serializable]
    public struct PlayerUI
    {
        public RectTransform hp, hpLose, sta, btr, skill, life;
        public Text combo;
        public Vector2 hpV, staV, btrV, skillV;
        public List<Image> iconList;
    }
    [System.Serializable]
    public struct ResultPanel
    {
        public RectTransform result, p1, p2;
        public List<RectTransform> useChar;
    }
    public ResultPanel resultPanel;
    public UnityEvent restartEvent, backEvent;
    public Image goodGame, extraTime;
    float timer;
    bool result = false, exit = false;

    void Start()
    {
        if (pc1 == null && GameObject.Find("Player1").GetComponentInChildren<ActionSystem>() != null) pc1 = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>();
        if (pc2 == null && GameObject.Find("Player2").GetComponentInChildren<ActionSystem>() != null) pc2 = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>();
        timer = GameSystem.timer;
    }

    void FixedUpdate()
    {
        if (pc1 != null && pc2 != null)
        {
            p1.hp.anchoredPosition = new Vector2(Mathf.Lerp(p1.hpV.x, p1.hpV.y, pc1.hp.x / pc1.hp.y * hpCurve.Evaluate(pc1.hp.x / pc1.hp.y)), p1.hp.anchoredPosition.y);
            if (pc2.combo == 0)
                p1.hpLose.anchoredPosition = new Vector2(Mathf.Lerp(p1.hpLose.anchoredPosition.x, p1.hp.anchoredPosition.x, .05f), p1.hpLose.anchoredPosition.y);
            p2.hp.anchoredPosition = new Vector2(Mathf.Lerp(p2.hpV.x, p2.hpV.y, pc2.hp.x / pc2.hp.y * hpCurve.Evaluate(pc2.hp.x / pc2.hp.y)), p2.hp.anchoredPosition.y);
            if (pc1.combo == 0)
                p2.hpLose.anchoredPosition = new Vector2(Mathf.Lerp(p2.hpLose.anchoredPosition.x, p2.hp.anchoredPosition.x, .05f), p2.hpLose.anchoredPosition.y);
            p1.sta.anchoredPosition = new Vector2(Mathf.Lerp(p1.staV.x, p1.staV.y, pc1.sta.x / pc1.sta.y * hpCurve.Evaluate(pc1.sta.x / pc1.sta.y)), p1.sta.anchoredPosition.y);
            p2.sta.anchoredPosition = new Vector2(Mathf.Lerp(p2.staV.x, p2.staV.y, pc2.sta.x / pc2.sta.y * hpCurve.Evaluate(pc2.sta.x / pc2.sta.y)), p2.sta.anchoredPosition.y);
            p1.sta.GetComponent<Image>().sprite = pc1.staCD ? stab1[1] : stab1[0];
            p2.sta.GetComponent<Image>().sprite = pc2.staCD ? stab2[1] : stab2[0];
            p1.btr.anchoredPosition = new Vector2(Mathf.Lerp(p1.btrV.x, p1.btrV.y, pc1.btr.x / pc1.btr.y), p1.btr.anchoredPosition.y);
            p2.btr.anchoredPosition = new Vector2(Mathf.Lerp(p2.btrV.x, p2.btrV.y, pc2.btr.x / pc2.btr.y), p2.btr.anchoredPosition.y);
            p1.skill.anchoredPosition = new Vector2(Mathf.Lerp(p1.skillV.x, p1.skillV.y, pc1.skill.x / pc1.skill.y), p1.skill.anchoredPosition.y);
            p2.skill.anchoredPosition = new Vector2(Mathf.Lerp(p2.skillV.x, p2.skillV.y, pc2.skill.x / pc2.skill.y), p2.skill.anchoredPosition.y);
            if (p1.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hide")) p1.combo.text = "00";
            if (pc1.combo > 1 && pc1.combo < 100 && p1.combo.text != pc1.combo.ToString())
            { p1.combo.GetComponent<Animator>().Play("FadeIn", 0, 0); p1.combo.text = pc1.combo.ToString(); }
            if (pc1.combo < 1 && p1.combo.text != pc1.combo.ToString() && p1.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FadeIn")) p1.combo.GetComponent<Animator>().Play("FadeOut");
            if (p2.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Hide")) p2.combo.text = "00";
            if (pc2.combo > 1 && pc2.combo < 100 && p2.combo.text != pc2.combo.ToString())
            { p2.combo.GetComponent<Animator>().Play("FadeIn", 0, 0); p2.combo.text = pc2.combo.ToString(); }
            if (pc2.combo < 1 && p2.combo.text != pc2.combo.ToString() && p2.combo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("FadeIn")) p2.combo.GetComponent<Animator>().Play("FadeOut");
            if (practiceMode) { timerTxt.text = "∞"; pc1.life = 2; pc2.life = 2; }
            else
            {
                for (int i = 0; i < p1.life.childCount; i++)
                {
                    p1.life.GetChild(i).gameObject.SetActive(i < GameSystem.life ? true : false);
                    p1.life.GetChild(i).GetComponent<Animator>().enabled = i < pc1.life ? false : true;
                }
                for (int i = 0; i < p2.life.childCount; i++)
                {
                    p2.life.GetChild(i).gameObject.SetActive(i < GameSystem.life ? true : false);
                    p2.life.GetChild(i).GetComponent<Animator>().enabled = i < pc2.life ? false : true;
                }
                if (pc1.pc.isCtrl && pc2.pc.isCtrl && timer > 0)
                    timer -= Time.fixedDeltaTime;
                timerTxt.text = (timer + .1f).ToString("0");
                if (!result && (timer <= 0 || pc1.life <= 0 || pc2.life <= 0)) { result = true; GoodGame(); }
            }
        }
    }

    public void GoodGame()
    {
        pc1.pc.isCtrl = false; pc2.pc.isCtrl = false;
        battleMenu.gameObject.SetActive(false);
        int win = 0;
        if (pc1.life > pc2.life) win = 1;
        else if (pc1.life < pc2.life) win = 2;
        else if (pc1.hp.x > pc2.hp.x) win = 1;
        else if (pc1.hp.x < pc2.hp.x) win = 2;
        else { StartCoroutine(ExtraTime()); return; }
        StartCoroutine(Result(win));
    }

    IEnumerator ExtraTime()
    {
        yield return new WaitForSecondsRealtime(1f);
        pc1.life = 1; pc2.life = 1; timer = 30;
        extraTime.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        extraTime.gameObject.SetActive(false);
        if (pc1.death) StartCoroutine(pc1.Recovery());
        else pc1.pc.isCtrl = true;
        if (pc2.death) StartCoroutine(pc2.Recovery());
        else pc2.pc.isCtrl = true;
        result = false;
    }

    IEnumerator Result(int winner)
    {
        yield return new WaitForSecondsRealtime(1f);
        goodGame.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        goodGame.gameObject.SetActive(false);
        resultPanel.result.gameObject.SetActive(true);
        foreach (AudioVolume item in FindObjectsOfType<AudioVolume>())
            if (item.audioType == AudioVolume.AudioType.bgm) item.GetComponent<AudioSource>().volume = (float)GameSystem.playerData.bgmVol * item.scale * .5f;
        Cinemachine.CinemachineTargetGroup cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        Cinemachine.CinemachineFramingTransposer cineTrans = resultPanel.result.GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
        for (int i = 0; i < cineTarget.m_Targets.Length; i++)
            if (cineTarget.m_Targets[i].target.GetComponent<ActionSystem>().pc.pc == winner - 1)
                cineTarget.m_Targets[1 - i].weight = 0;
        cineTrans.m_ScreenX = .35f;
        if (winner == 1) resultPanel.p1.gameObject.SetActive(true);
        else resultPanel.p2.gameObject.SetActive(true);
        resultPanel.useChar[winner == 1 ? GameSystem.p1Char : GameSystem.p2Char].gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        exit = true;
    }

    public void InputAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed && ctx.action.name + ctx.ReadValue<float>() == "W_cls1" && exit)
            restartEvent.Invoke();
        if (ctx.phase != InputActionPhase.Performed && ctx.action.name + ctx.ReadValue<float>() == "R_cls1" && exit)
            backEvent.Invoke();
    }

    public void ChangeScene(string sceneID)
    {
        StartCoroutine(Menu.PreloadScene(sceneID));
    }
}
