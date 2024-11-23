using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void OnClickStart()
    {
        GameData.playerStat = new BattleStat(60, 0, 10);
        GameData.playerStat.curHP = GameData.playerStat.maxHP;
        SceneManager.LoadSceneAsync(1);
    }
    public void OnClickLoad()
    {
        GameData.LoadData();
        SceneManager.LoadSceneAsync(1);
    }
}
