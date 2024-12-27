using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Combat
{
    public class Armor : MonoBehaviour
    {
        public TMP_Text text;

        void Start()
        {
            OnUpdate();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnUpdate()
        {
            text.text = BattleManager.inst.player.battleStat.armor.ToString();
        }
    }
}

