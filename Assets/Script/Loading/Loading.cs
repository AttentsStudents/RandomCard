using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static int nScene;

    void Start() => SceneManager.LoadSceneAsync(nScene);

    public static void LoadScene(Scene scene)
    {
        nScene = (int)scene;
        SceneManager.LoadScene(0);
    }
}
