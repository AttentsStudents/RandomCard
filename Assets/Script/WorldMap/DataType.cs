using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    [Serializable]
    public class Node
    {
        public static Dictionary<Node, Vector3> pos { get; set; }
        public static Dictionary<(int, int), Node> map { get; set; } 
        public static Node firstNode { get; set; }
        public static Node lastNode { get; set; }
        public static Node GetNode(int y, int x) => map.GetValueOrDefault((y, x));
        public static void InitNode()
        {
            pos = null;
            map = null;
            firstNode = null;
            lastNode = null;
        }
        public enum Type
        {
            RANDOM,
            MONSTER,
            TREASURE,
            REST,
            START,
            END
        }
        public Type type { get; set; }
        public bool clear { get; set; }
        public List<(int, int)> monsterInfo { get; set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public int min { get; set; }
        public int max { get; set; }
        List<(int, int)> _children = new List<(int, int)>();
        public List<(int, int)> children
        {
            get => _children;
            set => _children = value;
        }


        public Node(int x, int y, Type type = Type.RANDOM)
        {
            this.x = x;
            this.y = y;
            this.clear = false;
            if (type == Type.RANDOM) RandomMyType();
            else this.type = type;

            map.Add((y, x), this);
        }

        public string GetFilePath()
        {
            switch (type)
            {
                case Type.START:
                    return "StartPoint";
                case Type.END:
                    return "EndPoint";
                default:
                    return "MiddlePoint";
            }
        }

        void RandomMyType()
        {
            int random = UnityEngine.Random.Range(1, 8);
            if (random < 6) type = Type.MONSTER;
            else if (random < 7) type = Type.TREASURE;
            else type = Type.REST;
        }

        public Vector3 GetPos() => Node.pos.GetValueOrDefault(this);
        public (int, int) GetKey() => (this.y, this.x);
    }

}
