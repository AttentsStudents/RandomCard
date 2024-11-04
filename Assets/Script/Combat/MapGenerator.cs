using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node
{
    public int Floor;  // 현재 층
    public int Index;  // 노드의 인덱스
    public List<Node> Connections;  // 연결된 다음 층 노드들

    public Node(int floor, int index)
    {
        Floor = floor;
        Index = index;
        Connections = new List<Node>();
    }
}

public class MapGenerator : MonoBehaviour
{
    public int Floors = 15;
    public int MaxNodesPerFloor = 4;  // 각 층에서 최대 노드 수
    public GameObject LinePrefab;  // 라인 표시 프리팹
    public GameObject Event;
    public Color PathColor = Color.blue;

    private List<List<Node>> map;  // 층별 노드들

    void Start()
    {
        GenerateMap();
        VisualizePath();
    }

    void GenerateMap()
    {
        map = new List<List<Node>>();

        // 층별 노드 생성
        for (int i = 0; i < Floors; i++)
        {
            int a = Random.Range(2, 6);
            int StartNodesCount = a;  // 시작 지점 갯수
            List<Node> floorNodes = new List<Node>();
            int nodeCount = (i == 0) ? Mathf.Clamp(StartNodesCount, 2, 6) : Random.Range(1, 5);  // 시작층과 이후 층의 노드 수 제한
            for (int j = 0; j < nodeCount; j++)
                floorNodes.Add(new Node(i, j));
            map.Add(floorNodes);
        }

        // 경로 연결
        for (int i = 0; i < Floors - 1; i++)
        {
            List<Node> currentFloor = map[i];
            List<Node> nextFloor = map[i + 1];

            foreach (Node node in currentFloor)
            {
                // 다음 층의 노드 중 하나와 연결
                if (((currentFloor.Count< nextFloor.Count) || (currentFloor.Count > nextFloor.Count-1)
                    )||((currentFloor.Count <= nextFloor.Count-1)||(currentFloor.Count >= nextFloor.Count + 1))) {
                    int randomIndex = Random.Range(0, nextFloor.Count);
                    node.Connections.Add(nextFloor[randomIndex]);
                }
            }

            if (i < Floors - 2)  // 마지막 층 바로 아래층 제외
            {
                foreach (Node nextNode in nextFloor)
                {
                    if (nextNode.Connections.Count == 0)  // 연결되지 않은 노드가 있다면
                    {
                        int randomIndex = Random.Range(0, currentFloor.Count);
                        currentFloor[randomIndex].Connections.Add(nextNode);
                    }
                }
            }
        }


        Node finalNode = new Node(Floors -1, 0);
        map[map.Count -1].Clear();  // 15층에 노드를 하나만 두도록 설정
        map[map.Count - 1 ].Add(finalNode);

        foreach (Node node in map[Floors - 2])  // 14층의 모든 노드가 마지막 노드로 연결되도록 설정
        {
            node.Connections.Add(finalNode);
        }
    }

    private void VisualizePath()
    {
        foreach (List<Node> floor in map)
        {
            foreach (Node node in floor)
            {
                Vector3 startPos = GetPosition(node.Floor, node.Index);  // 노드의 위치 계산
                GameObject nodeObject = Instantiate(LinePrefab, startPos, Quaternion.identity);  // 노드 오브젝트 생성
                //GameObject nodestage = Instantiate(NodePrefab, startPos, Quaternion.identity);
                nodeObject.name = $"Node_{node.Floor}_{node.Index}";

                foreach (Node connectedNode in node.Connections)
                {
                    Vector3 endPos = GetPosition(connectedNode.Floor, connectedNode.Index);
                    DrawLine(startPos, endPos);  // 노드 간 선 그리기
                }
            }
        }
    }

    private void DrawCircle(Vector3 here)
    {
        GameObject circle = new GameObject("Circle");
        MeshRenderer meshRenderer= circle.GetComponent<MeshRenderer>();
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("Line");
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.startColor = PathColor;
        lineRenderer.endColor = PathColor;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    private Vector3 GetPosition(int floor, int index)
    {
        // 각 층의 높이를 2로 설정하여 4x15 그리드 내에 위치하도록 조정
        float x = index *5;
        float y = floor *3;
        return new Vector3(x, y, 0);  // 그리드 내 위치 배치
    }
}
