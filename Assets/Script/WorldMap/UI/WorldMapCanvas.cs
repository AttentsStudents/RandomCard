using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CheonJiWoon
{

    public partial class WorldMapCanvas : MonoBehaviour
    {
        State myState = State.NORMAL;
        public enum State { NORMAL, INVENTORY, MAP, CONFIG }

        GameObject activeObject;
        Image activeIcon;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M)) ChangeState(State.MAP);
            if (Input.GetKeyDown(KeyCode.I)) ChangeState(State.INVENTORY);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (myState == State.NORMAL) ChangeState(State.CONFIG);
                else ChangeState(State.NORMAL);
            }
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
            if (myState == State.NORMAL)
            {
                activeObject = null;
                activeIcon = null;
            }
            else
            {
                WorldMapMenu selectMenu = LinkStateMenu.GetValueOrDefault(myState);
                activeObject = selectMenu.core;
                activeIcon = selectMenu.icon.image;
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
        public WorldMapMenu[] MenuList;
        Dictionary<State, WorldMapMenu> LinkStateMenu;

        void Awake()
        {
            LinkStateMenu = new Dictionary<State, WorldMapMenu>();
            foreach (WorldMapMenu menu in MenuList)
            {
                LinkStateMenu.Add(menu.state, menu);
                menu.icon.ClickAction = () => ChangeState(menu.state);
            }
        }

        public void OnClose() => ChangeState(State.NORMAL);
    }

    [Serializable]
    public class WorldMapMenu
    {
        public GameObject core;
        public MenuIcon icon;
        [SerializeField] public WorldMapCanvas.State state;
    }
}
