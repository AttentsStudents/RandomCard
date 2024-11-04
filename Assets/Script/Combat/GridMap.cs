using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class Node
{
    public Vector2Int Position { get; private set; }  // �׸��� ���� ��ġ
    public List<Node> NextNodes { get; private set; }  // ����� ���� ����

    public Node(Vector2Int position)
    {
        Position = position;
        NextNodes = new List<Node>();
    }

    public void AddNextNode(Node nextNode)
    {
        NextNodes.Add(nextNode);
    }
}

public class GridMap : MonoBehaviour
{
    public int rows = 7;   // �׸����� �� (Y��)
    public int cols = 15;  // �׸����� �� (X��)
    private Node[,] grid;  // ������ 2���� �迭
    private List<List<Node>> paths = new List<List<Node>>();  // ������ ��� ����

    public GameObject nodePrefab;  // ��带 ��Ÿ���� UI ������ (�̹��� �Ǵ� ��ư)
    public GameObject linePrefab;  // ���� ��Ÿ���� UI ������ (Image + RectTransform)
    public Transform gridParent;   // �׸��带 ���� �θ� ������Ʈ (Canvas ��)

    public Color[] pathColors;     // ��θ� �����ϱ� ���� ���� �迭

    private void Start()
    {
        GenerateGridMap();
        GeneratePaths(6);  // �� 6���� ��� ����
        DisplayGrid();
        DisplayPaths();
    }

    // �׸��� �� ����
    private void GenerateGridMap()
    {
        grid = new Node[rows, cols];

        // �׸����� �� ��ġ�� ��� ����
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                grid[y, x] = new Node(new Vector2Int(x, y));
            }
        }

        Debug.Log("�׸��� �� ���� �Ϸ�");
    }

    // ��θ� �����ϴ� �޼ҵ�
    private void GeneratePaths(int pathCount)
    {
        HashSet<int> startingNodes = new HashSet<int>();  // ù ��° ������ ������ ���� ������ �ε����� ����

        for (int i = 0; i < pathCount; i++)
        {
            int startX;

            // 2�� �̻��� �������� ������ ���� ó�� 2���� ��δ� ���� �ٸ� ��忡�� ����
            if (i < 2)
            {
                do
                {
                    startX = Random.Range(0, rows);  // 7���� �� �� �ϳ� ����
                } while (startingNodes.Contains(startX));  // �̹� ���õ� ���������� �ƴ� ������ �ݺ�
                startingNodes.Add(startX);
            }
            else
            {
                startX = Random.Range(0, rows);  // ���� ��δ� ������ ��ġ���� ���� ����
            }

            // ��� ����: 15������ ���� �ö󰡸� ��带 ����
            List<Node> path = CreatePathFromBottomToTop(startX);
            paths.Add(path);  // ������ ��θ� ����
        }
    }

    // �Ʒ������� �������� ��θ� ����� �޼ҵ�
    private List<Node> CreatePathFromBottomToTop(int startX)
    {
        List<Node> path = new List<Node>();
        int currentX = startX;
        Node currentNode = grid[currentX, cols - 1];  // 15��(������ ��)���� ����
        path.Add(currentNode);

        for (int y = cols - 1; y > 0; y--)
        {
            List<Node> possibleNextNodes = new List<Node>();

            if (currentX > 0 && y - 1 >= 0) possibleNextNodes.Add(grid[currentX - 1, y - 1]);
            if (y - 1 >= 0) possibleNextNodes.Add(grid[currentX, y - 1]);
            if (currentX < rows - 1 && y - 1 >= 0) possibleNextNodes.Add(grid[currentX + 1, y - 1]);
            if (possibleNextNodes.Count == 0)
                break;
            Node nextNode = possibleNextNodes[Random.Range(0, possibleNextNodes.Count)];
            currentNode.AddNextNode(nextNode);
            currentNode = nextNode;
            currentX = currentNode.Position.x;

            path.Add(currentNode);
        }

        return path;
    }
    // �׸��带 UI�� ǥ��
    private void DisplayGrid()
    {
        Vector2 gridOffset = new Vector2(-rows / 2f * 100, -cols / 2f * 100);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // Node�� null���� Ȯ��
                Node node = grid[y, x];
                if (node == null)
                {
                    Debug.LogWarning($"Node at grid[{y},{x}] is null");
                    continue;  // null�̸� �ǳʶڴ�
                }

                // �׸��� �󿡼��� ����� ��ġ ���
                Vector2 nodePos = new Vector2(node.Position.x * 300, node.Position.y * 300) + gridOffset;

                // ��� UI ����
                GameObject nodeUI = Instantiate(nodePrefab, gridParent);  // ��� UI ����
                RectTransform rt = nodeUI.GetComponent<RectTransform>();
                rt.anchoredPosition = nodePos;  // ��带 ĵ���� ���� ��ġ�� ��ġ
            }
        }
    }


    // ��θ� UI�� ǥ��
    private void DisplayPaths()
    {
        Vector2 gridOffset = new Vector2(-rows / 2f * 100, -cols / 2f * 100);

        for (int i = 0; i < paths.Count; i++)
        {
            List<Node> path = paths[i];
            if (path == null || path.Count == 0) continue;

            Color pathColor = pathColors[i % pathColors.Length];  // �� ��ο� ���� �Ҵ�

            for (int j = 0; j < path.Count - 1; j++)
            {
                Node currentNode = path[j];
                Node nextNode = path[j + 1];

                Vector2 currentPos = new Vector2(currentNode.Position.x * 100, currentNode.Position.y * 100) + gridOffset;
                Vector2 nextPos = new Vector2(nextNode.Position.x * 100, nextNode.Position.y * 100) + gridOffset;

                // ���� �׸� UI �̹��� ����
                GameObject lineUI = Instantiate(linePrefab, gridParent);
                Image lineImage = lineUI.GetComponent<Image>();
                RectTransform rt = lineUI.GetComponent<RectTransform>();

                // ���� ��ġ �� ũ�� ����
                rt.anchoredPosition = (currentPos + nextPos) / 2;  // �� ����� �߰��� ��ġ
                rt.sizeDelta = new Vector2(Vector2.Distance(currentPos, nextPos), 5);  // ���� ����
                rt.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(nextPos.y - currentPos.y, nextPos.x - currentPos.x) * Mathf.Rad2Deg);  // ���� ����

                lineImage.color = pathColor;  // �� ���� ����
            }
        }
    }
}
*/