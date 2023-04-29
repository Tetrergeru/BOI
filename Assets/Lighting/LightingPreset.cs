using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Scriptables/Lighting Preset", order = 1)]
public class LightingPreset : ScriptableObject
{
    public Gradient SkyBox_SkyColor;
    //public Color SkyBox_SkyColor_MidDay;
    public Gradient DirectionalColor;
    //public Gradient FogColor;
}