using UnityEngine;
using System.Collections.Generic;

public class SummonManager : MonoBehaviour
{
    public UnitData[] availableUnits;
    public UnitData[] allUnitTiers;
    public GridManager gridManager;

    public int currentMana = 5;
    public int maxMana = 10;
    public float manaRegenRate = 1f;

    private float manaTimer = 0f;

    void Update()
    {
        RegenerateMana();
    }

    void RegenerateMana()
    {
        manaTimer += Time.deltaTime;
        if (manaTimer >= manaRegenRate)
        {
            manaTimer = 0f;
            if (currentMana < maxMana)
                currentMana++;
        }
    }

    public void Summon()
    {
        Vector2Int cell = GetRandomEmptyCell();
        if (cell.x == -1) 
        {
            Debug.Log("No empty cells");
            return;
        }

        UnitData unitData = availableUnits[Random.Range(0, availableUnits.Length)];

        if (currentMana < unitData.summonCost)
        {
            Debug.Log("Not enough mana");
            return;
        }

        Vector3 spawnPos = gridManager.GetCellPosition(cell.x, cell.y);
        GameObject obj = Instantiate(unitData.prefab, spawnPos, Quaternion.identity);
        UnitBase unit = obj.GetComponent<UnitBase>();
        unit.data = unitData;

        gridManager.SetOccupied(cell.x, cell.y, true);
        currentMana -= unitData.summonCost;

        Debug.Log($"Summoned {unitData.unitName} at cell {cell.x},{cell.y}");
    }

    Vector2Int GetRandomEmptyCell()
    {
        List<Vector2Int> emptyCells = new List<Vector2Int>();

        for (int x = 0; x < gridManager.columns; x++)
        {
            for (int y = 0; y < gridManager.rows; y++)
            {
                if (!gridManager.IsCellOccupied(x, y))
                    emptyCells.Add(new Vector2Int(x, y));
            }
        }

        if (emptyCells.Count == 0) return new Vector2Int(-1, -1);
        return emptyCells[Random.Range(0, emptyCells.Count)];
    }
}