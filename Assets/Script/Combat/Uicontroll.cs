using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uicontroll : MonoBehaviour
{
    public Transform deckPanel;
    public Transform handPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClosedDeck()
    {
        deckPanel.gameObject.SetActive(false);
        handPanel.gameObject.SetActive(true);
    }
    public void ClosedHand()
    {
        handPanel.gameObject.SetActive(false);
    }
}
