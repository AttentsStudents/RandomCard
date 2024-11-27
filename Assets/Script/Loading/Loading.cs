using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    public static int nScene;
    void Start() => SceneManager.LoadSceneAsync(nScene);

    public static void LoadScene(int n)
    {
        nScene = n;
        SceneManager.LoadSceneAsync(0);
    }
}
