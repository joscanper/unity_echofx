using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EchoData", menuName = "Echo")]
public class EchoData : ScriptableObject
{
    public Vector3[] Vertices;
    public Vector3[] Normals;
    public Vector3 LightDirection = new Vector3(1, 1, 0);
}