using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float ViewRadius;
    [Range(0, 360)]
    public float ViewAngle;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public List<Transform> VisibleTargets = new List<Transform>();

    private WaitForSeconds _delay;

    public float MeshResolution;

    private ViewCastInfo _viewCastInfo;


    public MeshFilter ViewMeshFilter;
    private Mesh _viewMesh;

    private void Start()
    {

        _viewMesh = new Mesh();
        _viewMesh.name = "View Mesh";

        ViewMeshFilter.mesh = _viewMesh;

        StartCoroutine("FindVisibleTargetsWithDelay", .2f);
    }

    private void LateUpdate()
    {
        DrawFieldOfView();
    }

    IEnumerator FindVisibleTargetsWithDelay(float delay)
    {

        _delay = new WaitForSeconds(delay);
        while (true)
        {

            yield return _delay;
            FindVisibleTargets();
        }
    }

    private void FindVisibleTargets()
    {
        VisibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, ViewRadius, TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < ViewAngle / 2)
            {
                float dist = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dist, ObstacleMask))
                {
                    VisibleTargets.Add(target);
                }

            }
        }

    }

    private void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
        float stepAngleSize = ViewAngle / stepCount;

        List<Vector3> viewPoints = new List<Vector3>();

        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;

            //Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * ViewRadius, Color.red);

            ViewCastInfo newCast = ViewCast(angle);
            viewPoints.Add(newCast.Point);
        }

        int vertexCount = viewPoints.Count + 1;


        Vector3[] vertices = new Vector3[vertexCount];

        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {

            if (i < vertexCount - 2)
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }

        }

        _viewMesh.Clear();

        _viewMesh.vertices = vertices;

        _viewMesh.triangles = triangles;

        _viewMesh.RecalculateNormals();

    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, ViewRadius, ObstacleMask))
        {
            _viewCastInfo.Hit = true;
            _viewCastInfo.Dist = hit.distance;
            _viewCastInfo.Angle = globalAngle;
            _viewCastInfo.Point = hit.point;
            return _viewCastInfo;
        }
        else
        {

            _viewCastInfo.Hit = false;
            _viewCastInfo.Dist = ViewRadius;
            _viewCastInfo.Angle = globalAngle;
            _viewCastInfo.Point = transform.position + dir * ViewRadius;

            return _viewCastInfo;
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {

        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }



}
