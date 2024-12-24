using WorldMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void OnClickStart()
    {
        GameData.InitData();
        GameData.CreateNewPlayer();
        Node.InitNode();

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
                        Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
