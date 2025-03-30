using UnityEngine;

public class HighlightOnKey : MonoBehaviour
{
    public enum ActivationType { HoldA, HoldD, NoKey }
    public ActivationType activationCondition;

    public SpriteRenderer[] targets;
    public Color highlightColor = Color.white;

    private Color[] originalColors;

    void Start()
    {
        originalColors = new Color[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
                originalColors[i] = targets[i].color;
        }
    }

    void Update()
    {
        bool shouldHighlight = false;

        if (activationCondition == ActivationType.HoldA && Input.GetKey(KeyCode.A))
            shouldHighlight = true;
        else if (activationCondition == ActivationType.HoldD && Input.GetKey(KeyCode.D))
            shouldHighlight = true;
        else if (activationCondition == ActivationType.NoKey && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            shouldHighlight = true;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == null) continue;

            targets[i].color = shouldHighlight ? highlightColor : originalColors[i];
        }
    }
}
