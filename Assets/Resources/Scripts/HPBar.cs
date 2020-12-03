using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField]
    GameObject prefabNode;


    public Color hpColor = new Color(0.3f, 0.85f, 0.3f, 1);
    public Color lostHPColor = new Color(0.6f, 0.5f, 0.35f, 1);
    public Vector2 nodeSize = new Vector2(0.10f, 0.15f);
    public float nodeDistance = 0.05f;


    Unit unit;
    List<GameObject> nodes = new List<GameObject>();
    List<SpriteRenderer> nodesRenderers = new List<SpriteRenderer>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Хп бар создает нужное число нодов в зависимости от макс хп юнита
    /// </summary>
    /// <param name="owner"></param>
    public void Init(Unit owner)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            Destroy(nodes[i]);
        }
        nodes.Clear();
        nodesRenderers.Clear();

        unit = owner.GetComponent<Unit>();

        int nodesCount = Mathf.RoundToInt(unit.stats.healthMax / 10f);
        nodesCount = nodesCount < 1 ? 1 : nodesCount;

        for (int i = 0; i < nodesCount; i++)
        {
            Vector2 pos = Vector2.zero;
            pos.x += i * nodeSize.x + i * nodeDistance - nodesCount * 0.5f * nodeSize.x - nodesCount * 0.5f * nodeDistance;
            GameObject newNode = Instantiate(prefabNode);
            newNode.transform.SetParent(this.gameObject.transform);
            newNode.transform.localPosition = pos;
            newNode.transform.localScale = nodeSize;
            nodes.Add(newNode);
            nodesRenderers.Add(newNode.GetComponent<SpriteRenderer>());
        }
    }



    // Update is called once per frame
    void Update()
    {
        UpdateVisibility();
        UpdateColor();
    }

    void UpdateVisibility()
    {
        if (unit.stats.healthCurrent == unit.stats.healthMax)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].SetActive(true);
            }
        }
            
    }

    void UpdateColor()
    {
        float healthPercent = unit.stats.healthCurrent / unit.stats.healthMax;

        for (int i = 0; i < nodes.Count; i++)
        {
            if ((float)i / nodes.Count < healthPercent)
                nodesRenderers[i].color = hpColor;
            else
                nodesRenderers[i].color = lostHPColor;
        }
    }
}
