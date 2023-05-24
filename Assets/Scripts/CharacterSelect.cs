using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public List<int> pc, layer, arrowX, arrowY, checkID, color;
    public List<RectTransform> selectTrans, complete;
    public List<Text> nameObj, signObj, colorNum;
    public List<Animator> selectAnim, skinAnim;
    public Vector2 selectRange;
    public List<ActionSystem> selectChar;
    public Transform transCharL, transCharR;
    [System.Serializable]
    public struct SelectContent
    {
        public int selectID;
        public Text selectName, selectSign;
        public RectTransform selectTrans;
    }
    public List<SelectContent> selectContent;
    public int colorCount;

    void Start()
    {
        GameSystem.p1Comp = false; GameSystem.p2Comp = false;
        for (int i = 0; i < selectContent.Count; i++)
        {
            if (selectContent[i].selectID == GameSystem.p1Char) pc[0] = i;
            if (selectContent[i].selectID == GameSystem.p2Char) pc[1] = i;
        }
        color[0] = GameSystem.p1Color; color[1] = GameSystem.p2Color;
        colorNum[0].text = (color[0] + 1).ToString("00"); colorNum[1].text = (color[1] + 1).ToString("00");
    }

    void FixedUpdate()
    {
        for (int i = 0; i < pc.Count; i++)
        {
            if (layer[i] == 0)
            {
                complete[i].gameObject.SetActive(false);
                if (arrowX[i] > 0 && selectAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f) pc[i]++;
                else if (arrowX[i] < 0 && selectAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f) pc[i]--;
                if (arrowY[i] > 0 && skinAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f)
                    if (color[i] < colorCount - 1) color[i]++; else color[i] = 0;
                else if (arrowY[i] < 0 && skinAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f)
                    if (color[i] > 0) color[i]--; else color[i] = colorCount - 1;
            }
            if (layer[i] == -1)
            {
                layer[i] = -2;
                GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
                StartCoroutine(Menu.PreloadScene("Main"));
            }
            if (layer[i] == 1 && !complete[i].gameObject.activeSelf)
            {
                complete[i].gameObject.SetActive(true);
            }
            if (layer[i] == 2)
            {
                layer[i] = 3;
                if (i == 0)
                {
                    GameSystem.p1Char = ConfirmCharacter(checkID[i]);
                    GameSystem.p2Char = ConfirmCharacter(checkID[1]);
                    GameSystem.p1Comp = true;
                }
                else
                {
                    GameSystem.p2Char = ConfirmCharacter(checkID[i]);
                    GameSystem.p1Char = ConfirmCharacter(checkID[0]);
                    GameSystem.p2Comp = true;
                }
                GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
                if (GameSystem.gamemode == 0)
                    StartCoroutine(Menu.PreloadScene("VersusMode"));
                if (GameSystem.gamemode == 1)
                    StartCoroutine(Menu.PreloadScene("PracticeMode"));
            }
            if (layer[i] == 4)
            {
                layer[0] = 5; layer[1] = 5;
                GameSystem.p1Char = ConfirmCharacter(checkID[0]); GameSystem.p1Comp = false;
                GameSystem.p2Char = ConfirmCharacter(checkID[1]); GameSystem.p2Comp = false;
                GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
                if (GameSystem.gamemode == 0)
                    StartCoroutine(Menu.PreloadScene("VersusMode"));
                if (GameSystem.gamemode == 1)
                    StartCoroutine(Menu.PreloadScene("PracticeMode"));
            }
            if (pc[i] > selectRange.y) pc[i] = (int)selectRange.x; if (pc[i] < selectRange.x) pc[i] = (int)selectRange.y;
            selectTrans[i].position = Vector2.Lerp(selectTrans[i].position, selectContent[pc[i]].selectTrans.position, .2f);
            nameObj[i].text = selectContent[pc[i]].selectName.text; signObj[i].text = selectContent[pc[i]].selectSign.text;
            if (checkID[i] != selectContent[pc[i]].selectID)
            {
                selectAnim[i].Play("OnSelect", -1, 0);
                checkID[i] = selectContent[pc[i]].selectID;
                if (i == 0)
                {
                    if (transCharL.GetComponentInChildren<ActionSystem>() != null)
                        Destroy(transCharL.GetComponentInChildren<ActionSystem>().gameObject);
                    if (checkID[i] != -1)
                    {
                        ActionSystem _char = Instantiate(selectChar[checkID[i]], transCharL);
                        _char.transform.localPosition = new Vector3(_char.transform.localPosition.x, _char.transform.localPosition.y, 0);
                    }
                    foreach (CharacterColor item in transCharL.GetComponentsInChildren<CharacterColor>())
                        item.ColorChange(0);
                }
                else
                {
                    if (transCharR.GetComponentInChildren<ActionSystem>() != null)
                        Destroy(transCharR.GetComponentInChildren<ActionSystem>().gameObject);
                    if (checkID[i] != -1)
                    {
                        ActionSystem _char = Instantiate(selectChar[checkID[i]], transCharR);
                        _char.transform.localPosition = new Vector3(-_char.transform.localPosition.x, _char.transform.localPosition.y, 0);
                        _char.root.localScale = new Vector3(-_char.root.localScale.x, _char.root.localScale.y, _char.root.localScale.z);
                    }
                    foreach (CharacterColor item in transCharR.GetComponentsInChildren<CharacterColor>())
                        item.ColorChange(1);
                }
            }
        }
        if (GameSystem.p1Color != color[0])
        {
            skinAnim[0].Play("Select", -1, 0);
            colorNum[0].text = (color[0] + 1).ToString("00");
            GameSystem.p1Color = color[0];
            foreach (CharacterColor item in transCharL.GetComponentsInChildren<CharacterColor>())
                item.ColorChange(0);
        }
        if (GameSystem.p2Color != color[1])
        {
            skinAnim[1].Play("Select", -1, 0);
            colorNum[1].text = (color[1] + 1).ToString("00");
            GameSystem.p2Color = color[1];
            foreach (CharacterColor item in transCharR.GetComponentsInChildren<CharacterColor>())
                item.ColorChange(1);
        }
        if (layer[0] == 1 && layer[1] == 1)
        {
            layer[0] = 3; layer[1] = 3;
            GameSystem.p1Char = ConfirmCharacter(checkID[0]);
            GameSystem.p2Char = ConfirmCharacter(checkID[1]);
            GameSystem.p1Comp = true; GameSystem.p2Comp = true;
            GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
            if (GameSystem.gamemode == 0)
                StartCoroutine(Menu.PreloadScene("VersusMode"));
            if (GameSystem.gamemode == 1)
                StartCoroutine(Menu.PreloadScene("PracticeMode"));
        }
    }

    int ConfirmCharacter(int id)
    {
        if (id != -1) return id;
        return Random.Range(0, selectChar.Count);
    }
}
