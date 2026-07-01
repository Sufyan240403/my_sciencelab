using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackground : MonoBehaviour
{
    [SerializeField] private GameObject backgroundQuad;
    private WebCamTexture webCamTexture;

    private IEnumerator Start()
    {
        
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
     
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            
            webCamTexture = new WebCamTexture();
            if (backgroundQuad !=null)
            {
                Renderer rend = backgroundQuad.GetComponent<Renderer>();
                if (rend != null)
                
                    rend.material.mainTexture = webCamTexture;
                }
                    webCamTexture.Play();
                }
                else
                {
                    Debug.LogError("Renderer component not found on backgroundQuad.");
                }
            }
    private void OnDestroy()
    {
        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
        }
    }





}
