using System.Collections;
using UnityEngine;

public class WebViewController : MonoBehaviour
{
    public string Url;
    public int LeftMargin = 100;
    public int RightMargin = 100;
    public int TopMargin = 150;
    public int BottomMargin = 150;

    private WebViewObject webViewObject;
    private bool _initialized = false;

    public void OpenWebView()
    {
        StartCoroutine(LoadWebView(Url));
    }

    public void OpenURL(string newUrl)
    {
        Url = newUrl;
        StartCoroutine(LoadWebView(Url));
    }

    public void CloseWebView()
    {
        if (webViewObject != null)
            webViewObject.SetVisibility(false);
    }

    public void SetVisibility(bool visibility)
    {
        if (webViewObject != null)
            webViewObject.SetVisibility(visibility);
    }

    public bool GetVisibility()
    {
        return webViewObject != null && webViewObject.GetVisibility();
    }

    private void OnDisable()
    {
        if (webViewObject != null)
            webViewObject.SetVisibility(false);
    }

    private IEnumerator LoadWebView(string loadUrl)
    {
        if (!_initialized)
        {
            // Auto-create the WebViewObject — no manual assignment needed
            webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

            webViewObject.Init(
                cb: (msg) => { Debug.Log("WebView CB: " + msg); },
                err: (msg) => { Debug.LogError("WebView Error: " + msg); },
                httpErr: (msg) => { Debug.LogError("WebView HTTP Error: " + msg); },
                started: (msg) => { Debug.Log("WebView Started: " + msg); },
                hooked: (msg) => { },
                cookies: (msg) => { },
                ld: (msg) => { Debug.Log("WebView Loaded: " + msg); }
            );

            webViewObject.SetMargins(LeftMargin, TopMargin, RightMargin, BottomMargin);
            webViewObject.SetTextZoom(30);

            // Keep it alive between scenes
            DontDestroyOnLoad(webViewObject.gameObject);

            _initialized = true;
        }

        if (!string.IsNullOrEmpty(loadUrl) && loadUrl.StartsWith("http"))
        {
            webViewObject.LoadURL(loadUrl.Replace(" ", "%20"));
        }

        webViewObject.SetVisibility(true);
        yield break;
    }
}