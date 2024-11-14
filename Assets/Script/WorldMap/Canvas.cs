using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CheonJiWoon
{
    public class Canvas : MonoBehaviour
    {
        State myState = State.NORMAL;
        enum State { NORMAL, INVENTORY, MAP }

        public GameObject inventory;
        public Image inventoryIcon;
        public GameObject map;
        public Image mapIcon;

        GameObject activeObject;
        Image activeIcon;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)) ChangeState(State.MAP);
            if (Input.GetKeyDown(KeyCode.I)) ChangeState(State.INVENTORY);
            if (Input.GetKeyDown(KeyCode.Escape)) ChangeState(State.NORMAL);
        }


        void ChangeState(State s)
        {
            if (myState != State.NORMAL)
            {
                activeObject.SetActive(false);
                Color iconColor = activeIcon.color;
                iconColor.a = 0.8f;
                activeIcon.color = iconColor;
            }

            if (myState == s) s = State.NORMAL;

            myState = s;
            switch (myState)
            {
                case State.NORMAL:
                    activeObject = null;
                    activeIcon = null;
                    break;
                case State.INVENTORY: activeObject = inventory;
                    activeIcon = inventoryIcon;
                    break;
                case State.MAP: activeObject = map;
                    activeIcon = mapIcon;
                    break;
            }

            if(myState != State.NORMAL)
            {
                activeObject.SetActive(true);
                Color iconColor = activeIcon.color;
                iconColor.a = 1.0f;
                activeIcon.color = iconColor;
            }
        }
    }
}
