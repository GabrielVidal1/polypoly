using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Circuit : MonoBehaviour
{
    [SerializeField] private Section sectionPrefab;

    [SerializeField] private int sectionNumber = 8;
    [SerializeField, Range(1f, 10f)] private float k;
    [SerializeField] private float circuitRadius = 40;
    [SerializeField, Range(0f, 1f)] private float circuitRadiusRandom = 0;
    [SerializeField] private float circuitWidth = 10f;
    [SerializeField] private Vector4 seed;

    public List<Section> sections = null;
    
    public void CreateCircuit()
    {
        sections = new List<Section>();
        
        List<Vector3> points = new List<Vector3>();
        int total = (int) (sectionNumber * k);
        for (int i = 0; i < total; i++)
        {
            float angle = 2 * Mathf.PI * i / total;

            float distance = circuitRadius + circuitRadius * circuitRadiusRandom * (1f - 2f * Mathf.PerlinNoise(seed.x + Mathf.Cos(seed.y * angle), seed.z + Mathf.Sin(seed.w * angle)));
            
            points.Add(distance * (Mathf.Cos(angle) * Vector3.forward + Mathf.Sin(angle) * Vector3.right));
        }

        for (int i = 0; i < total - sectionNumber; i++)
            points.RemoveAt((int) (Random.value * points.Count));

        for (int i = 0; i < sectionNumber; i++)
        {
            Section section = Instantiate(sectionPrefab, transform);
            section.SetPoints(circuitWidth, points[(i - 1 + points.Count) % points.Count], points[i], points[(i + 1) % points.Count], i == 0);
            sections.Add(section);
        }
    }

    IEnumerator Test()
    {
        while (true)
        {
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            CreateCircuit();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
