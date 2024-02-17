using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]private float rotateSpeed = 1f; // 旋转速度
    private Vector3 _offset; // 摄像机与玩家的偏移量
    
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
        
        _offset = _camera.transform.position - transform.position; // 计算摄像机与玩家的初始偏移量
    }

    private void Update()
    {
        var horizontalInput = Input.GetAxisRaw("Horizontal");
        var verticalInput = Input.GetAxisRaw("Vertical");
        var forward = new Vector2(_camera.transform.forward.x, _camera.transform.forward.y).normalized;
        var right = new Vector2(_camera.transform.right.x, _camera.transform.right.y).normalized;
        var direction = new Vector3(horizontalInput * right.x + verticalInput * forward.x, horizontalInput * right.y + verticalInput * forward.y, 0f).normalized;
        var displacement = direction * (Time.deltaTime * 3f);
        
        if (Physics.CheckSphere(transform.localPosition + direction * 0.2f, 0.1f))
        {
            displacement = Vector3.zero;
        }

        transform.localScale = new Vector3(horizontalInput > 0 ? 1 : horizontalInput < 0 ? -1 : transform.localScale.x, transform.localScale.y, transform.localScale.z);

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
        
        if (Input.GetMouseButton(1)) // 如果按住右键
        {
            float horizontalInput = Input.GetAxisRaw("Mouse X"); // 获取鼠标水平移动
            float verticalInput = Input.GetAxisRaw("Mouse Y"); // 获取鼠标垂直移动
        
            // 计算摄像机绕玩家旋转的目标位置
            Quaternion camTurnAngleHorizontal = Quaternion.AngleAxis(horizontalInput * rotateSpeed, Vector3.forward);
            Quaternion camTurnAngleVertical = Quaternion.AngleAxis(verticalInput * rotateSpeed, _camera.transform.right);
            Quaternion camTurnAngle = camTurnAngleHorizontal * camTurnAngleVertical;
            _offset = camTurnAngle * _offset;
        
            // 将摄像机绕玩家进行旋转
            _camera.transform.position = transform.position + _offset;
            _camera.transform.RotateAround(transform.position, transform.forward, horizontalInput * rotateSpeed);
            _camera.transform.RotateAround(transform.position, _camera.transform.right, verticalInput * rotateSpeed);
        }
        
        _spriteRenderer.transform.rotation = _camera.transform.rotation;
        _camera.transform.position = new Vector3(transform.position.x + _offset.x, transform.position.y + _offset.y, _camera.transform.position.z);
        // var curPos = _camera.transform.position;
        // var targetPos = transform.position;
        // _camera.transform.position = Vector3.Lerp(curPos, targetPos, Time.deltaTime * 2f);
    }
}
