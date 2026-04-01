using UnityEngine;

public class MergeSystem : MonoBehaviour
{
    private Vector3 originPosition;
    private Vector2Int originCell;
    private bool isDragging = false;

    private GridManager gridManager;
    private SummonManager summonManager;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        summonManager = FindFirstObjectByType<SummonManager>();
    }
    void OnMouseDown()
    {
        originPosition = transform.position;
        originCell = GetCellAtPosition(originPosition);
        isDragging = true;
        GetComponent<Collider2D>().enabled = false;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    void OnMouseUp()
{
    isDragging = false;

    Vector3 mousePos = Camera.main.ScreenToWorldPoint(
        new Vector3(Input.mousePosition.x, Input.mousePosition.y,
        Mathf.Abs(Camera.main.transform.position.z)));
    mousePos.z = 0;

    Collider2D hit = Physics2D.OverlapCircle(mousePos, 0.5f);

    // Re-enable AFTER the check
    GetComponent<Collider2D>().enabled = true;

    Debug.Log($"Released at: {mousePos}, Hit: {(hit != null ? hit.name : "null")}");

    if (hit != null && hit.gameObject != gameObject)
    {
        UnitBase otherUnit = hit.GetComponent<UnitBase>();
        UnitBase thisUnit = GetComponent<UnitBase>();

        Debug.Log($"Other unit: {otherUnit?.data?.unitName}, tier: {otherUnit?.data?.tier}");
        Debug.Log($"This unit: {thisUnit?.data?.unitName}, tier: {thisUnit?.data?.tier}");

        if (otherUnit != null
            && otherUnit.data.unitName == thisUnit.data.unitName
            && otherUnit.data.tier == thisUnit.data.tier
            && thisUnit.data.tier < 3)
        {
            MergeWith(otherUnit, GetCellAtPosition(hit.transform.position));
            return;
        }
    }

    transform.position = originPosition;
}
    void MergeWith(UnitBase other, Vector2Int targetCell)
{
    Debug.Log($"MergeWith called. TargetCell: {targetCell}");

    UnitBase thisUnit = GetComponent<UnitBase>();
    UnitData nextTier = GetNextTier(thisUnit.data);

    Debug.Log($"NextTier found: {(nextTier != null ? nextTier.unitName : "null")}");

    if (nextTier == null)
    {
        transform.position = originPosition;
        return;
    }

    if (targetCell.x == -1)
    {
        Debug.Log("Target cell not found");
        transform.position = originPosition;
        return;
    }

    gridManager.SetOccupied(originCell.x, originCell.y, false);
    gridManager.SetOccupied(targetCell.x, targetCell.y, false);

    Vector3 spawnPos = gridManager.GetCellPosition(targetCell.x, targetCell.y);

    Destroy(other.gameObject);
    Destroy(gameObject);

    GameObject obj = Instantiate(nextTier.prefab, spawnPos, Quaternion.identity);
    UnitBase newUnit = obj.GetComponent<UnitBase>();
    newUnit.data = nextTier;

    gridManager.SetOccupied(targetCell.x, targetCell.y, true);

    Debug.Log($"Merged into {nextTier.unitName} tier {nextTier.tier}");
}

    UnitData GetNextTier(UnitData current)
{
    foreach (UnitData ud in summonManager.allUnitTiers)
    {
        if (ud.unitName == current.unitName && ud.tier == current.tier + 1)
            return ud;
    }
    return null;
}

    Vector2Int GetCellAtPosition(Vector3 worldPos)
    {
        for (int x = 0; x < gridManager.columns; x++)
        {
            for (int y = 0; y < gridManager.rows; y++)
            {
                if (Vector3.Distance(gridManager.GetCellPosition(x, y), worldPos) < 0.6f)
                    return new Vector2Int(x, y);
            }
        }
        return new Vector2Int(-1, -1);
    }
}