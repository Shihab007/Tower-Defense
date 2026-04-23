using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int columns = 5;
    public int rows = 5;
    public GameObject cellPrefab;

    [Header("Extra spacing")]
    public float gapX = 0.02f;
    public float gapY = 0.02f;

    private GameObject[,] cells;
    private bool[,] occupied;

    private float cellWidth;
    private float cellHeight;

    void Start()
    {
        BuildGrid();
    }

    void BuildGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("GridManager: Cell Prefab is not assigned.");
            return;
        }

        SpriteRenderer sr = cellPrefab.GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
        {
            Debug.LogError("GridManager: Cell Prefab needs a SpriteRenderer with a Sprite.");
            return;
        }

        cells = new GameObject[columns, rows];
        occupied = new bool[columns, rows];

        // Sprite size in world units
        float spriteWidth = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        float spriteHeight = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;

        // Apply prefab scale
        cellWidth = spriteWidth * cellPrefab.transform.localScale.x + gapX;
        cellHeight = spriteHeight * cellPrefab.transform.localScale.y + gapY;

        float totalWidth = (columns - 1) * cellWidth;
        float totalHeight = (rows - 1) * cellHeight;

        Vector3 bottomLeft = transform.position - new Vector3(totalWidth / 2f, totalHeight / 2f, 0f);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                Vector3 pos = bottomLeft + new Vector3(x * cellWidth, y * cellHeight, 0f);

                GameObject cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{y}";

                cells[x, y] = cell;
                occupied[x, y] = false;
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