using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CheonJiWoon
{

    public partial class WorldMapCanvas : MonoBehaviour
    {
        State myState = State.NORMAL;
        public enum State { NORMAL, INVENTORY, MAP }



        GameObject activeObject;
        Image activeIcon;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)) ChangeState(State.MAP);
            if (Input.GetKeyDown(KeyCode.I)) ChangeState(State.INVENTORY);
            if (Input.GetKeyDown(KeyCode.Escape)) ChangeState(State.NORMAL);
        }


        public void ChangeState(State s)
        {
            if (myState != State.NORMAL)
            {
                activeObject.SetActive(false);
                Color iconColor = activeIcon.color;
                iconColor.a = SceneData.iconOffAlpha;
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
                case State.INVENTORY:
                    activeObject = inventory;
                    activeIcon = inventoryIcon;
                    break;
                case State.MAP:
                    activeObject = map;
                    activeIcon = mapIcon;
                    break;
            }

            if (myState != State.NORMAL)
            {
                activeObject.SetActive(true);
                Color iconColor = activeIcon.color;
                iconColor.a = SceneData.iconOnAlpha;
                activeIcon.color = iconColor;
            }
        }
    }



    public partial class WorldMapCanvas
    {
        public GameObject inventory;
        public Image inventoryIcon;

        public GameObject map;
        public Image mapIcon;

        public void OnOpenInventory() => ChangeState(State.INVENTORY);
        public void OnOpenMap() => ChangeState(State.MAP);
        public void OnClose() => ChangeState(State.NORMAL);
    }
}
