using System;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Presets;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time")] [Tooltip("Day length in minutes")] [SerializeField]
    private float targetDayLength = 13f;
    public float TargetDayLength => targetDayLength;
    [SerializeField] private float elapsedTime;
    [SerializeField] private TextMeshProUGUI timeText;

    [SerializeField] [Range(0f, 1f)] 
    private float timeOfDay;
    public float TimeOfDay => timeOfDay;

    [SerializeField] private int dayNumber = 0;
    public int DayNumber => dayNumber;

    [SerializeField] private int yearNumber = 0;
    public int YearNumber => yearNumber;

    [SerializeField] private int yearLength = 100;
    public int YearLength => yearLength;

    private float _timeScale = 100f;
    
    public bool pause = false;

    [SerializeField] private AnimationCurve timeCurve;
    private float _timeCurveNormalization;
    

    [Header("Sun Light")] [SerializeField] private Transform dailyRotation;
    [SerializeField] private Light sun;
    private float _intensity;

    [SerializeField] private float sunBaseIntensity = 1f;
    [SerializeField] private float sunVariation = 1.5f;
    [SerializeField] private Gradient sunColor;

    [Header("Seasonal Variables")] [SerializeField] private Transform sunSeasonalRotation;
    [SerializeField] [Range(-45f, 45f)] private float maxSeasonalTilt;

    [Header("Modules")] private List<Dn_ModuleBase> _moduleList = new List<Dn_ModuleBase>();

    private void Start()
    {
        NormalTimeCurve();
    }

    private void Update()
    {
        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime(); 
            UpdateClock();
        }
        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
        UpdateModules();
    }


    private void UpdateTimeScale()
    {
        _timeScale = 25 / (targetDayLength / 60);
        _timeScale *= timeCurve.Evaluate(elapsedTime / (targetDayLength * 60));
        _timeScale /= _timeCurveNormalization;
    }

    private void NormalTimeCurve()
    {
        const float stepSize = 0.01f;
        var numberSteps = Mathf.FloorToInt(1f / stepSize);
        var curveTotal = 0f;

        for (var i = 0; i < numberSteps; i++)
        {
            curveTotal += timeCurve.Evaluate(i * stepSize);
        }

        _timeCurveNormalization = curveTotal / numberSteps;
    }
    
    private void UpdateTime()
    {
        timeOfDay += Time.deltaTime * _timeScale / 86400;
        elapsedTime += Time.deltaTime;
        if (timeOfDay > 1)
        {
            elapsedTime = 0;
            dayNumber++;
            timeOfDay -= 1;
            if (dayNumber > yearLength)
            {
                yearNumber++;
                dayNumber = 0;
            }
        }
    }

    private void UpdateClock()
    {
        var time = elapsedTime / (targetDayLength * 60);
        var hour = Mathf.FloorToInt(time * 24);
        var minute = Mathf.FloorToInt(((time * 24) - hour) * 60);

        string hourString;
        string minuteString;

        if (hour < 10)
            hourString = "0" + hour.ToString();
        else
            hourString = hour.ToString();
        
        if (minute < 10)
            minuteString = "0" + minute.ToString();
        else
            minuteString = minute.ToString();
        
        timeText.text = hourString + " : " + minuteString;

        
    }
    
    private void AdjustSunRotation()
    {
        var sunAngle = timeOfDay * 360f;
        dailyRotation.transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, sunAngle));

        if (timeOfDay >= 0.25f && timeOfDay <= 0.75f)
        {
            RenderSettings.skybox.SetFloat("_Exposure", 1f);

        }
        else
        {
            RenderSettings.skybox.SetFloat("_Exposure", 0.15f);
        }

        

        var seasonalAngle = -maxSeasonalTilt * Mathf.Cos(dayNumber / yearLength * 2f * Mathf.PI);
        sunSeasonalRotation.localRotation = Quaternion.Euler(new Vector3(seasonalAngle, 0f, 0f));
    }

    private void SunIntensity()
    {
        _intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        _intensity = Mathf.Clamp01(_intensity);

        sun.intensity = _intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(_intensity);
    }

    public void AddModule(Dn_ModuleBase module)
    {
        _moduleList.Add(module);
    }

    public void RemoveModule(Dn_ModuleBase module)
    {
        _moduleList.Remove(module);
    }

    private void UpdateModules()
    {
        foreach (Dn_ModuleBase module in _moduleList)
        {
            module.UpdateModule(_intensity);
        }
    }
}
