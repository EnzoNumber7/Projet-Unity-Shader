using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private GameObject _fadeCamera;
    [SerializeField] private GameObject _characterControls;
    [SerializeField] private GameObject _characterCamera;


    [SerializeField] private bool _changeToCameraPriority;

    private void Update()
    {
        if (!_changeToCameraPriority) return;

        ChangePriority();
    }

    private void ChangePriority()
    {
        _fadeCamera.GetComponent<CinemachineVirtualCamera>().Priority = 0;
        _characterControls.GetComponent<Controls>().enabled = true;
        _fadeCamera.transform.position = _characterCamera.transform.position;
        _fadeCamera.transform.rotation = _characterCamera.transform.rotation;

    }
}
