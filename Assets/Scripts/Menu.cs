using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public int transition, arrow, pc;
    public MenuList selection;
    public List<MenuList> menuLists;
    [System.Serializable]
    public class MenuContent
    {
        public List<Text> textDisplay;
        public List<Text> textSource;
        public UnityEvent selectEvent;
        public UnityEvent confirmEvent;
        public UnityEvent backEvent;
    }
    public List<MenuContent> menuContent;
    public Animator anim;
    Cinemachine.CinemachineTargetGroup cineTarget;
    Cinemachine.CinemachineFramingTransposer cineTrans;
    public string sceneChangeID = "";
    bool infoView = false;
    Text infoTarget;

    void Start()
    {
        SelectTransition(0);
    }

    void FixedUpdate()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Null") && transition == 0)
        {
            if (arrow > 0)
                if (infoView)
                    infoTarget.rectTransform.Translate(0, 4, 0);
                else anim.Play("Next", 0, 0);
            if (arrow < 0)
                if (infoView)
                    infoTarget.rectTransform.Translate(0, -4, 0);
                else anim.Play("Previous", 0, 0);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Null"))
        {
            if (sceneChangeID != "")
            {
                if (sceneChangeID == "Exit") Application.Quit();
                else StartCoroutine(PreloadScene(sceneChangeID));
                sceneChangeID = "";
            }
            else gameObject.SetActive(false);
        }
    }

    public void SelectMove(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().y < -.1f)
            arrow = 1;
        else if (ctx.ReadValue<Vector2>().y > .1f)
            arrow = -1;
        else
            arrow = 0;
    }

    public void SelectAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed && !anim.GetCurrentAnimatorStateInfo(0).IsName("Null") && transition == 0)
        {
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1" && !infoView)
                menuContent[selection.index].confirmEvent.Invoke();
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
                if (infoView) { infoView = false; SelectTransition(0); }
                else menuContent[selection.index].backEvent.Invoke();
        }
    }

    public void SelectTransition(int trans) //選擇選單動畫啟用
    {
        transition = trans;
        foreach (MenuList item in menuLists)
        {
            item.index += trans;
            if (item.index < 0) item.index += menuContent.Count;
            if (item.index > menuContent.Count - 1) item.index -= menuContent.Count;
            item.GetComponentInChildren<Text>().text = menuContent[item.index].textSource[0].text;
        }
        for (int i = 0; i < menuContent[selection.index].textDisplay.Count; i++)
            menuContent[selection.index].textDisplay[i].text = menuContent[selection.index].textSource[i].text;
        if (trans != 0) menuContent[selection.index].selectEvent.Invoke();
    }

    public void BattleMenu(bool enable) //戰鬥選單啟用
    {
        if (enable)
        {
            cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
            cineTrans = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            PlayerController.InstallDevices(gameObject, pc);
            for (int i = 0; i < cineTarget.m_Targets.Length; i++)
                if (cineTarget.m_Targets[i].target.GetComponent<ActionSystem>().pc.pc == pc)
                    cineTarget.m_Targets[1 - i].weight = 0;
            cineTrans.m_ScreenX = .65f;
            foreach (PlayerController item in FindObjectsOfType<PlayerController>())
                item.isCtrl = false;
            foreach (AudioVolume item in FindObjectsOfType<AudioVolume>())
                if (item.audioType == AudioVolume.AudioType.bgm)
                    item.GetComponent<AudioSource>().volume = (float)GameSystem.playerData.bgmVol * item.scale * .5f;
        }
        else
        {
            cineTrans.m_ScreenX = .5f;
            for (int i = 0; i < cineTarget.m_Targets.Length; i++)
                cineTarget.m_Targets[i].weight = 1;
            foreach (PlayerController item in FindObjectsOfType<PlayerController>())
                if (!item.isCtrl) item.IsControl(true);
            foreach (AudioVolume item in FindObjectsOfType<AudioVolume>())
                if (item.audioType == AudioVolume.AudioType.bgm)
                    item.GetComponent<AudioSource>().volume = (float)GameSystem.playerData.bgmVol * item.scale;
        }
    }

    public void ChangeScene(string sceneID) //讓Unity Event轉換場景用
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Null"))
        {
            anim.Play("OnExit");
            sceneChangeID = sceneID;
        }
    }

    static public IEnumerator PreloadScene(string sceneID) //讓其他腳本轉換場景用
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        while (!asyncLoad.isDone)
            yield return null;
    }

    public void GameMode(int mode) //主選單選擇模式用
    {
        GameSystem.gamemode = mode;
    }

    public void ViewInfo()
    {
        infoView = true;
    }

    public void MoveList(MoveListDisplay mldp)
    {
        infoTarget = mldp.contenTarget;
        mldp.contenTarget.text = pc == 0 ? mldp.skills[GameSystem.p1Char] : mldp.skills[GameSystem.p2Char];
    }

    public void Reset()
    {
        for (int i = 0; i < menuContent.Count; i++)
            menuContent[i].textDisplay[0].text = menuContent[i].textSource[0].text;
    }
}