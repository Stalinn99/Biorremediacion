using UnityEngine;

public class InformationObject : MonoBehaviour
{
    [Header("Object Data")]
    public string displayName;
    public string size;
    
    [TextArea(3, 5)]
    public string specifications;
}