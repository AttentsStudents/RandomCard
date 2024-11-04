using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheonJiWoon
{
    public class Node
    {
        public int x
        {
            get; private set;
        }
        public int y
        {
            get; private set;
        }
        public int min
        {
            get; set;
        }
        public int max
        {
            get; set;
        }

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
        int xSize = 7;
        int ySize = 12;
        int startPointCount = 4;
        public Transform lines;

        void Awake()
        {
            Generate();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Generate()
        {
            mapInfo = new Node[ySize, xSize];

            HashSet<int> startPointCheck = new HashSet<int>();
            while (startPointCheck.Count < startPointCount)
            {
                int random = Random.Range(0, xSize);
                if (!startPointCheck.Contains(random))
                {
                    startPointCheck.Add(random);

                    CreatCube(random * 2, 0);
                    Node newNode = new Node(random, 0);
                    mapInfo[0, random] = newNode;
                    CreatePaths(newNode, 3, 1);
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

                    GameObject obj = Instantiate(Resources.Load("Fabs/Line") as GameObject, lines);
                    obj.transform.position = new Vector3(randomX * 2, 0.0f, depth * 2);
                    LineRenderer line = obj.GetComponent<LineRenderer>();
                    Vector3[] lineList = { new Vector3(parent.x * 2, 0.0f, parent.y * 2), new Vector3(randomX * 2, 0.0f, depth * 2) };
                    line.positionCount = lineList.Length;
                    line.SetPositions(lineList);

                    if (mapInfo[depth, randomX] == null)
                    {
                        Node newNode = new Node(randomX, depth);
                        mapInfo[depth, randomX] = newNode;
                        CreatCube(newNode.x * 2, newNode.y * 2);
                        if (ySize > next) CreatePaths(newNode, pathNumbs, range);
                    }

                    parent.nodes.Add(mapInfo[depth, randomX]);

                }
            }
        }

        void CreatCube(int x, int z)
        {
            GameObject obj = Instantiate(Resources.Load("Fabs/Sphere") as GameObject);
            obj.transform.position = new Vector3(x, 0.0f, z);
        }
    }
}

