using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float _minFingerMovementToMoveMap = 20f;
    [SerializeField] float _pressingTimeToChooseDestination = 0.5f;
    [SerializeField] Image _centerCameraActiveImage;
    [SerializeField] float _recenterCameraDuration = 1f;

    [Header("References")]
    [SerializeField] Transform _topDownCameraOrigin;
    [SerializeField] Camera _topDownCamera;
    [SerializeField] NavigationManager _navigationManager;

    Vector3 _touchStartWorld; // start of finger touch
    Vector3 _touchStartScreen;
    float _pressingTimer = 0f;
    bool _hasMovedCamera = false;

    void Update()
    {
        // mouse of fingertouch moves camera
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartScreen = Input.mousePosition;
            _touchStartWorld = _topDownCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        else if (Input.GetMouseButton(0))
        {
            Vector3 fingerMovement = _touchStartScreen - Input.mousePosition;

            //If the finger does not practically move, a destination is chosen
            if (fingerMovement.magnitude < _minFingerMovementToMoveMap)
            {
                if (_pressingTimer < _pressingTimeToChooseDestination)
                {
                    _pressingTimer += Time.deltaTime;
                }
                else
                {
                    _navigationManager.SetManualDestination(Input.mousePosition);
                    _pressingTimer = 0f;
                }
            }
            else //Otherwise the map is moved proportionally to the distance the finger moved
            {
                Move(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            _pressingTimer = 0f;
        }
    }

    private void Move(Vector2 currentPos)
    {
        if (!_hasMovedCamera)
        {
            _topDownCamera.transform.parent = null;
            _hasMovedCamera = true;
            _centerCameraActiveImage.gameObject.SetActive(false);
        }
        DOTween.Kill(_topDownCamera.transform);

        Vector3 direction = _touchStartWorld - _topDownCamera.ScreenToWorldPoint(currentPos);
        _topDownCamera.transform.position += direction;
    }

    public void MoveCameraToCenter()
    {
        if (_hasMovedCamera)
        {
            _topDownCamera.transform.DOMove(_topDownCameraOrigin.position, _recenterCameraDuration).SetEase(Ease.OutCirc).
                OnComplete(RecenterCamera);
        }
    }

    public void RecenterCamera()
    {
        _hasMovedCamera = false;
        DOTween.Kill(_topDownCamera.transform);
        _topDownCamera.transform.position = _topDownCameraOrigin.position;
        _topDownCamera.transform.SetParent(_topDownCameraOrigin);
        _centerCameraActiveImage.gameObject.SetActive(true);
    }
}
