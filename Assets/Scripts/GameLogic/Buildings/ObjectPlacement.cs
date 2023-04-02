using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPlacement : MonoBehaviour
{
    public abstract void Place(HexCell cell);
}
