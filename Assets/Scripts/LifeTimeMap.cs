using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeMap : MonoBehaviour
{
    public Menu menu;
    public Vector3Int lifeNStep, timerNStep;
    public int mapCount;
    public List<Text> mapName;
    public List<GameObject> mapObjs;
    public int isList;
    int mapId;

    void FixedUpdate()
    {
        if (isList > 0 && isList < 3) //isList 0 = Menu , 1 = Life, 2 = Time, 3 = Map
            GetComponent<Text>().text = GetComponent<Translater>().contents[GameSystem.playerData.language] + (isList == 1 ? GameSystem.life : GameSystem.timer);
        else if (isList == 3)
            GetComponent<Text>().text = mapName[GameSystem.map].GetComponent<Translater>().contents[GameSystem.playerData.language];
        else if (GameSystem.map != mapId)
        {
            mapId = GameSystem.map;
            foreach (GameObject item in mapObjs)
                item.SetActive(false);
            mapObjs[GameSystem.map].SetActive(true);
        }
    }

    public void Add(int id) //id 0 = Life, 1 = Time, 2 = Map
    {
        switch (id)
        {
            case 0: GameSystem.life = GameSystem.life >= lifeNStep.y ? lifeNStep.x : (GameSystem.life + lifeNStep.z); break;
            case 1: GameSystem.timer = GameSystem.timer >= timerNStep.y ? timerNStep.x : (GameSystem.timer + timerNStep.z); break;
            case 2: GameSystem.map = GameSystem.map >= mapCount ? 0 : (GameSystem.map + 1); break;
        }
        if (id < 2)
        {
            menu.menuContent[menu.selection.index].textDisplay[0].text = menu.menuContent[menu.selection.index].textSource[0].GetComponent<Translater>().contents[GameSystem.playerData.language] + (id == 0 ? GameSystem.life : GameSystem.timer);
            menu.menuContent[menu.selection.index].textDisplay[1].text = menu.menuContent[menu.selection.index].textSource[1].GetComponent<Translater>().contents[GameSystem.playerData.language] + (id == 0 ? GameSystem.life : GameSystem.timer);
        }
        else if (id == 2)
        {
            menu.menuContent[menu.selection.index].textDisplay[0].text = mapName[GameSystem.map].GetComponent<Translater>().contents[GameSystem.playerData.language];
            menu.menuContent[menu.selection.index].textDisplay[1].text = mapName[GameSystem.map].GetComponent<Translater>().contents[GameSystem.playerData.language];
        }
    }
}
