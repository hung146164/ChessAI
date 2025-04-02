using UnityEngine;

[CreateAssetMenu(fileName = "TileColorConfig", menuName = "Data/TileColorConfig")]
public class TileColorConfig : ScriptableObject
{
    public Material hoverMaterial;
    public Material exitMaterial;
    public Material selectMaterial;
    public Material validMoveMaterial;
}