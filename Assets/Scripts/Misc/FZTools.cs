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
}
