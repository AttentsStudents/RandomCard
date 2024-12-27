using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{
    public Transform modal;
    private void Start()
    {
        modal.gameObject.SetActive(false);
    }

    public void OnClickVictoryButton()
    {
        Loading.LoadScene(Scene.WORLDMAP);
    }
    public void OnViewModal()
    {
        modal.gameObject.SetActive(true);
    }
}
