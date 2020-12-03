using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwipePhase
{ 
    Nothing, 
    Began,
    Moved,
    Ended
}

public class Swipe : MonoBehaviour
{
    static SwipePhase swipePhase = SwipePhase.Nothing;
    static List<Vector2> points = new List<Vector2>();
    
    public static SwipePhase SwipePhase { get { return swipePhase; } }
    public static List<Vector2> Points { get { return new List<Vector2>(points); } }

    public static float Distance { get { return points.Count >= 2 ? Vector2.Distance(points[0], points[points.Count-1]) : 0 ; }}
    public static float DistanceLastFrame { get { return points.Count >= 2 ? Vector2.Distance(points[points.Count - 2], points[points.Count - 1]) : 0; } }

    public static Vector2 Vector { get { return points.Count >= 2 ? points[points.Count - 1] - points[0] : Vector2.zero; } }

    public static Vector2 VectorLastFrame { get { return points.Count >= 2 ? points[points.Count - 1] - points[points.Count - 2] : Vector2.zero; } }

    public static bool IsVertical {
        get 
        {
            if (points.Count < 2) return false;
            
            Vector2 dir = points[points.Count - 1] - points[0];
            if (Mathf.Abs(dir.y) - Mathf.Abs(dir.x) > 0) return true;
            else return false;
        }
    }

    public static bool IsHorizontal
    {
        get
        {
            if (points.Count < 2) return false;

            Vector2 dir = points[points.Count - 1] - points[0];
            if (Mathf.Abs(dir.x) - Mathf.Abs(dir.y) >= 0) return true;
            else return false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 pos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    points.Clear();
                    swipePhase = SwipePhase.Began;
                    points.Add(pos);
                    break;
                case TouchPhase.Moved:
                    swipePhase = SwipePhase.Moved;
                    points.Add(pos);
                    break;
                case TouchPhase.Ended:
                    points.Add(pos);
                    swipePhase = SwipePhase.Ended;
                    break;
            }
        }
        else
        {
            swipePhase = SwipePhase.Nothing;
        }
    }
}
