using EpPathFinding.cs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public enum LevelStatus
{ 
    inGame,
    Win,
    Loose
}

public class Level : MonoBehaviour
{
    [SerializeField]
    List<GameObject> prefabListUnits = new List<GameObject>();
    [SerializeField]
    GameObject prefabGroundObstacle;
    [SerializeField]
    GameObject prefabAirObstacle;

    public Vector2Int size = new Vector2Int(80,80); // размер уровня. в зависимости от него всё будет генериться
    public StaticGrid groundGrid; // сетка поиска пути для наземных юнитов
    public StaticGrid airGrid; // сетка поиска пути для воздушных юнитов
    public int unitsNumber = 0; // кол-во юнитов за каждую из сторон
    public LevelStatus levelStatus = LevelStatus.inGame;

    List<GameObject> unitObjects = new List<GameObject>(); // список юнитов
    List<GameObject> obstacleObjects = new List<GameObject>(); // список препятствий

    public Dictionary<int, List<Unit>> units = new Dictionary<int, List<Unit>>(); // первая переменная - номер команды. вторая - список юнитов в команде
    public Dictionary<string, UnitStats> unitStats = new Dictionary<string, UnitStats>(); // словарь содержащий статы юнитов по имени юнита

    // Start is called before the first frame update
    void Start()
    {
        LoadUnitStats();
        Restart();
    }

    private void Update()
    {
        if (levelStatus == LevelStatus.inGame) UpdateWinLoose();

    }

    void UpdateWinLoose()
    {
        if (GetUnitsCount(1) == 0)
        {
            levelStatus = LevelStatus.Win;
            AudioManager.Instance.PlayWin();
            MessageUI.RecieveMessage("Вы победили!");
        }
        if (GetUnitsCount(0) == 0)
        {
            levelStatus = LevelStatus.Loose;
            AudioManager.Instance.PlayLoose();
            MessageUI.RecieveMessage("Вы проиграли :(");
        }
    }

    /// <summary>
    /// Метод подгружает статы юнитов из Json в словарь 
    /// </summary>
    void LoadUnitStats()
    {
        string filePath = "SetupData/UnitStats";
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        StatsContainer statsCont = JsonUtility.FromJson<StatsContainer>(targetFile.text);
        for (int i = 0; i < statsCont.list.Count; i++)
        {
            unitStats.Add(statsCont.list[i].unitName, statsCont.list[i]);
        }
    }

    /// <summary>
    /// Создаёт карты путей для юнитов
    /// </summary>
    void GeneratePathMaps()
    {
        bool[][] groundMatrix = new bool[size.x][];
        bool[][] airMatrix = new bool[size.x][];

        for (int i = 0; i < size.x; i++)
        {
            groundMatrix[i] = new bool[size.y];
            airMatrix[i] = new bool[size.y];

            for (int j = 0; j < size.y; j++)
            {
                float value = Random.Range(0, 1f);


                if (value <= 0.90f)
                    groundMatrix[i][j] = true;
                else
                    groundMatrix[i][j] = false;

                if (value <= 0.95f)
                    airMatrix[i][j] = true;
                else
                    airMatrix[i][j] = false;
            }
        }

        groundGrid = new StaticGrid(size.x, size.y, groundMatrix);
        airGrid = new StaticGrid(size.x, size.y, airMatrix);
    }

    /// <summary>
    /// Создает препятствия в зависимости от карт путей
    /// </summary>
    void GenerateObstacles()
    {
        for (int i = 0; i < obstacleObjects.Count; i++)
        {
            Destroy(obstacleObjects[i]);
        }
        obstacleObjects.Clear();

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (groundGrid.IsWalkableAt(i, j) == false && airGrid.IsWalkableAt(i, j) == true)
                {
                    var newObst = Instantiate(prefabGroundObstacle, new Vector2(i, j), Quaternion.identity);
                    obstacleObjects.Add(newObst);
                    newObst.transform.SetParent(this.transform);
                }


                if (airGrid.IsWalkableAt(i, j) == false)
                {
                    var newObst = Instantiate(prefabAirObstacle, new Vector2(i, j), Quaternion.identity);
                    obstacleObjects.Add(newObst);
                    newObst.transform.SetParent(this.transform);
                }
                    
            }
        }
    }

    /// <summary>
    /// Создаёт юнитов
    /// </summary>
    void GenerateUnits()
    {
        if (prefabListUnits.Count == 0) return;

        for (int i = 0; i < unitsNumber; i++)
        {
            Vector2 pos = new Vector2(Random.Range(0, 10), Random.Range(0, size.y - 1));
            CreateUnit(0, pos);
        }

        for (int i = 0; i < unitsNumber; i++)
        {
            Vector2 pos = new Vector2(Random.Range(size.x - 11, size.x - 1), Random.Range(0, size.y - 1));
            CreateUnit(1, pos);
        }
    }

    /// <summary>
    /// Создает одного случайного юнита для выбранной команды
    /// </summary>
    /// <param name="team"></param>
    /// <param name="position"></param>
    void CreateUnit(int team, Vector2 position)
    {
        int unitType = Random.Range(0, prefabListUnits.Count);
        GameObject newUnitObj = Instantiate(prefabListUnits[unitType], position, Quaternion.identity);
        Unit unit = newUnitObj.GetComponent<Unit>();
        unit.team = team;
        unit.transform.SetParent(this.transform);
        if (team == 0) unit.color = Color.green;
        if (team == 1) unit.color = Color.red;
        if (team == 2) unit.color = Color.blue;
        InitUnit(unit);
    }

    /// <summary>
    /// Инициализирует созданного юнита и добавляет его в словарь и список юнитов
    /// </summary>
    /// <param name="unit"></param>
    void InitUnit(Unit unit)
    {
        unit.Init(this);
        unit.died += RemoveUnit;

        if (!units.ContainsKey(unit.team))
            units.Add(unit.team, new List<Unit>());

        unitObjects.Add(unit.gameObject);
        units[unit.team].Add(unit);
    }

    /// <summary>
    /// Удаляет юнита из словаря и списка
    /// </summary>
    /// <param name="unit"></param>
    void RemoveUnit(Unit unit)
    {
        unitObjects.Remove(unit.gameObject);
        units[unit.team].Remove(unit);
    }

    /// <summary>
    /// Перезапускает уровень
    /// </summary>
    public void Restart()
    {
        levelStatus = LevelStatus.inGame;
        MessageUI.RecieveMessage("Бой начался!");

        GeneratePathMaps();
        
        for (int i = 0; i < obstacleObjects.Count; i++)
        {
            Destroy(obstacleObjects[i]);
        }
        obstacleObjects.Clear();

        GenerateObstacles();

        for (int i = 0; i < unitObjects.Count; i++)
        {
            Destroy(unitObjects[i]);
        }
        unitObjects.Clear();
        units.Clear();

        GenerateUnits();
    }

    /// <summary>
    /// Возвращает нужные статы из словаря по названию юнита
    /// </summary>
    /// <param name="unitName"></param>
    /// <returns></returns>
    public UnitStats GetStats(string unitName)
    {
        UnitStats result = new UnitStats();
        if (unitStats.ContainsKey(unitName)) result = unitStats[unitName];
        return result;
    }

    public int GetUnitsCount(int team)
    {
        if (units.ContainsKey(team)) return units[team].Count;
        else return 0;
    }
}

