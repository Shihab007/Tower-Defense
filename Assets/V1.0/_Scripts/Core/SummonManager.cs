using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class SummonManager : MonoBehaviour
{
    public UnitData[] availableUnits;
    public GridManager gridManager;

    public int currentMana = 5;
    public int maxMana = 20;
    public float manaRegenRate = 1f;

    private float manaTimer = 0f;
    public TextMeshProUGUI manaText;

    void Start()
    {
        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        if (gridManager == null)
        {
            Debug.LogError("SummonManager: GridManager not found.");
            return;
        }

        UpdateManaUI();
    }

    void Update()
    {
        RegenerateMana();
    }

    void UpdateManaUI()
    {
        if (manaText != null)
            manaText.text = $"Mana: {currentMana}/{maxMana}";
    }

    public void AddMana(int amount)
    {
        currentMana = Mathf.Min(currentMana + amount, maxMana);
        UpdateManaUI();
    }

    void RegenerateMana()
    {
        manaTimer += Time.deltaTime;
        if (manaTimer >= manaRegenRate)
        {
            manaTimer = 0f;
            if (currentMana < maxMana)
            {
                currentMana++;
                UpdateManaUI();
            }
        }
    }

    public void Summon()
    {
        if (gridManager == null)
        {
            Debug.LogError("SummonManager: GridManager is missing.");
            return;
        }

        if (availableUnits == null || availableUnits.Length == 0)
        {
            Debug.LogError("SummonManager: No available units assigned.");
            return;
        }

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
        unit.Initialize(unitData, cell);

        gridManager.SetOccupied(cell.x, cell.y, true);
        currentMana -= unitData.summonCost;
        UpdateManaUI();
    }

    Vector2Int GetRandomEmptyCell()
    {
        if (gridManager == null)
            return new Vector2Int(-1, -1);

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