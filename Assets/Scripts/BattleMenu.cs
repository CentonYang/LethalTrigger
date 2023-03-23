using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleMenu : MonoBehaviour
{
    public int pc, layer, select, arrowV, arrowH, language;
    public Animator menuAnim;
    [HideInInspector] public bool changeLayer, changeSelect;
    public List<Text> menuOptions;
    public List<string> menuOptionsText;
    public Text infoTitle, infoContent;
    public List<MainMenu.ContentName> contentName;
    public string exitScene;
    public GameObject moveList;
    public List<GameObject> hideUI;
    Cinemachine.CinemachineTargetGroup cineTarget;
    Cinemachine.CinemachineFramingTransposer cineTrans;

    void Start()
    {
        changeLayer = true; changeSelect = true;
        menuAnim = GetComponent<Animator>();
        foreach (Text item in menuOptions)
            menuOptionsText.Add(item.text);
        cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        cineTrans = GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
    }

    void FixedUpdate()
    {
        if (layer == 0)
        {
            if (changeLayer)
            {
                changeLayer = false;
                PlayerController.InstallDevices(gameObject, pc);
                for (int i = 0; i < cineTarget.m_Targets.Length; i++)
                    if (cineTarget.m_Targets[i].target.GetComponent<ActionSystem>().pc.pc == pc)
                        cineTarget.m_Targets[1 - i].weight = 0;
                cineTrans.m_ScreenX = .65f;
                foreach (PlayerController item in FindObjectsOfType<PlayerController>())
                { item.movesNum = 5; item.moveString = "5"; item.comString = "N"; }
                foreach (GameObject item in hideUI)
                    item.SetActive(false);
            }
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
            {
                changeSelect = true;
                if (arrowV == 1) menuAnim.Play("Next");
                else if (arrowV == -1) menuAnim.Play("Previous");
            }
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Next") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && changeSelect)
            { changeSelect = false; select++; menuOptionsText.Add(menuOptionsText[0]); menuOptionsText.RemoveAt(0); }
            else if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Previous") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && changeSelect)
            { changeSelect = false; select--; menuOptionsText.Insert(0, menuOptionsText[menuOptionsText.Count - 1]); menuOptionsText.RemoveAt(menuOptionsText.Count - 1); }
            if (select > 4) select = 0; if (select < 0) select = 4;
            if ((menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Next") || menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Previous")) && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                for (int i = 0; i < menuOptions.Count; i++)
                    menuOptions[i].text = menuOptionsText[i];
                menuAnim.Play("Normal");
            }
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("OnExit") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                menuAnim.Play("Null");
                if (select == 0)
                {
                    GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
                    //StartCoroutine(MainMenu.PreloadScene(SceneManager.GetActiveScene().name));
                }
                else if (select == 4)
                    SceneManager.LoadSceneAsync(exitScene);
            }
            switch (select)
            {
                case 0: infoTitle.text = ReadContent("Reset", 0); infoContent.text = ReadContent("Reset", 1); break;
                case 1: infoTitle.text = ReadContent("MoveList", 0); infoContent.text = ReadContent("MoveList", 1); break;
                case 2: infoTitle.text = ReadContent("PracticeSetting", 0); infoContent.text = ReadContent("PracticeSetting", 1); break;
                case 3: infoTitle.text = ReadContent("Setting", 0); infoContent.text = ReadContent("Setting", 1); break;
                case 4: infoTitle.text = ReadContent("Exit", 0); infoContent.text = ReadContent("Exit", 1); break;
            }
        }
        if (layer == -1)
        {
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("OnExit") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                foreach (GameObject item in hideUI)
                    item.SetActive(true);
                cineTrans.m_ScreenX = .5f;
                for (int i = 0; i < cineTarget.m_Targets.Length; i++)
                    cineTarget.m_Targets[i].weight = 1;
                gameObject.SetActive(false);
            }
        }
        if (layer == 1)
        {
            if (select == 1)
            {
                infoContent.rectTransform.Translate(0, arrowV * 4, 0);
                infoContent.rectTransform.Translate(arrowH * 2, 0, 0);
            }
        }
    }

    public void SelectMotion(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().y < -.1f)
            arrowV = 1;
        else if (ctx.ReadValue<Vector2>().y > .1f)
            arrowV = -1;
        else arrowV = 0;
        if (ctx.ReadValue<Vector2>().x < -.1f)
            arrowH = 1;
        else if (ctx.ReadValue<Vector2>().x > .1f)
            arrowH = -1;
        else arrowH = 0;
    }

    public void SelectAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (ctx.action.name == "Start" || ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
            {
                if (layer == 0)
                { layer = -1; menuAnim.Play("OnExit"); }
                if (layer == 1) layer = 0;
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
            {
                if (layer == 0)
                {
                    if (select == 0 || select == 4)
                        menuAnim.Play("OnExit");
                    if (select == 1)
                    {
                        layer = 1;
                        infoContent.text = moveList.GetComponent<MoveListDisplay>().skills[pc == 0 ? GameSystem.p1Char : GameSystem.p2Char];
                    }
                }
            }
        }
    }

    public string ReadContent(string name, int split)
    {
        foreach (MainMenu.ContentName item in contentName)
        {
            if (item.title == name)
                if (language == 0)
                    return item.zh.Split('§')[split];
                else if (language == 1)
                    return item.en.Split('§')[split];
                else if (language == 2)
                    return item.jp.Split('§')[split];
        }
        return null;
    }
}