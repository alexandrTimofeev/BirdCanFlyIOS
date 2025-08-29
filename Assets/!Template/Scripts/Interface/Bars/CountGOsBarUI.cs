using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class CountGOsBarUI : BarUI
{
    [Space]
    [SerializeField] private GameObject prefab;
    [SerializeField] private float offset = 0.5f;
    private List<GameObject> listObs = new List<GameObject>();

    [Space]
    [SerializeField] private bool useMaxValue;
    [SerializeField] private GameObject prefabMaxValue;
    private List<GameObject> listMaxValueObs = new List<GameObject>();

    public override void ShowMaxValueInInterface(float maxValue)
    {
        if (useMaxValue == false)
        {
            foreach (var ob in listMaxValueObs)
            {
                ob.SetActive(false);
            }
            return;
        }

        int count = (int)(maxValue - CurrentValue);
        for (int i = 0; i < listMaxValueObs.Count; i++)
        {
            listMaxValueObs[i].SetActive(i < count);
            listMaxValueObs[i].transform.localPosition += (listObs.Count > 0 ?
                listObs[(int)CurrentValue].transform.localPosition : Vector3.zero) + (Vector3.right * offset * i);
        }
        count -= listMaxValueObs.Count;
        while (count > 0)
        {
            CreateMaxOb();
            count--;
        }
    }

    public override void ShowValueInInterface(float currentValue)
    {
        int count = (int)currentValue;
        for (int i = 0; i < listObs.Count; i++)
        {
            listObs[i].SetActive(i < count);
        }
        count -= listObs.Count;
        while (count > 0)
        {
            CreateOb();
            count--;
        }
    }

    private void CreateOb()
    {
        GameObject ngo = Instantiate(prefab, transform);
        ngo.transform.localPosition += ((listObs.Count > 0 && listObs.Count > CurrentValue) ?
            listObs[(int)CurrentValue - 1].transform.localPosition : Vector3.zero) + (Vector3.right * offset);
        listObs.Add(ngo);
    }

    private void CreateMaxOb()
    {
        GameObject ngo = Instantiate(prefabMaxValue, transform);
        ngo.transform.localPosition += ((listObs.Count > 0 && listObs.Count > CurrentValue) ?
            listObs[(int)CurrentValue - 1].transform.localPosition : Vector3.zero) + (Vector3.right * offset);
        listMaxValueObs.Add(ngo);
    }
}