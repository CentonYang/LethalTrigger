using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeTimeMap : MonoBehaviour
{
    public Menu menu;
    public int isList;

    void FixedUpdate()
    {
        if (isList > 0)
            GetComponent<Text>().text = GetComponent<Translater>().contents[GameSystem.playerData.language] + (isList == 1 ? GameSystem.life : GameSystem.timer);
    }

    public void Add(int id) //id 0 = Life, 1 = Time, 2 = Map
    {
        switch (id)
        {
            case 0: GameSystem.life = GameSystem.life >= 5 ? 1 : (GameSystem.life + 1); break;
            case 1: GameSystem.timer = GameSystem.timer >= 180 ? 60 : (GameSystem.timer + 30); break;
        }
        if (id < 2)
        {
            menu.menuContent[menu.selection.index].textDisplay[0].text = menu.menuContent[menu.selection.index].textSource[0].GetComponent<Translater>().contents[GameSystem.playerData.language] + (id == 0 ? GameSystem.life : GameSystem.timer);
            menu.menuContent[menu.selection.index].textDisplay[1].text = menu.menuContent[menu.selection.index].textSource[1].GetComponent<Translater>().contents[GameSystem.playerData.language] + (id == 0 ? GameSystem.life : GameSystem.timer);
        }
    }
}
