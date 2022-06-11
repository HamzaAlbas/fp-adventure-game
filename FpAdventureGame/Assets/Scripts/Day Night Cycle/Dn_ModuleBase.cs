using System;
using UnityEngine;

public abstract class Dn_ModuleBase : MonoBehaviour
{
    protected DayNightCycle dayNightCycle;

    private void OnEnable()
    {
        dayNightCycle = this.GetComponent<DayNightCycle>();
        if (dayNightCycle != null)
        {
            dayNightCycle.AddModule(this);
        }
    }

    public abstract void UpdateModule(float intensity);

    private void OnDisable()
    {
        if (dayNightCycle != null)
        {
            dayNightCycle.RemoveModule(this);
        }
    }
}
