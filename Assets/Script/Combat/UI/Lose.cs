using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose : MonoBehaviour
{
    public Transform modal;
    private void Start()
    {
        modal.gameObject.SetActive(false);
    }

    public void OnClickLoseButton()
    {
        Loading.LoadScene(Scene.STARTMENU);
    }
    public void OnViewModal()
    {
        modal.gameObject.SetActive(true);
    }
}
