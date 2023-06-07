using System.Collections.Generic;
using UnityEngine;

public class lineTracer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<GameObject> gameObjects = new List<GameObject>();

    private int indexLine;
    private bool circle = false;

    private List<int> line1, line2, line3, line4, line5, line6, line7, line8;
    private List<List<int>> lines;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        line1 = new List<int> { 11, 12 };
        line2 = new List<int> { 23, 24 };
        line3 = new List<int> { 21, 15, 17, 19, 15, 13, 11, 23, 25, 27, 29, 31, 27 };
        line4 = new List<int> { 22, 16, 18, 20, 16, 14, 12, 24, 26, 28, 30, 32, 28 };
        line5 = new List<int> { 9, 10 };
        line6 = new List<int> { 1, 2, 3 };
        line7 = new List<int> { 4, 5, 6 };
        line8 = new List<int> { 7, 2, 0, 5, 8 };

        lines = new List<List<int>> { line1, line2, line3, line4, line5, line6, line7, line8 };
    }

    // Update is called once per frame
    void Update()
    {
        if (!circle)
        {
            lineRenderer.positionCount = lines[indexLine].Count;
            for (int i = 0; i < lines[indexLine].Count; i++)
            {
                lineRenderer.SetPosition(i, gameObjects[lines[indexLine][i]].transform.position);
            }
        }
        else
        {
            DrawCircle(100,0.1f);
        }
    }

    void DrawCircle(int steps, float radius)
    {
        lineRenderer.positionCount = steps;

        for(int currentStep = 0; currentStep < steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;
            
            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled*radius;
            float y = yScaled*radius;
            
            Vector3 currentPosition = new Vector3(gameObjects[0].transform.position.x + x, gameObjects[0].transform.position.y + y, gameObjects[0].transform.position.z);

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }

    public void setGameObjects(List<GameObject> gameObjects)
    {
        this.gameObjects = gameObjects;
    }

    public void setCircle(bool circle)
    {
        this.circle = circle;
    }

    public void setIndexLine(int i)
    {
        this.indexLine = i;
    }
}
