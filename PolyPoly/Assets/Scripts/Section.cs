using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Section : MonoBehaviour
{
    [SerializeField] private SectionSide leftSide;
    [SerializeField] private SectionSide rightSide;

    [SerializeField] private SectionCollider sectionCollider;

    [SerializeField] private FinishLine finishLinePrefab;
    
    public Vector3 CurrentPoint { get; private set; } 
    public Vector3 NextPoint { get; private set; } 
    public Vector3 Direction { get; private set; }
    public float Length { get; private set; }
    
    public void SetPoints(float circuitWidth, Vector3 prevPoint, Vector3 currentPoint, Vector3 nextPoint, bool isFinishLine = false)
    {
        sectionCollider.SetUp(this);
        
        CurrentPoint = currentPoint;
        NextPoint = nextPoint;
        Direction = (nextPoint - currentPoint).normalized;
        Length = Vector3.Distance(currentPoint, nextPoint);
        transform.position = currentPoint;

        Vector3 prevDir = (currentPoint - prevPoint).normalized;
        Vector3 prevX = new Vector3(prevDir.z, 0, -prevDir.x);
        Vector3 currentLeft = prevX * circuitWidth * 0.5f + currentPoint;
        Vector3 currentRight = - prevX * circuitWidth * 0.5f + currentPoint;

        Vector3 currentDir = (nextPoint - currentPoint).normalized;
        Vector3 currentX = new Vector3(currentDir.z, 0, -currentDir.x);
        Vector3 nextLeft = currentX * circuitWidth * 0.5f + nextPoint;
        Vector3 nextRight = - currentX * circuitWidth * 0.5f + nextPoint;

        sectionCollider.transform.position = currentPoint;
        sectionCollider.transform.localScale = new Vector3(circuitWidth, 1f, Vector3.Distance(currentPoint, nextPoint));
        sectionCollider.transform.LookAt(nextPoint);
        
        leftSide.Set(currentLeft, nextLeft);
        rightSide.Set(currentRight, nextRight);

        if (isFinishLine)
        {
            FinishLine finishLine = Instantiate(finishLinePrefab, transform);
            finishLine.SetUp(currentLeft, currentRight);
        }
    }
}
