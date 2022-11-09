using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleMenu : MonoBehaviour
{
    public int layer, select, arrow, language;
    public Animator menuAnim;
    [HideInInspector] public bool changeLayer, changeSelect;
    public List<Text> menuOptions;
    public List<string> menuOptionsText;
    public Text infoTitle, infoContent;
    public List<MainMenu.ContentName> contentName;

    void Start()
    {
        changeLayer = true; changeSelect = true;
        menuAnim = GetComponent<Animator>();
        foreach (Text item in menuOptions)
            menuOptionsText.Add(item.text);
    }

    void FixedUpdate()
    {
        if (layer == 0)
        {
            if (changeLayer)
            {
                changeLayer = false;
                GetComponent<PlayerInput>().user.UnpairDevices();
                foreach (PlayerController item in FindObjectsOfType<PlayerController>())
                { item.movesNum = 5; item.moveString = "5"; }
            }
            if (select > 4) select = 0; if (select < 0) select = 4;
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Normal"))
                if (arrow == 1) menuAnim.Play("Next");
                else if (arrow == -1) menuAnim.Play("Previous");
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Next") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && changeSelect)
            { changeSelect = false; select++; menuOptionsText.Add(menuOptionsText[0]); menuOptionsText.RemoveAt(0); }
            else if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Previous") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && changeSelect)
            { changeSelect = false; select--; menuOptionsText.Insert(0, menuOptionsText[menuOptionsText.Count - 1]); menuOptionsText.RemoveAt(menuOptionsText.Count - 1); }
            if ((menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Next") || menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Previous")) && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                changeSelect = true;
                for (int i = 0; i < menuOptions.Count; i++)
                    menuOptions[i].text = menuOptionsText[i];
                menuAnim.Play("Normal");
            }
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("OnExit") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                menuAnim.Play("Null");
                if (select == 0)
                    StartCoroutine(PreloadScene(SceneManager.GetActiveScene().name));
                else if (select == 4)
                    SceneManager.LoadSceneAsync("Main");
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
            if (menuAnim.GetCurrentAnimatorStateInfo(0).IsName("OnExit") && menuAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                gameObject.SetActive(false);
    }

    IEnumerator PreloadScene(string sceneID)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneID);
        while (!asyncLoad.isDone)
            yield return null;
    }

    public void SelectMotion(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().y < -.1f)
            arrow = 1;
        else if (ctx.ReadValue<Vector2>().y > .1f)
            arrow = -1;
        else arrow = 0;
    }

    public void SelectAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed)
        {
            if (ctx.action.name == "Start")
            {
                layer = -1; menuAnim.Play("OnExit");
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
            {
                if (layer == 0)
                {
                    if (select == 0 || select == 4)
                        menuAnim.Play("OnExit");
                }
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
            {
                layer = -1; menuAnim.Play("OnExit");
            }
        }
    }

    public string ReadContent(string name, int split)
    {
        foreach (MainMenu.ContentName item in contentName)
        {
            if (item.title == name)
                if (language == 0)
                    return item.zh.Split('$')[split];
                else if (language == 1)
                    return item.en.Split('$')[split];
                else if (language == 2)
                    return item.jp.Split('$')[split];
        }
        return null;
    }
}