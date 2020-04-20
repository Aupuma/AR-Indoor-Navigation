using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject mapScreen;
    public GameObject calibrationScreen;

    public GameObject calibrationMarker;

    [SerializeField] POIManager _poiManager;

    [SerializeField] Image _bgFader;
    [SerializeField] float _fadeTime;

    [SerializeField] GameObject _miniMap;
    [SerializeField] GameObject _largeMap;

    Sequence _fadeSequence;
    bool _isLargeMapVisible = false;

    [Header("Calibration Screen")]
    [SerializeField] Image _lookForMarkerImg;
    [SerializeField] TextMeshProUGUI _calibrationStateText;
    string[] _calibrationStates = new string[3] { "Looking for a marker...", "Calibrating...", "Calibration complete" };
    int _currentCalibrationState = -1;

    // Start is called before the first frame update
    void Start()
    {
        //Prepares the sequence
        _fadeSequence = DOTween.Sequence();
        _fadeSequence.Append(_bgFader.DOFade(1, _fadeTime / 2f));
        _fadeSequence.Append(_bgFader.DOFade(0, _fadeTime / 2f));
        _fadeSequence.SetAutoKill(false);
        _fadeSequence.Pause();
    }

    public void MinimapPressed()
    {
        _fadeSequence.Restart();
        _fadeSequence.Play();
        Invoke("SwitchMapMode", _fadeTime / 2);
    }


    public void BackFromLargeMapPressed()
    {
        _fadeSequence.Restart();
        _fadeSequence.Play();
        Invoke("SwitchMapMode", _fadeTime / 2);
    }

    private void SwitchMapMode()
    {
        if (_isLargeMapVisible)
        {
            _isLargeMapVisible = false;
            _miniMap.SetActive(true);
            _largeMap.SetActive(false);
            _poiManager.SwitchToMinimapMode();
        }
        else
        {
            _isLargeMapVisible = true;
            _miniMap.SetActive(false);
            _largeMap.SetActive(true);
            _poiManager.SwitchToLargeMapMode();
        }
    }

    public void SetMapScreenVisibility(bool visible)
    {
        mapScreen.SetActive(visible);
        mainScreen.SetActive(!visible);
    }

    public void SetCalibrationScreenVisibility(bool visible)
    {
        calibrationScreen.SetActive(visible);
        calibrationMarker.SetActive(visible);
        mainScreen.SetActive(!visible);
    }

    public void ShowNextCalibrationState()
    {
        _currentCalibrationState++;

        if (_currentCalibrationState == 0)
        {
            _calibrationStateText.DOFade(1f, 0.5f);
        }
        else if(_currentCalibrationState < _calibrationStates.Length)
        {
            _calibrationStateText.DOFade(0f, 0.5f).OnComplete(()=>
            {
                _calibrationStateText.text = _calibrationStates[_currentCalibrationState];
                _calibrationStateText.DOFade(1f, 0.5f);
            });
        }
        else if (_currentCalibrationState == _calibrationStates.Length)
        {
            _calibrationStateText.DOFade(0f, 0.5f).OnComplete(() =>
            {
                _currentCalibrationState = -1;
            });
        }
    }

    public void SetFindMarkerImgVisibility(bool visible)
    {
        if (visible)
            _lookForMarkerImg.DOFade(1f, 0.5f);
        else
            _lookForMarkerImg.DOFade(0f, 0.5f);
    }
}
