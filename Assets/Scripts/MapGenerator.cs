using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Node
{
    public int Floor;  // ���� ��
    public int Index;  // ����� �ε���
    public List<Node> Connections;  // ����� ���� �� ����

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
    public int MaxNodesPerFloor = 4;  // �� ������ �ִ� ��� ��
    public GameObject LinePrefab;  // ���� ǥ�� ������
    public GameObject Event;
    public Color PathColor = Color.blue;

    private List<List<Node>> map;  // ���� ����

    void Start()
    {
        GenerateMap();
        VisualizePath();
    }

    void GenerateMap()
    {
        map = new List<List<Node>>();

        // ���� ��� ����
        for (int i = 0; i < Floors; i++)
        {
            int a = Random.Range(2, 6);
            int StartNodesCount = a;  // ���� ���� ����
            List<Node> floorNodes = new List<Node>();
            int nodeCount = (i == 0) ? Mathf.Clamp(StartNodesCount, 2, 6) : Random.Range(1, 5);  // �������� ���� ���� ��� �� ����
            for (int j = 0; j < nodeCount; j++)
                floorNodes.Add(new Node(i, j));
            map.Add(floorNodes);
        }

        // ��� ����
        for (int i = 0; i < Floors - 1; i++)
        {
            List<Node> currentFloor = map[i];
            List<Node> nextFloor = map[i + 1];

            foreach (Node node in currentFloor)
            {
                // ���� ���� ��� �� �ϳ��� ����
                if (((currentFloor.Count< nextFloor.Count) || (currentFloor.Count > nextFloor.Count-1)
                    )||((currentFloor.Count <= nextFloor.Count-1)||(currentFloor.Count >= nextFloor.Count + 1))) {
                    int randomIndex = Random.Range(0, nextFloor.Count);
                    node.Connections.Add(nextFloor[randomIndex]);
                }
            }

            if (i < Floors - 2)  // ������ �� �ٷ� �Ʒ��� ����
            {
                foreach (Node nextNode in nextFloor)
                {
                    if (nextNode.Connections.Count == 0)  // ������� ���� ��尡 �ִٸ�
                    {
                        int randomIndex = Random.Range(0, currentFloor.Count);
                        currentFloor[randomIndex].Connections.Add(nextNode);
                    }
                }
            }
        }


        Node finalNode = new Node(Floors -1, 0);
        map[map.Count -1].Clear();  // 15���� ��带 �ϳ��� �ε��� ����
        map[map.Count - 1 ].Add(finalNode);

        foreach (Node node in map[Floors - 2])  // 14���� ��� ��尡 ������ ���� ����ǵ��� ����
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
                Vector3 startPos = GetPosition(node.Floor, node.Index);  // ����� ��ġ ���
                GameObject nodeObject = Instantiate(LinePrefab, startPos, Quaternion.identity);  // ��� ������Ʈ ����
                //GameObject nodestage = Instantiate(NodePrefab, startPos, Quaternion.identity);
                nodeObject.name = $"Node_{node.Floor}_{node.Index}";

                foreach (Node connectedNode in node.Connections)
                {
                    Vector3 endPos = GetPosition(connectedNode.Floor, connectedNode.Index);
                    DrawLine(startPos, endPos);  // ��� �� �� �׸���
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
        // �� ���� ���̸� 2�� �����Ͽ� 4x15 �׸��� ���� ��ġ�ϵ��� ����
        float x = index *5;
        float y = floor *3;
        return new Vector3(x, y, 0);  // �׸��� �� ��ġ ��ġ
    }
}
