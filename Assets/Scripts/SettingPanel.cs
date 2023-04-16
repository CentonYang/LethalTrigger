using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class SettingPanel : MonoBehaviour
{
    public int select, arrowX, arrowY;
    public List<Animator> settingAnims; //bgm, sfx, voice, lang
    public List<int> setNum;
    public List<Text> setNumTxt;
    public List<Slider> setSlider;
    float timeStepX = 0, timeStepY = 0;
    public UnityEvent closeEvent;
    GameSystem.PlayerData playerData;

    void Start()
    {
        setNum[0] = (int)(GameSystem.playerData.bgmVol * 100);
        setNum[1] = (int)(GameSystem.playerData.sfxVol * 100);
        setNum[2] = (int)(GameSystem.playerData.voiceVol * 100);
        setNum[3] = (int)(GameSystem.playerData.language);
        playerData = new GameSystem.PlayerData { bgmVol = (double)setNum[0] * .01, sfxVol = (double)setNum[1] * .01, voiceVol = (double)setNum[2] * .01, language = setNum[3] };
    }

    void FixedUpdate()
    {
        for (int i = 0; i < settingAnims.Count; i++)
            if (i == select)
                settingAnims[i].Play("Select");
            else settingAnims[i].Play("UnSelect");
        for (int i = 0; i < setSlider.Count; i++)
        {
            setNumTxt[i].text = setNum[i].ToString();
            setSlider[i].value = (float)setNum[i] / 100;
        }
        if (arrowX != 0)
        {
            if (timeStepX >= .25f) timeStepX = 0;
            if (timeStepX == 0)
                if (select != 3)
                {
                    setNum[select] += arrowX * 10;
                    if (setNum[select] > 100) setNum[select] = 0;
                    else if (setNum[select] < 0) setNum[select] = 100;
                    Save();
                }
                else if (select == 3)
                {
                    setNum[select] += arrowX;
                    if (setNum[select] > 2) setNum[select] = 0;
                    else if (setNum[select] < 0) setNum[select] = 2;
                    setNumTxt[select].text = setNumTxt[select].GetComponent<Translater>().contents[setNum[select]];
                }
            timeStepX += Time.fixedDeltaTime;
        }
        else timeStepX = 0;
        if (arrowY != 0)
        {
            if (timeStepY >= .25f) timeStepY = 0;
            if (timeStepY == 0)
                if ((select < settingAnims.Count - 1 && arrowY > 0) || (select > 0 && arrowY < 0)) select += arrowY;
            timeStepY += Time.fixedDeltaTime;
        }
        else timeStepY = 0;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().x < -.25f && ctx.ReadValue<Vector2>().y < .25f && ctx.ReadValue<Vector2>().y > -.25f)
            arrowX = -1;
        else if (ctx.ReadValue<Vector2>().x > .25f && ctx.ReadValue<Vector2>().y < .25f && ctx.ReadValue<Vector2>().y > -.25f)
            arrowX = 1;
        else arrowX = 0;
        if (ctx.ReadValue<Vector2>().y < -.25f && ctx.ReadValue<Vector2>().x < .25f && ctx.ReadValue<Vector2>().x > -.25f)
            arrowY = 1;
        else if (ctx.ReadValue<Vector2>().y > .25f && ctx.ReadValue<Vector2>().x < .25f && ctx.ReadValue<Vector2>().x > -.25f)
            arrowY = -1;
        else arrowY = 0;
    }

    public void Action(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
            { Save(); closeEvent.Invoke(); }
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
            { GameSystem.SaveGame(playerData); closeEvent.Invoke(); }
        }
    }

    void Save()
    {
        GameSystem.SaveGame(new GameSystem.PlayerData { bgmVol = (double)setNum[0] * .01, sfxVol = (double)setNum[1] * .01, voiceVol = (double)setNum[2] * .01, language = setNum[3] });
        foreach (AudioVolume item in FindObjectsOfType<AudioVolume>()) item.StartCoroutine("Start");
    }
}
