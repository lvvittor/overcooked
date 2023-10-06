using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CmdSlerp : ICommand {

    private Transform _transform;
    private Vector3 _direction;
    private float _speed;

    public CmdSlerp(Transform transform, Vector3 direction, float speed) {
        _transform = transform;
        _direction = direction;
        _speed = speed;
    }

    public void Do() {
        _transform.forward = Vector3.Slerp(_transform.forward, _direction, _speed * Time.deltaTime);
    }

}
