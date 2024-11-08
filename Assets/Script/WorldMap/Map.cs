using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Node
    {
        public int x { get; private set; }
        public int y { get; private set; }
        public int min { get; set; }
        public int max { get; set; }

        public List<Node> nodes = new List<Node>();

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class Map : MonoBehaviour
    {
        Node[,] mapInfo;
        int xSize = 9;
        int ySize = 12;
        Vector2 dist = new Vector2(4.0f, 6.0f);
        Vector3 orgPos;

        int startPointCount = 4;
        public Transform lines;
        public Transform Islands;

        void Awake()
        {
            orgPos = -new Vector3(xSize * dist.x, 0.0f, ySize * dist.y) * 0.5f;
            Generate();
        }

        void Generate()
        {
            mapInfo = new Node[ySize, xSize];
            lines.position = Islands.position = orgPos;

            HashSet<int> startPointCheck = new HashSet<int>();
            while (startPointCheck.Count < startPointCount)
            {
                int random = Random.Range(0, xSize);
                if (!startPointCheck.Contains(random))
                {
                    startPointCheck.Add(random);

                    CreateIsland(random, 0);
                    Node newNode = new Node(random, 0);
                    mapInfo[0, random] = newNode;
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
                if (mapInfo[parent.y, i] != null && mapInfo[parent.y, i].nodes.Count > 0) min = Mathf.Max(min, mapInfo[parent.y, i].max);
            }

            for (int i = parent.x + 1; i <= SearchRangeMax; i++)
            {
                if (mapInfo[parent.y, i] != null && mapInfo[parent.y, i].nodes.Count > 0) max = Mathf.Min(max, mapInfo[parent.y, i].min);
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

                    if (parent.nodes.Count > 0)
                    {
                        parent.max = Mathf.Max(randomX, parent.max);
                        parent.min = Mathf.Min(randomX, parent.min);
                    }
                    else
                    {
                        parent.max = randomX;
                        parent.min = randomX;
                    }

                    Vector3 start = new Vector3(parent.x, 0.5f, parent.y);
                    Vector3 end = new Vector3(randomX, 0.5f, depth);

                    CreateLine(ref start, ref end);

                    if (mapInfo[depth, randomX] == null)
                    {
                        Node newNode = new Node(randomX, depth);
                        mapInfo[depth, randomX] = newNode;
                        CreateIsland(newNode.x, newNode.y);
                        if (ySize > next) CreatePaths(newNode, pathNumbs, range);
                    }

                    parent.nodes.Add(mapInfo[depth, randomX]);

                }
            }
        }

        void CreateIsland(int x, int z)
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/WorldMap/Island3") as GameObject, Islands);
            obj.transform.localPosition = new Vector3(x * dist.x, -0.05f, z * dist.y);
        }

        void CreateLine(ref Vector3 start, ref Vector3 end)
        {
            start.x *= dist.x; start.z *= dist.y;
            end.x *= dist.x; end.z *= dist.y;
            GameObject obj = Instantiate(Resources.Load("Prefabs/WorldMap/Line") as GameObject, lines);
            LineRenderer line = obj.GetComponent<LineRenderer>();
            Vector3[] lineList = { start + orgPos, end + orgPos };
            line.positionCount = lineList.Length;
            line.SetPositions(lineList);
        }
    }
}

