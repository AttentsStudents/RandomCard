using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Node
    {
        public GameObject gameobject { get; set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public int min { get; set; }
        public int max { get; set; }

        public List<Line> paths = new List<Line>();

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Line
    {
        public GameObject gameobject { get; private set; }
        public Node node { get; private set; }

        public Line(GameObject obj, Node nd)
        {
            gameobject = obj;
            node = nd;
        }
    }
}
