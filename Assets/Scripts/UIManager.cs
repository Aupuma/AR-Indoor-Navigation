using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
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
}
