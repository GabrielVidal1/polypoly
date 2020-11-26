using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionCollider : MonoBehaviour
{
    private Section _section;
    
    public void SetUp(Section section)
    {
        _section = section;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().CurrentSection = _section;
        }
    }
}
