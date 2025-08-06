using UnityEngine;

public class SunLightController : MonoBehaviour
{
    public Transform planet;

    void Update()
    {
        if (planet != null)
        {
            // Make the directional light point toward the planet
            transform.rotation = Quaternion.LookRotation(planet.position - transform.position);
        }
    }
}
