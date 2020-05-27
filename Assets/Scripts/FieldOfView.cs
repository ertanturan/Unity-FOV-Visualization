﻿using System.Collections;
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

    private void Start()
    {
        StartCoroutine("FindVisibleTargetsWithDelay", .2f);
    }

    private void Update()
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


        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - ViewAngle / 2 + stepAngleSize * i;

            Debug.DrawLine(transform.position, transform.position + DirFromAngle(angle, true) * ViewRadius, Color.red);
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
