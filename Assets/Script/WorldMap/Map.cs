using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace CheonJiWoon
{
    public class Map : MonoBehaviour
    {
        public static Map instance { get; private set; }
        public Node[,] mapInfo { get; private set; }
        public Node firstNode { get; private set; }
        public Node lastNode { get; private set; }

        int xSize = 9;
        int ySize = 12;
        Vector2 dist = new Vector2(5.0f, 12.0f);
        Vector3 orgPos;

        int startPointCount = 4;
        public Transform lines;
        public Transform Islands;
        public UnityEvent<Node> OnClickNode;

        void Awake()
        {
            instance = this;
            orgPos = -new Vector3(xSize * dist.x, 0.0f, ySize * dist.y) * 0.5f;
            Generate();
        }

        void Generate()
        {
            mapInfo = new Node[ySize, xSize];
            lines.position = Islands.position = orgPos;

            firstNode = new Node(xSize/2, -2);
            CreateIsland(firstNode);

            HashSet<int> startPointCheck = new HashSet<int>();
            while (startPointCheck.Count < startPointCount)
            {
                int random = Random.Range(0, xSize);
                if (!startPointCheck.Contains(random))
                {
                    startPointCheck.Add(random);

                    Node newNode = new Node(random, 0);
                    CreateIsland(newNode);

                    mapInfo[0, random] = newNode;
                    CreatePaths(newNode, 2, 2);

                    CreateLine(firstNode, newNode);
                }
            }

            lastNode = new Node(xSize / 2, ySize + 1);
            CreateIsland(lastNode);
            int endLine = ySize - 1;
            for (int i = 0; i < xSize; i++)
            {
                if (mapInfo[endLine, i] != null) CreateLine(mapInfo[endLine, i], lastNode);
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
                if (mapInfo[parent.y, i] != null && mapInfo[parent.y, i].paths.Count > 0) min = Mathf.Max(min, mapInfo[parent.y, i].max);
            }

            for (int i = parent.x + 1; i <= SearchRangeMax; i++)
            {
                if (mapInfo[parent.y, i] != null && mapInfo[parent.y, i].paths.Count > 0) max = Mathf.Min(max, mapInfo[parent.y, i].min);
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

                    if (parent.paths.Count > 0)
                    {
                        parent.max = Mathf.Max(randomX, parent.max);
                        parent.min = Mathf.Min(randomX, parent.min);
                    }
                    else
                    {
                        parent.max = randomX;
                        parent.min = randomX;
                    }



                    if (mapInfo[depth, randomX] == null)
                    {
                        Node newNode = new Node(randomX, depth);
                        mapInfo[depth, randomX] = newNode;
                        CreateIsland(newNode);
                        if (ySize > next) CreatePaths(newNode, pathNumbs, range);
                    }

                    CreateLine(parent, mapInfo[depth, randomX]);
                }
            }
        }

        void CreateIsland(Node node)
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/WorldMap/Island5") as GameObject, Islands);
            obj.transform.localPosition = new Vector3(node.x * dist.x, 0.0f, node.y * dist.y);

            node.gameobject = obj;
            node.gameobject.GetComponent<IClickAction>().ClickAction += () => OnClickNode.Invoke(node);
        }

        void CreateLine(Node startNode, Node endNode)
        {
            Vector3 start = new Vector3(startNode.x * dist.x, 0.2f, startNode.y * dist.y);
            Vector3 end = new Vector3(endNode.x * dist.x, 0.2f, endNode.y * dist.y);

            startNode.paths.Add(new Line(CreateLine(start + orgPos, end + orgPos), endNode));
        }

        GameObject CreateLine(Vector3 start, Vector3 end)
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/WorldMap/Line") as GameObject, lines);
            LineRenderer renderer = obj.GetComponent<LineRenderer>();
            Vector3[] lineList = { start, end };
            renderer.positionCount = lineList.Length;
            renderer.SetPositions(lineList);

            return obj;
        }

    }
}

