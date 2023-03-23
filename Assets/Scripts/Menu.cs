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
    Animator anim;
    Cinemachine.CinemachineTargetGroup cineTarget;
    Cinemachine.CinemachineFramingTransposer cineTrans;
    public string sceneChangeID = "";

    void Start()
    {
        anim = GetComponent<Animator>();
        SelectTransition(0);
    }

    void FixedUpdate()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Null") && transition == 0)
        {
            if (arrow > 0) anim.Play("Next", 0, 0);
            if (arrow < 0) anim.Play("Previous", 0, 0);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Null") && sceneChangeID != "")
        {
            if (sceneChangeID == "Exit") Application.Quit();
            else StartCoroutine(PreloadScene(sceneChangeID));
            sceneChangeID = "";
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
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
                menuContent[selection.index].confirmEvent.Invoke();
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
                menuContent[selection.index].backEvent.Invoke();
        }
    }

    public void SelectTransition(int trans)
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

    public void BattleEnable()
    {
        cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        cineTrans = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
        PlayerController.InstallDevices(gameObject, pc);
        for (int i = 0; i < cineTarget.m_Targets.Length; i++)
            if (cineTarget.m_Targets[i].target.GetComponent<ActionSystem>().pc.pc == pc)
                cineTarget.m_Targets[1 - i].weight = 0;
        cineTrans.m_ScreenX = .65f;
        foreach (PlayerController item in FindObjectsOfType<PlayerController>())
        { item.movesNum = 5; item.moveString = "5"; item.comString = "N"; }
    }

    public void ChangeScene(string sceneID)
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Null"))
        {
            anim.Play("OnExit");
            sceneChangeID = sceneID;
        }
    }

    static public IEnumerator PreloadScene(string sceneID)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        while (!asyncLoad.isDone)
            yield return null;
    }

    public void GameMode(int mode)
    {
        GameSystem.gamemode = mode;
    }
}