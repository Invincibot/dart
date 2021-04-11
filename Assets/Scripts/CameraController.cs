using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static readonly Vector3 spacing = Vector3.back * 10;

    public Transform target;

    [SerializeField] private Color[] colors;
    [SerializeField] [Range(0, 1)] private float colorShiftSpeed;
    private Color _currentColor;
    private Color _nextColor;
    private int _currentColorIndex;
    private float _lerpDistance;
    private Camera _camera;

    private void Start()
    {
        _currentColorIndex = 0;
        _currentColor = colors[0];
        _nextColor = colors[1];
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        transform.position = target.transform.position + spacing;

        _lerpDistance += colorShiftSpeed * Time.deltaTime;
        if (_lerpDistance >= 1)
        {
            _lerpDistance = 0;
            _currentColor = _nextColor;
            _currentColorIndex = _currentColorIndex == colors.Length - 1 ? 0 : _currentColorIndex + 1;
            _nextColor = colors[_currentColorIndex];
        }

        _camera.backgroundColor = Color.Lerp(_currentColor, _nextColor, _lerpDistance);
    }
}
