using UnityEngine;

public struct ViewCastInfo
{
    public bool Hit;
    public Vector3 Point;
    public float Dist;
    public float Angle;

    public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle)
    {
        Hit = _hit;
        Point = _point;
        Dist = _dist;
        Angle = _angle;
    }
}