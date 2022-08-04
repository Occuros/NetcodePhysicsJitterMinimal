using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SimulatedPlayerMovement : MonoBehaviour
{
    public List<Transform> Points;
    public float Speed = 10;
    private int _currentIndex;
    [SerializeField] private float _progress;
    [SerializeField] private float3 _currentPosition;
    private Transform _previousPoint;

    private void Awake()
    {
        _previousPoint = Points[0];
        _currentIndex = 1;

    }

    void Update()
    {
        _progress += Time.deltaTime * Speed;

        if (_progress >= 1.0f)
        {
            _previousPoint = Points[_currentIndex];
            _progress = 0;
            _currentIndex = (_currentIndex + 1) % Points.Count;

        }
        var nextPoint = Points[_currentIndex];
        
        transform.position = math.lerp(_previousPoint.position, nextPoint.position, _progress);
    }
}
