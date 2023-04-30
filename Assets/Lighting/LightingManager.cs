using UnityEngine;

public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Material Sky_material;
    [SerializeField] private LightingPreset Preset;

    int DayLength = 24;
    //Variables
    [SerializeField] private float TimeOfDay;


    private void Update()
    {
        //(Replace with a reference to the game time)
        TimeOfDay += Time.deltaTime;
        TimeOfDay %= DayLength; //Modulus to ensure always between 0-DayLength
        UpdateLighting(TimeOfDay / DayLength);
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        //RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        //RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        // day
        if (timePercent <= 0.5)
        {
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f), 20f, 0));
            
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f), 20f, 0));
            float colorPercent = timePercent * 2.0f;
            DirectionalLight.color = Preset.DirectionalColor_Day.Evaluate(colorPercent);
            Sky_material.SetColor("_SkyTint", Preset.SkyBox_SkyColor_Day.Evaluate(colorPercent));
        }
        // night
        else
        {
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(((timePercent - 0.5f) * 360f), 20f, 0));
            float colorPercent = (timePercent - 0.5f) * 2.0f;
            
            DirectionalLight.color = Preset.DirectionalColor_Night.Evaluate(colorPercent);
            Sky_material.SetColor("_SkyTint", Preset.SkyBox_SkyColor_Night.Evaluate(colorPercent));
        }
    }

}