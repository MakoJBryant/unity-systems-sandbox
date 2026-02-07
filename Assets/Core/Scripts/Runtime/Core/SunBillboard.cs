using UnityEngine;

public class SunBillboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
