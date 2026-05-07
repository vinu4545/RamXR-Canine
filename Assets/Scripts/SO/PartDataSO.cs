using UnityEngine;

[CreateAssetMenu(fileName = "ModelPartData", menuName = "VR/Model Part Data")]
public class ModelPartDataSO : ScriptableObject
{
    public string id;
    public string partName;
    [TextArea(3, 10)]
    public string[] descriptionChunks;
}