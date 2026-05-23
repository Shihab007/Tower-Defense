using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    private Vector3 originPosition;
    private Vector2Int originCell;
    private bool isDragging = false;

    private GridManager gridManager;
    public GameObject mergePopupPrefab;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
    }

    void OnMouseDown()
    {
        UnitBase unit = GetComponent<UnitBase>();
        originPosition = transform.position;
        originCell = unit != null ? unit.currentCell : GetCellAtPosition(originPosition);
        isDragging = true;
        GetComponent<Collider2D>().enabled = false;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        isDragging = false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(
            new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                Mathf.Abs(Camera.main.transform.position.z)
            )
        );
        mousePos.z = 0f;

        GetComponent<Collider2D>().enabled = true;

        UnitBase thisUnit = GetComponent<UnitBase>();
        if (thisUnit == null || gridManager == null)
        {
            transform.position = originPosition;
            return;
        }

        Vector2Int targetCell = GetCellAtPosition(mousePos);

        // Invalid target
        if (targetCell.x < 0)
        {
            SnapToCell(originCell);
            return;
        }

        // Same tile
        if (targetCell == originCell)
        {
            SnapToCell(originCell);
            return;
        }

        // Empty tile is no longer allowed
        if (!gridManager.IsCellOccupied(targetCell.x, targetCell.y))
        {
            SnapToCell(originCell);
            return;
        }

        // Occupied tile: only allow valid merge
        UnitBase otherUnit = FindUnitAtCell(targetCell);

        if (otherUnit != null
            && otherUnit != thisUnit
            && otherUnit.data.unitName == thisUnit.data.unitName
            && otherUnit.data.tier == thisUnit.data.tier
            && thisUnit.data.nextTierUnit != null)
        {
            MergeWith(otherUnit, targetCell);
            return;
        }

        // Any other case: snap back
        SnapToCell(originCell);
    }

    void SnapToCell(Vector2Int cell)
    {
        if (cell.x < 0) return;
        transform.position = gridManager.GetCellPosition(cell.x, cell.y);
    }

    UnitBase FindUnitAtCell(Vector2Int cell)
    {
        Vector3 center = gridManager.GetCellPosition(cell.x, cell.y);
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, 0.6f);

        foreach (Collider2D c in hits)
        {
            if (c.gameObject == gameObject) continue;

            UnitBase u = c.GetComponent<UnitBase>();
            if (u != null)
                return u;
        }

        return null;
    }

    void ShowMergePopup(Vector3 position, string message)
    {
        if (mergePopupPrefab == null)
        {
            Debug.LogWarning("Merge popup prefab is not assigned.");
            return;
        }

        Vector3 popupPos = position + new Vector3(0f, 1.5f, -2f);
        GameObject popup = Instantiate(mergePopupPrefab, popupPos, Quaternion.identity);

        Debug.Log($"Popup spawned at: {popupPos}");

        FloatingText floatingText = popup.GetComponent<FloatingText>();
        if (floatingText != null)
        {
            floatingText.SetText(message);
        }
    }

    void MergeWith(UnitBase other, Vector2Int targetCell)
    {
        UnitBase thisUnit = GetComponent<UnitBase>();
        UnitData nextTier = thisUnit.data.nextTierUnit;

        if (nextTier == null)
        {
            SnapToCell(originCell);
            return;
        }

        if (targetCell.x == -1)
        {
            SnapToCell(originCell);
            return;
        }

        gridManager.SetOccupied(originCell.x, originCell.y, false);
        gridManager.SetOccupied(targetCell.x, targetCell.y, false);

        Vector3 spawnPos = gridManager.GetCellPosition(targetCell.x, targetCell.y);

        string mergedFromName = thisUnit.data.unitName;
        int mergedFromTier = thisUnit.data.tier;

        Destroy(other.gameObject);
        Destroy(gameObject);

        GameObject obj = Instantiate(nextTier.prefab, spawnPos, Quaternion.identity);
        UnitBase newUnit = obj.GetComponent<UnitBase>();
        newUnit.Initialize(nextTier, targetCell);

        gridManager.SetOccupied(targetCell.x, targetCell.y, true);

        if (MergeFeedbackUI.Instance != null)
        {
            MergeFeedbackUI.Instance.ShowFeedback($"Merge! T{nextTier.tier}");
        }

        // for popup
        // ShowMergePopup(spawnPos, $"Merge! T{nextTier.tier}");

        Debug.Log($"Merge Success: {mergedFromName} Tier {mergedFromTier} + {mergedFromName} Tier {mergedFromTier} -> {nextTier.unitName} Tier {nextTier.tier}");
    }

    Vector2Int GetCellAtPosition(Vector3 worldPos)
    {
        float closestDistance = Mathf.Infinity;
        Vector2Int closestCell = new Vector2Int(-1, -1);

        for (int x = 0; x < gridManager.columns; x++)
        {
            for (int y = 0; y < gridManager.rows; y++)
            {
                float dist = Vector3.Distance(gridManager.GetCellPosition(x, y), worldPos);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestCell = new Vector2Int(x, y);
                }
            }
        }

        if (closestDistance > 1.0f)
            return new Vector2Int(-1, -1);

        return closestCell;
    }
}