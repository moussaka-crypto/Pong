using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaSetter : MonoBehaviour
{
    public Canvas canvas;
    private RectTransform _panelSafeArea;
    public GameObject PlayBar;
    private Rect _currentSafeArea;

    private ScreenOrientation _currentOrientation = ScreenOrientation.LandscapeLeft;
    // Start is called before the first frame update
    void Start()
    {
        _panelSafeArea = GetComponent<RectTransform>();
        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;
        ApplySafeArea();
    }

    /// <summary>
    /// Resizes the panel to the Safe Area
    /// </summary>
    private void ApplySafeArea()
    {
        if (_panelSafeArea == null)
            return;
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        Rect pixelRect = canvas.pixelRect;
        
        anchorMin.x /= pixelRect.width;
        anchorMin.y /= pixelRect.height;

        anchorMax.x /= pixelRect.width;
        anchorMax.y /= pixelRect.height;

        _panelSafeArea.anchorMin = anchorMin;
        _panelSafeArea.anchorMax = anchorMax;

        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;
        //Setting bar Position inside the Scree Space
        PlayBar.transform.position =
            new Vector3(PlayBar.transform.position.x, safeArea.y-4 , PlayBar.transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        if ((_currentOrientation != Screen.orientation) ||
            (_currentSafeArea != Screen.safeArea)
           )
        {
            ApplySafeArea();
        }
    }
}
