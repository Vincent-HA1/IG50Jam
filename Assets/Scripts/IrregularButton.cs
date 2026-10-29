using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class IrregularButton : MonoBehaviour
{
    void Start()
    {
        //Only can click non transparent parts, enable read write on sprite import to allow this
        GetComponent<Image>().alphaHitTestMinimumThreshold = 0.01f;
    }
}