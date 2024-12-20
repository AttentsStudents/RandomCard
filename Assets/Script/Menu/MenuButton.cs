using WorldMap;
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

        GameData.playerStat = new PlayerStat(60, 0, 10, 6);
        GameData.playerStat.curHP = GameData.playerStat.maxHP;
        Loading.LoadScene(Scene.WORLDMAP);
    }
    public void OnClickLoad()
    {
        GameData.LoadData();
        Loading.LoadScene(Scene.WORLDMAP);
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit(); // ���ø����̼� ����
        #endif
    }
}
