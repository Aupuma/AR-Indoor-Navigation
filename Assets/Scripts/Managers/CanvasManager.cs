using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum AppState
{
    CALIBRATION,
    NAVIGATION,
    CHOOSING_DESTINATION,
}

public class CanvasManager : MonoBehaviour
{
    #region SINGLETON
    public static CanvasManager instance;
    
    private void Awake()
    {
        instance = this;
    }
    #endregion

    AppState _currentState;

    [SerializeField] POIManager _poiManager;

    [Header("Main Screen")]
    [SerializeField] GameObject _miniMap;
    RectTransform _minimapRectTransform;
    [SerializeField] Image _recalibrateButtonImg;

    [Header("Map Screen")]
    [SerializeField] GameObject _largeMap;
    [SerializeField] Animator _bgFader;
    [SerializeField] Camera _arCamera;
    [SerializeField] float _angleToChangeMap;

    [Header("Calibration Screen")]
    [SerializeField] CalibrationManager _calibrationManager;
    [SerializeField] Image _lookForMarkerImg;
    [SerializeField] TextMeshProUGUI _calibrationStateText;

    // Start is called before the first frame update
    void Start()
    {
        _minimapRectTransform = _miniMap.GetComponent<RectTransform>();
        _minimapRectTransform.localPosition = new Vector3(0f, _minimapRectTransform.localPosition.y - 500f);

        _calibrationManager.enabled = true;
        _currentState = AppState.CALIBRATION;
    }

    private void Update()
    {
        if(_currentState == AppState.NAVIGATION)
        {
            if(_arCamera.transform.eulerAngles.x >= _angleToChangeMap)
            {
                _bgFader.SetBool("mapActive", true);
            }
        }
        if (_currentState == AppState.CHOOSING_DESTINATION)
        {
            if (_arCamera.transform.eulerAngles.x < _angleToChangeMap)
            {
                _bgFader.SetBool("mapActive", false);
            }
        }
    }

    public void RecalibrateButtonPressedHandler()
    {
        SwitchState(AppState.CALIBRATION);
    }

    public void SwitchState(AppState appState)
    {
        switch (appState)
        {
            case AppState.CALIBRATION:
                _calibrationManager.enabled = true;
                SetMinimapVisibility(false);
                SetCalibrateButtonVisibility(false);
                NavigationManager.instance.EndNavigation();
                _currentState = appState;
                break;

            case AppState.NAVIGATION:

                if(_currentState == AppState.CALIBRATION)
                {
                    _calibrationManager.enabled = false;
                    SetMinimapVisibility(true);
                    SetCalibrateButtonVisibility(true);
                    NavigationManager.instance.SetNavigationReady();
                    _currentState = appState;
                }
                if(_currentState == AppState.CHOOSING_DESTINATION)
                {
                    _miniMap.SetActive(true);
                    _largeMap.SetActive(false);
                    _poiManager.ActivateMinimapCamera();
                    _currentState = appState;
                }
                break;

            case AppState.CHOOSING_DESTINATION:
                _miniMap.SetActive(false);
                _largeMap.SetActive(true);
                _poiManager.ActivateLargeMapCamera();
                _currentState = appState;
                break;
        }
    }

    public void ShowCalibrationState(string state, bool autoHide = false)
    {
        Sequence fadeTextSequence = DOTween.Sequence();

        if (_calibrationStateText.alpha > 0)
            fadeTextSequence.Append(_calibrationStateText.DOFade(0f, 0.5f)
            .OnComplete(() =>
            {
                _calibrationStateText.text = state;
            }));
        else
            _calibrationStateText.text = state;

        fadeTextSequence.Append(_calibrationStateText.DOFade(1f, 0.5f));

        if(autoHide)
            fadeTextSequence.Append(_calibrationStateText.DOFade(0f, 0.5f)).SetDelay(0.5f);
    }

    public void SetFindMarkerImgVisibility(bool visible)
    {
        if (visible)
            _lookForMarkerImg.DOFade(1f, 0.5f);
        else
            _lookForMarkerImg.DOFade(0f, 0.5f);
    }

    public void SetMinimapVisibility(bool visible)
    {
        if (visible)
            _minimapRectTransform.DOLocalMoveY(_minimapRectTransform.localPosition.y + 500, 1f);
        else
            _minimapRectTransform.DOLocalMoveY(_minimapRectTransform.localPosition.y - 500, 1f);
    }

    public void SetCalibrateButtonVisibility(bool visible)
    {
        if (visible)
            _recalibrateButtonImg.DOFade(1f, 0.5f);
        else
            _recalibrateButtonImg.DOFade(0f, 0.5f);
    }
}
