using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CheonJiWoon
{
    public class World : MonoBehaviour
    {
        public static World instance { get; private set; }

        int xSize = 9;
        int ySize = 12;
        int startPointCount = 4;
        Vector2 dist = new Vector2(5.0f, 12.0f);
        Vector3 orgPos;
        HashSet<Node> completeNode;

        public Transform lines;
        public Transform Islands;
        public UnityEvent<Node> OnClickNode;

        void Awake()
        {
            instance = this;
            orgPos = -new Vector3(xSize * dist.x, 0.0f, ySize * dist.y) * 0.5f;
            lines.position = Islands.position = orgPos;
            if (Node.map == null) MapGenerate();
            completeNode = new HashSet<Node>();
            Node.pos = new Dictionary<Node, Vector3>();
            CreateAllObject(Node.firstNode);
        }

        void MapGenerate()
        {
            Node.map = new Dictionary<(int, int), Node>();
            Node.firstNode = new Node(xSize / 2, -2, Node.Type.START);
            Node.lastNode = new Node(xSize / 2, ySize + 1, Node.Type.END);

            HashSet<int> startPointCheck = new HashSet<int>();
            while (startPointCheck.Count < startPointCount)
            {
                int random = Random.Range(0, xSize);
                if (!startPointCheck.Contains(random))
                {
                    startPointCheck.Add(random);

                    Node newNode = new Node(random, 0);

                    Node.firstNode.children.Add(newNode.GetKey());
                    CreatePaths(newNode, 2, 2);
                }
            }
        }

        void CreatePaths(Node parent, int pathNumbs, int range)
        {
            int depth = parent.y + 1;
            int next = depth + 1;

            int SearchRangeMin = Mathf.Max(0, parent.x - (range * 2) + 1);
            int SearchRangeMax = Mathf.Min(xSize - 1, parent.x + (range * 2) - 1);

            int min = Mathf.Max(0, parent.x - range);
            int max = Mathf.Min(xSize - 1, parent.x + range);

            for (int i = parent.x - 1; i >= SearchRangeMin; i--)
            {
                if (Node.map.ContainsKey((parent.y, i)) && Node.GetNode(parent.y, i).children.Count > 0)
                    min = Mathf.Max(min, Node.GetNode(parent.y, i).max);
            }

            for (int i = parent.x + 1; i <= SearchRangeMax; i++)
            {
                if (Node.map.ContainsKey((parent.y, i)) && Node.GetNode(parent.y, i).children.Count > 0)
                    max = Mathf.Min(max, Node.GetNode(parent.y, i).min);
            }

            max++;

            int limitCount = Random.Range(1, 10);
            limitCount = limitCount * (pathNumbs + 1) / 10;
            if (limitCount == 0) limitCount = 1;
            limitCount = Mathf.Min(limitCount, max - min);

            HashSet<int> check = new HashSet<int>();
            while (check.Count < limitCount)
            {
                int randomX = Random.Range(min, max);
                if (!check.Contains(randomX))
                {
                    check.Add(randomX);

                    if (parent.children.Count > 0)
                    {
                        parent.max = Mathf.Max(randomX, parent.max);
                        parent.min = Mathf.Min(randomX, parent.min);
                    }
                    else
                    {
                        parent.max = randomX;
                        parent.min = randomX;
                    }

                    if (!Node.map.ContainsKey((depth, randomX)))
                    {
                        Node newNode = new Node(randomX, depth);
                        if (ySize > next) CreatePaths(newNode, pathNumbs, range);
                        else newNode.children.Add(Node.lastNode.GetKey());
                    }
                    parent.children.Add(Node.GetNode(depth, randomX).GetKey());
                }
            }
        }

        void InitNode(Node node)
        {
            GameObject obj = Instantiate(Resources.Load($"{SceneData.prefabPath}/{node.GetFilePath()}") as GameObject, Islands);
            obj.transform.localPosition = new Vector3(node.x * dist.x, 0.0f, node.y * dist.y);
            Node.pos.Add(node, obj.transform.position);
            obj.GetComponent<Island>().myNode = node;

            IClickAction clickComponent = obj.GetComponent<IClickAction>();
            if (clickComponent != null) clickComponent.ClickAction += () => OnClickNode.Invoke(node);
        }


        void CreateLine(Node startNode, Node endNode)
        {
            GameObject obj = Instantiate(Resources.Load($"{SceneData.prefabPath}/Line") as GameObject, lines);
            LineRenderer renderer = obj.GetComponent<LineRenderer>();
            Vector3[] lineList = { startNode.GetPos(), endNode.GetPos() };
            renderer.positionCount = lineList.Length;
            renderer.SetPositions(lineList);
        }

        void CreateAllObject(Node parent)
        {
            if (completeNode.Contains(parent)) return;
            completeNode.Add(parent);
            InitNode(parent);
            foreach ((int,int) key in parent.children)
            {
                CreateAllObject(Node.GetNode(key.Item1, key.Item2));
                CreateLine(parent, Node.GetNode(key.Item1, key.Item2));
            }
        }
    }
}

