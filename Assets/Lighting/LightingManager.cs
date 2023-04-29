using UnityEngine;

public class LightingManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private Material Sky_material;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] private float TimeOfDay;


    private void Update()
    {
        //(Replace with a reference to the game time)
        TimeOfDay += Time.deltaTime;
        TimeOfDay %= 24; //Modulus to ensure always between 0-24
        UpdateLighting(TimeOfDay / 24f);
    }


    private void UpdateLighting(float timePercent)
    {
        //Set ambient and fog
        //RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        //RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
        DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
        Sky_material.SetColor("_SkyTint", Preset.SkyBox_SkyColor.Evaluate(timePercent));

        //var t = (timePercent + 0.5f);
        //t = t > 1 ? t - 1 : t;
        //DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Abs(t * 180f), 170f, 0));

        DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));

    }

}