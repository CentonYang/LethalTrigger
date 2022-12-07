using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{

    public int layer, select, arrow, language;
    public GameObject title, menu;
    public List<GameObject> bgm;
    [HideInInspector] public Animator titleAnim, menuAnim;
    [HideInInspector] public bool changeLayer, changeSelect;
    public List<Text> menuOptions;
    public List<string> menuOptionsText;
    public Text infoTitle, infoContent;
    public List<ContentName> contentName;

    [System.Serializable]
    public struct ContentName
    {
        public string title;
        [TextArea] public string zh;
        [TextArea] public string en;
        [TextArea] public string jp;
    }

    void Start()
    {
        changeLayer = true; changeSelect = true;
        titleAnim = title.GetComponent<Animator>();
        menuAnim = menu.GetComponent<Animator>();
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
                title.SetActive(true);
                menu.SetActive(false);
                foreach (GameObject item in bgm) item.gameObject.SetActive(false);
            }
            if (titleAnim.GetCurrentAnimatorStateInfo(0).IsName("Press") && titleAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .99f)
            { layer = 1; select = 0; changeLayer = true; }
        }
        if (layer == 1)
        {
            if (changeLayer)
            {
                changeLayer = false;
                title.SetActive(false);
                menu.SetActive(true);
                foreach (GameObject item in bgm) item.gameObject.SetActive(false);
                bgm[0].SetActive(true);
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
                if (select == 1)
                    SceneManager.LoadSceneAsync("PracticeMode");
                else if (select == 4)
                    Application.Quit();
            }
            switch (select)
            {
                case 0: infoTitle.text = ReadContent("Versus", 0); infoContent.text = ReadContent("Versus", 1); break;
                case 1: infoTitle.text = ReadContent("Practice", 0); infoContent.text = ReadContent("Practice", 1); break;
                case 2: infoTitle.text = ReadContent("Collection", 0); infoContent.text = ReadContent("Collection", 1); break;
                case 3: infoTitle.text = ReadContent("Setting", 0); infoContent.text = ReadContent("Setting", 1); break;
                case 4: infoTitle.text = ReadContent("Exit", 0); infoContent.text = ReadContent("Exit", 1); break;
            }
        }
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
        if (ctx.phase != InputActionPhase.Performed && !menuAnim.GetCurrentAnimatorStateInfo(0).IsName("Null"))
        {
            if (ctx.action.name == "Start")
            {
                if (layer == 0)
                    if (titleAnim.GetCurrentAnimatorStateInfo(0).IsName("Loop"))
                        titleAnim.Play("Press");
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
            {
                if (layer == 1)
                {
                    if (select == 1 || select == 4)
                        menuAnim.Play("OnExit");
                }
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
            {

            }
        }
    }

    public string ReadContent(string name, int split)
    {
        foreach (ContentName item in contentName)
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
