using EpPathFinding.cs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [SerializeField]
    List<GameObject> prefabListUnits = new List<GameObject>();
    [SerializeField]
    GameObject prefabGroundObstacle;
    [SerializeField]
    GameObject prefabAirObstacle;

    [SerializeField]
    Text teamText1;
    [SerializeField]
    Text teamText2;

    public Vector2Int size = new Vector2Int(80,80);
    public StaticGrid groundGrid;
    public StaticGrid airGrid;
    public int unitsNumber = 0;

    List<GameObject> unitObjects = new List<GameObject>();
    List<GameObject> obstacleObjects = new List<GameObject>();

    public Dictionary<int, List<Unit>> units = new Dictionary<int, List<Unit>>(); // первая переменная - номер команды. вторая - список юнитов в команде
    public Dictionary<string, UnitStats> unitStats = new Dictionary<string, UnitStats>();

    // Start is called before the first frame update
    void Start()
    {
        LoadUnitStats();
        GeneratePathMaps();
        GenerateObstacles();
        GenerateUnits();
    }

    private void Update()
    {
        if (teamText1 != null) 
            if (units.ContainsKey(0)) teamText1.text = units[0].Count.ToString();
            else teamText1.text = "0";

        if (teamText2 != null)
            if (units.ContainsKey(1)) teamText2.text = units[1].Count.ToString();
            else teamText2.text = "0";
    }

    void LoadUnitStats()
    {
        string filePath = "SetupData/UnitStats";
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        Container statsCont = JsonUtility.FromJson<Container>(targetFile.text);
        for (int i = 0; i < statsCont.list.Count; i++)
        {
            unitStats.Add(statsCont.list[i].unitName, statsCont.list[i]);
        }
        /*
        string unitStatsStr = File.ReadAllText(Application.dataPath + "/UnitStats.json");
        Container statsCont = JsonUtility.FromJson<Container>(unitStatsStr);
        for (int i = 0; i < statsCont.list.Count; i++)
        {
            unitStats.Add(statsCont.list[i].unitName, statsCont.list[i]);
        }*/
    }

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

    public void GenerateObstacles()
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

        /*
        List<UnitStats> stats = new List<UnitStats>();
        stats.Add(unitObjects[0].GetComponent<Unit>().stats);
        stats.Add(unitObjects[0].GetComponent<Unit>().stats);
        Container container = new Container(stats);
        string JsonString = JsonUtility.ToJson(container, true);
        print(JsonString);
        File.WriteAllText(Application.dataPath + "/UnitStats.json", JsonString);
        print(Application.dataPath + "/UnitStats.json");*/
    }

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

    void InitUnit(Unit unit)
    {
        unit.Init(this);
        unit.died += RemoveUnit;

        if (!units.ContainsKey(unit.team))
            units.Add(unit.team, new List<Unit>());

        unitObjects.Add(unit.gameObject);
        units[unit.team].Add(unit);
    }

    void RemoveUnit(Unit unit)
    {
        unitObjects.Remove(unit.gameObject);
        units[unit.team].Remove(unit);
    }

    public void Recreate()
    {
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

    public UnitStats GetStats(string unitName)
    {
        UnitStats result = new UnitStats();
        if (unitStats.ContainsKey(unitName)) result = unitStats[unitName];
        return result;
    }
}

public struct Container
{
    [SerializeField]
    public List<UnitStats> list;

    public Container(List<UnitStats> list)
    {
        this.list = list;
    }
}