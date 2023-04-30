using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient SkyBox_SkyColor_Day;
    public Gradient SkyBox_SkyColor_Night;
    //public Color SkyBox_SkyColor_MidDay;
    public Gradient DirectionalColor_Day;
    public Gradient DirectionalColor_Night;
    //public Gradient FogColor;
}