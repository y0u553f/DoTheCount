using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SquareStateenum { entryPoint, endPoint, normal , used}

[CreateAssetMenu (fileName = "New State" , menuName = "")]
public class SquareStateObject : ScriptableObject
{
    public Sprite logo;
    public Color color;
    public SquareStateenum state;
}
