using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class Node
{
    public Vector2Int Position { get; private set; }  // 그리드 상의 위치
    public List<Node> NextNodes { get; private set; }  // 연결된 다음 노드들

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
    public int rows = 7;   // 그리드의 행 (Y축)
    public int cols = 15;  // 그리드의 열 (X축)
    private Node[,] grid;  // 노드들의 2차원 배열
    private List<List<Node>> paths = new List<List<Node>>();  // 생성된 경로 저장

    public GameObject nodePrefab;  // 노드를 나타내는 UI 프리팹 (이미지 또는 버튼)
    public GameObject linePrefab;  // 선을 나타내는 UI 프리팹 (Image + RectTransform)
    public Transform gridParent;   // 그리드를 담을 부모 오브젝트 (Canvas 내)

    public Color[] pathColors;     // 경로를 구분하기 위한 색상 배열

    private void Start()
    {
        GenerateGridMap();
        GeneratePaths(6);  // 총 6개의 경로 생성
        DisplayGrid();
        DisplayPaths();
    }

    // 그리드 맵 생성
    private void GenerateGridMap()
    {
        grid = new Node[rows, cols];

        // 그리드의 각 위치에 노드 생성
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                grid[y, x] = new Node(new Vector2Int(x, y));
            }
        }

        Debug.Log("그리드 맵 생성 완료");
    }

    // 경로를 생성하는 메소드
    private void GeneratePaths(int pathCount)
    {
        HashSet<int> startingNodes = new HashSet<int>();  // 첫 번째 층에서 선택한 시작 지점의 인덱스를 추적

        for (int i = 0; i < pathCount; i++)
        {
            int startX;

            // 2개 이상의 시작지점 보장을 위해 처음 2개의 경로는 서로 다른 노드에서 시작
            if (i < 2)
            {
                do
                {
                    startX = Random.Range(0, rows);  // 7개의 행 중 하나 선택
                } while (startingNodes.Contains(startX));  // 이미 선택된 시작지점이 아닐 때까지 반복
                startingNodes.Add(startX);
            }
            else
            {
                startX = Random.Range(0, rows);  // 이후 경로는 랜덤한 위치에서 시작 가능
            }

            // 경로 생성: 15층에서 위로 올라가며 노드를 연결
            List<Node> path = CreatePathFromBottomToTop(startX);
            paths.Add(path);  // 생성된 경로를 저장
        }
    }

    // 아래층부터 위층으로 경로를 만드는 메소드
    private List<Node> CreatePathFromBottomToTop(int startX)
    {
        List<Node> path = new List<Node>();
        int currentX = startX;
        Node currentNode = grid[currentX, cols - 1];  // 15층(마지막 층)에서 시작
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
    // 그리드를 UI로 표시
    private void DisplayGrid()
    {
        Vector2 gridOffset = new Vector2(-rows / 2f * 100, -cols / 2f * 100);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                // Node가 null인지 확인
                Node node = grid[y, x];
                if (node == null)
                {
                    Debug.LogWarning($"Node at grid[{y},{x}] is null");
                    continue;  // null이면 건너뛴다
                }

                // 그리드 상에서의 노드의 위치 계산
                Vector2 nodePos = new Vector2(node.Position.x * 300, node.Position.y * 300) + gridOffset;

                // 노드 UI 생성
                GameObject nodeUI = Instantiate(nodePrefab, gridParent);  // 노드 UI 생성
                RectTransform rt = nodeUI.GetComponent<RectTransform>();
                rt.anchoredPosition = nodePos;  // 노드를 캔버스 상의 위치에 배치
            }
        }
    }


    // 경로를 UI로 표시
    private void DisplayPaths()
    {
        Vector2 gridOffset = new Vector2(-rows / 2f * 100, -cols / 2f * 100);

        for (int i = 0; i < paths.Count; i++)
        {
            List<Node> path = paths[i];
            if (path == null || path.Count == 0) continue;

            Color pathColor = pathColors[i % pathColors.Length];  // 각 경로에 색상 할당

            for (int j = 0; j < path.Count - 1; j++)
            {
                Node currentNode = path[j];
                Node nextNode = path[j + 1];

                Vector2 currentPos = new Vector2(currentNode.Position.x * 100, currentNode.Position.y * 100) + gridOffset;
                Vector2 nextPos = new Vector2(nextNode.Position.x * 100, nextNode.Position.y * 100) + gridOffset;

                // 선을 그릴 UI 이미지 생성
                GameObject lineUI = Instantiate(linePrefab, gridParent);
                Image lineImage = lineUI.GetComponent<Image>();
                RectTransform rt = lineUI.GetComponent<RectTransform>();

                // 선의 위치 및 크기 설정
                rt.anchoredPosition = (currentPos + nextPos) / 2;  // 두 노드의 중간에 배치
                rt.sizeDelta = new Vector2(Vector2.Distance(currentPos, nextPos), 5);  // 선의 길이
                rt.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(nextPos.y - currentPos.y, nextPos.x - currentPos.x) * Mathf.Rad2Deg);  // 선의 각도

                lineImage.color = pathColor;  // 선 색상 설정
            }
        }
    }
}
*/