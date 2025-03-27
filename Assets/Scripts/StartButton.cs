using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartButton : MonoBehaviour
{
    [SerializeField] private GameObject _cutSceneCamera;
    [SerializeField] private GameObject _canva;
    private Animator _animator;


    private void Start()
    {
        _animator = _cutSceneCamera.GetComponent<Animator>();
    }
    public void StartBtn()
    {
        _animator.SetBool("Start", true);
        _canva.SetActive(false);
    }
}
