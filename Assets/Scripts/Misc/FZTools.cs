using UnityEngine;
using System.Collections;

public static class FZTools
{
    public static string WriteVector(Vector3 _vector)
    {
        return "(" + _vector.x + ", " + _vector.y + ", " + _vector.z + ")";
    }

    public static void SetRotation(Transform _trans, float _rotation)
    {
        _trans.localRotation = Quaternion.Euler(new Vector3(0f, _rotation, 0f));
    }

    public static void LookAt2D(Transform _source, Transform _target)
    {
        Quaternion rotation = Quaternion.LookRotation(_target.position - _source.position, _source.TransformDirection(Vector3.up));
        _source.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
    }


    public static Ray GetDirectionRay(Vector3 _origin, Vector3 _target)
    {
        Vector3 direction = (_target - _origin).normalized;

        return new Ray(_origin, direction);
    }

    public static Vector3 GetDirectionNormalized(Vector3 _origin, Vector3 _target)
    {
        Vector3 direction = (_target - _origin).normalized;

        return new Vector3(direction.x, direction.y, direction.z);
    }

    public static void DebugRay(Vector3 _origin, Vector3 _target, float _height, Color _color, float _duration)
    {
        Vector3 direction = _target - _origin;
        _origin.y = _height;
        direction.y = _height;

        Debug.DrawRay(_origin, direction, _color, _duration);
    }

    public static void DebugLine(Vector3 _origin, Vector3 _target, float _height, Color _color, float _duration)
    {
        Vector3 direction = _target - _origin;
        _origin.y = _height;
        direction.y = _height;

        Debug.DrawLine(_origin, _origin + direction, _color, _duration);
    }

    public static void DebugLine(Ray _ray, float _height, Color _color, float _duration)
    {
        _ray.origin = new Vector3(_ray.origin.x, _height, _ray.origin.z);
        _ray.direction = new Vector3(_ray.direction.x, _height, _ray.direction.z);

        Debug.DrawLine(_ray.origin, _ray.origin + _ray.direction, _color, _duration);
    }
}
