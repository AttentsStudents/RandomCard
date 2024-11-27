using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Config : MonoBehaviour
{
    public void OnMainMenu() => Loading.LoadScene(Scene.STARTMENU);
    public void OnExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // ���ø����̼� ����
        #endif
    }
}
