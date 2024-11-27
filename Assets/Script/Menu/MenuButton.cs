using CheonJiWoon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    public void OnClickStart()
    {
        GameData.InitData();
        Node.InitNode();

        GameData.playerStat = new BattleStat(60, 0, 10);
        GameData.playerStat.curHP = GameData.playerStat.maxHP;
        Loading.LoadScene(2);
    }
    public void OnClickLoad()
    {
        GameData.LoadData();
        Loading.LoadScene(2);
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
