using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtils;

public class SnowParticleController : MonoBehaviour
{
    public static SnowParticleController main;
    public ParticleSystemForceField windField;
    public ParticleSystem topSnow;
    public ParticleSystem aboveSnow;
    public ParticleSystem belowSnow;

    [Range(0, 1)]
    [SerializeField]
    private float _intensity = 0;
    public float Intensity { get => _intensity; set => SetIntensity(value); }
    [SerializeField]
    private FloatRange _windRange = new FloatRange(0, 40);

    public FloatRange aboveEmissionOverTime = new FloatRange(0, 40);
    public FloatRange topEmissionOverTime = new FloatRange(0, 10);
    public FloatRange belowEmissionOverTime = new FloatRange(0, 10);
    public FloatRange gravityMinMax = new FloatRange(0, 0.4f);
    public FloatRange emissionOverTimeRange = new FloatRange(10, 10);

    private void Awake()
    {
        if(main == null)
        {
            main = this;
            SetIntensity(_intensity);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void Stop()
    {
        topSnow.Stop();
        aboveSnow.Stop();
        belowSnow.Stop();
    }

    private void SetIntensity(float intensity)
    {
        _intensity = intensity;
        SetEmitter(aboveSnow, intensity, aboveEmissionOverTime);
        SetEmitter(topSnow, intensity, topEmissionOverTime);
        SetEmitter(belowSnow, intensity, belowEmissionOverTime);
        windField.directionX = _windRange.Lerp(intensity);
    }

    private void SetEmitter(ParticleSystem ps, float intensity, FloatRange emissionOT)
    {
        var m = ps.main;
        m.gravityModifier = gravityMinMax.Lerp(intensity);
        var e = ps.emission;
        var rot = e.rateOverTime;
        rot.constantMin = emissionOT.Lerp(intensity);
        rot.constantMax = rot.constantMin + emissionOverTimeRange.Lerp(intensity);
        e.rateOverTime = rot;
    }
}
