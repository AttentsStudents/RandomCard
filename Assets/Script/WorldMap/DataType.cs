using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class WorldMapInfo
    {
        public Node[,] mapInfo { get; set; }
        public Node firstNode { get; set; }
        public Node lastNode { get; set; }
    }

    public class Node
    {
        public enum Type
        {
            NONE,
            MONSTER,
            TREASURE,
            REST
        }

        public string filePath { get; private set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public Type type { get; private set; }

        public Vector3 pos { get; set; }
        public int min { get; set; }
        public int max { get; set; }
        public bool clear { get; set; }

        public List<(int, int)> monsterInfo;
        public HashSet<Node> children = new HashSet<Node>();

        public Node(int x, int y, string filePath = "Island")
        {
            this.x = x;
            this.y = y;
            this.filePath = filePath;
            this.clear = false;
            RandomMyType();
        }

        void RandomMyType()
        {
            int random = Random.Range(1, 8);
            if (random < 6) type = Type.MONSTER;
            else if (random < 7) type = Type.TREASURE;
            else type = Type.REST;
        }
    }
}
