using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PostProcessDistance : MonoBehaviour
{
    Volume vol;
    Vignette vignette;
    ChromaticAberration aberration;

    public float minDistance;
    childDistance closestDist;

    public float abberIntensity = 0.2f;
    public float vignetteIntensity = 0.2f;

    public float maxVignette = 0.4f;
    public float maxChroma = 1f;

    float distanceToClosest;

    private void Start()
    {
        vol = GetComponent<Volume>();
        if (vol != null && vol.profile != null)
        {
            // Try to get Vignette effect
            if (!vol.profile.TryGet(out vignette))
                Debug.LogError("Vignette effect not found in Volume profile.");

            // Try to get Chromatic Aberration effect
            if (!vol.profile.TryGet(out aberration))
                Debug.LogError("Chromatic Aberration effect not found in Volume profile.");
        }
        else
        {
            Debug.LogError("Volume or Volume profile is missing.");
        }
        closestDist = GameObject.Find("Player").GetComponent<childDistance>();
        ChangeAberration(abberIntensity);
        ChangeVignette(vignetteIntensity);
    }

    private void Update()
    {
        distanceToClosest = Mathf.Sqrt(closestDist.dist);
        if (distanceToClosest < minDistance)
        {          
            if ((minDistance - distanceToClosest) / minDistance <= vignetteIntensity)
            {
                //Debug.Log("1");
                ChangeVignette(vignetteIntensity);
            }
            else if ((minDistance - distanceToClosest) / minDistance > vignetteIntensity && (minDistance - distanceToClosest) / minDistance < maxVignette)
            {
                //Debug.Log("2");
                ChangeVignette((minDistance - distanceToClosest)/minDistance);
            }
            else
            {
                //Debug.Log("3");
                ChangeVignette(maxVignette);
            }

            if ((minDistance - distanceToClosest)/minDistance <= abberIntensity)
            {
                //Debug.Log("1");
                ChangeAberration(vignetteIntensity);
            }
            else if ((minDistance - distanceToClosest) / minDistance > abberIntensity && (minDistance - distanceToClosest) / minDistance < maxChroma)
            {
                //Debug.Log("2");
                ChangeAberration((minDistance - distanceToClosest)/minDistance);
            }
            else
            {
                //Debug.Log("3");
                ChangeAberration(maxChroma);
            }

        }
        else
        {
            ChangeVignette(vignetteIntensity);
            ChangeAberration(abberIntensity);
        }
    }

    public void ChangeVignette(float intensity)
    {
        if (vignette != null)
        {
            vignette.intensity.value = intensity;
        }
        else
        {
            Debug.Log("Vignette is null.");
        }
    }

    public void ChangeAberration(float intensity)
    {
        if (aberration != null)
        {
            aberration.intensity.value = intensity;
        }
        else
        {
            Debug.Log("Abber is null.");
        }
    }
}