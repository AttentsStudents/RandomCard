using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Node
    {
        public string filePath { get; set; }
        public GameObject gameobject { get; set; }
        public int x { get; private set; }
        public int y { get; private set; }
        public int min { get; set; }
        public int max { get; set; }

        public HashSet<Node> children = new HashSet<Node>();

        public Node(int x, int y, string filePath = "Island")
        {
            this.x = x;
            this.y = y;
            this.filePath = filePath;
        }
    }
}
