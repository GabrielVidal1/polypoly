using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionSide : MonoBehaviour
{
    [SerializeField] private Transform mesh;

    public void Set(Vector3 pointA, Vector3 pointB)
    {
        mesh.position = pointA;
        mesh.LookAt(pointB);
        mesh.localScale = new Vector3(1, 1, Vector3.Distance(pointA, pointB));
    }
}
