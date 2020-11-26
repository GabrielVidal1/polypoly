using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private float height = 15f;
    
    [SerializeField] private Transform leftPole;
    [SerializeField] private Transform rightPole;
    [SerializeField] private Transform banderole;

    public void SetUp(Vector3 leftPos, Vector3 rightPos)
    {
        leftPole.position = leftPos;
        rightPole.position = rightPos;
        
        leftPole.localScale = new Vector3(1f, height + 0.7f, 1f);
        rightPole.localScale = new Vector3(1f, height + 0.7f, 1f);

        banderole.position = leftPos + Vector3.up * height;
        banderole.LookAt(rightPos + Vector3.up * height);
        banderole.localScale = new Vector3(1f, 1f, Vector3.Distance(leftPos, rightPos));
    }
}
