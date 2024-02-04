using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Camera _camera;
    private Animator _animator;

    private bool _isIdle;

    private void Awake()
    {
        _spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _animator = transform.GetChild(0).GetComponent<Animator>();
        _camera = Camera.main;

        _isIdle = true;
    }

    private void Update()
    {
        var direction = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;
        var displacement = direction * (Time.deltaTime * 3f);
        
        if (Physics.CheckSphere(transform.localPosition + direction * 0.2f, 0.1f))
        {
            displacement = Vector3.zero;
        }
        
        transform.localScale = new Vector3(direction.x > 0 ? 1 : direction.x < 0 ? -1 : transform.localScale.x, 1, 1);

        if (displacement != Vector3.zero && _isIdle)
        {
            _animator.Play("Run");
            _isIdle = false;
        }
        else if (displacement == Vector3.zero && !_isIdle)
        {
            _animator.Play("Idle");
            _isIdle = true;
        }
        
        transform.localPosition += displacement;
    }

    private void LateUpdate()
    {
        if (_camera == null)
        {
            return;
        }
        _spriteRenderer.transform.rotation = _camera.transform.rotation;
        var curPos = _camera.transform.localPosition;
        var targetPos = new Vector3(transform.position.x, transform.position.y - 4f, curPos.z);
        _camera.transform.localPosition = Vector3.Lerp(curPos, targetPos, Time.deltaTime * 2f);
    }
}
