using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public List<int> pc, layer, arrow, checkID;
    public List<RectTransform> selectTrans, complete;
    public List<Text> nameObj, signObj;
    public List<Animator> selectAnim;
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

    void Start()
    {
        GameSystem.p1Comp = false; GameSystem.p2Comp = false;
    }

    void FixedUpdate()
    {
        for (int i = 0; i < pc.Count; i++)
        {
            if (layer[i] == 0)
            {
                complete[i].gameObject.SetActive(false);
                if (arrow[i] > 0 && selectAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f) pc[i]++;
                else if (arrow[i] < 0 && selectAnim[i].GetCurrentAnimatorStateInfo(0).normalizedTime > .5f) pc[i]--;
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
                        Instantiate(selectChar[checkID[i]], transCharL);
                }
                else
                {
                    if (transCharR.GetComponentInChildren<ActionSystem>() != null)
                        Destroy(transCharR.GetComponentInChildren<ActionSystem>().gameObject);
                    if (checkID[i] != -1)
                        Instantiate(selectChar[checkID[i]], transCharR);
                }
            }
        }
        if (layer[0] == 1 && layer[1] == 1)
        {
            layer[0] = 3; layer[1] = 3;
            GameSystem.p1Char = ConfirmCharacter(checkID[0]);
            GameSystem.p2Char = ConfirmCharacter(checkID[1]);
            GameSystem.p1Comp = true; GameSystem.p2Comp = true;
            GameObject.Find("LoadingCover").GetComponent<Animator>().Play("FadeOut", -1, 0);
            if (GameSystem.gamemode == 1)
                StartCoroutine(Menu.PreloadScene("PracticeMode"));
        }
    }

    int ConfirmCharacter(int id)
    {
        if (id != -1) return id;
        return Random.Range(0, 2);
    }
}
