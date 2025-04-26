using UnityEngine;

public enum SquareStateenum { entryPoint, endPoint, normal, used }

[CreateAssetMenu(fileName = "New State", menuName = "")]
public class SquareStateObject : ScriptableObject
{
    public Sprite logo;
    public Color color;
    public SquareStateenum state;
    public float transitionDuration = 0.5f;
}