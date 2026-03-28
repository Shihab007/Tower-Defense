using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int columns = 5;
    public int rows = 4;
    public float cellSize = 1.2f;
    public GameObject cellPrefab;

    private GameObject[,] cells;
    private bool[,] occupied;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        cells = new GameObject[columns, rows];
        occupied = new bool[columns, rows];

        Vector3 startPos = transform.position;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                Vector3 pos = startPos + new Vector3(x * cellSize, y * cellSize, 0);
                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity);
                cell.name = $"Cell_{x}_{y}";
                cell.transform.parent = transform;
                cells[x, y] = cell;
            }
        }
    }

    public bool IsCellOccupied(int x, int y)
    {
        return occupied[x, y];
    }

    public void SetOccupied(int x, int y, bool state)
    {
        occupied[x, y] = state;
    }

    public Vector3 GetCellPosition(int x, int y)
    {
        return cells[x, y].transform.position;
    }
}