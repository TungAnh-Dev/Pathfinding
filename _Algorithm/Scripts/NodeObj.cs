using UnityEngine;

public class NodeObj : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color obstacleColor = Color.black;
    [SerializeField] private Color pathColor = Color.green;
    [SerializeField] private Color startColor = Color.cyan;
    [SerializeField] private Color endColor = Color.magenta;
    [SerializeField] private Color processingColor = Color.yellow;
    [SerializeField] private Color frontierColor = new Color(1, 0.5f, 0);
    [SerializeField] private Color visitedColor = new Color(0.8f, 0.8f, 0.8f);

    [SerializeField] private MeshRenderer meshRenderer;

    public void ResetNode(bool isWalkable)
    {
        if (meshRenderer)
        {
            meshRenderer.material.color = isWalkable ? defaultColor : obstacleColor;
        }

        this.gameObject.SetActive(isWalkable);
    }

    public void SetAsObstacle() => SetColor(obstacleColor);
    public void SetAsStart() => SetColor(startColor);
    public void SetAsEnd() => SetColor(endColor);
    public void SetAsPath() => SetColor(pathColor);
    public void SetAsProcessing() => SetColor(processingColor);
    public void SetAsFrontier() => SetColor(frontierColor);
    public void SetAsVisited() => SetColor(visitedColor);

    private void SetColor(Color c)
    {
        if (meshRenderer) meshRenderer.material.color = c;
    }
}