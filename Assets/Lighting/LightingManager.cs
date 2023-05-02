using UnityEngine;
using UnityEngine.VFX;

public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Material Sky_material;
    [SerializeField] private LightingPreset Preset;

    int DayLength = 180;
    //Variables
    [SerializeField] private float TimeOfDay;
    [SerializeField] private VisualEffect FireVFX;
    [SerializeField] private bool fire = false;

    private void Start()
    {
        FireVFX.Play();
        FireVFX.Stop();
        fire = false;
    }

    private void Update()
    {
        //(Replace with a reference to the game time)
        TimeOfDay += Time.deltaTime;
        TimeOfDay %= DayLength; //Modulus to ensure always between 0-DayLength
        UpdateLighting(TimeOfDay / DayLength);
    }


    private void UpdateLighting(float timePercent)
    {
        // day
        if (timePercent <= 0.5f)
        {
            if (fire)
            {
                fire = !fire;
                FireVFX.Stop();
                DirectionalLight.intensity = 2;
            }

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f), 20f, 0));
            
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f), 20f, 0));
            float colorPercent = timePercent * 2.0f;
            DirectionalLight.color = Preset.DirectionalColor_Day.Evaluate(colorPercent);
            Sky_material.SetColor("_SkyTint", Preset.SkyBox_SkyColor_Day.Evaluate(colorPercent));
            Sky_material.SetFloat("_SunSize", (Mathf.Abs(timePercent * 4.0f - 1.0f)) * 0.05f + 0.04f);
        }
        // night
        else
        {
            if (!fire)
            {
                fire = !fire;
                FireVFX.Play();
                DirectionalLight.intensity = 1;
            }
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(((timePercent - 0.5f) * 360f), 20f, 0));
            float colorPercent = (timePercent - 0.5f) * 2.0f;
            
            DirectionalLight.color = Preset.DirectionalColor_Night.Evaluate(colorPercent);
            Sky_material.SetColor("_SkyTint", Preset.SkyBox_SkyColor_Night.Evaluate(colorPercent));
        }
    }

}