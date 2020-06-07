﻿using UnityEngine;
using UnityEngine.U2D;

#if UNITY_EDITOR

using UnityEditor;

#endif

// Demo Script Usage:
//   When you want multiple SpriteShapes to share a common Spline,
//   attach this script to the secondary objects you would like to
//   copy the Spline and set the ParentObject to the original object
//   you are copying from.

[ExecuteInEditMode]
public class ConformingSpline : MonoBehaviour
{
    public GameObject m_ParentObject;
    private int hashCode;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_ParentObject != null)
        {
            hashCode = CopySpline(m_ParentObject, gameObject, hashCode);
        }
    }

    private static int CopySpline(GameObject src, GameObject dst, int hashCode)
    {
#if UNITY_EDITOR
        var parentSpriteShapeController = src.GetComponent<SpriteShapeController>();
        var mirrorSpriteShapeController = dst.GetComponent<SpriteShapeController>();

        if (parentSpriteShapeController != null && mirrorSpriteShapeController != null && parentSpriteShapeController.spline.GetHashCode() != hashCode)
        {
            SerializedObject srcController = new SerializedObject(parentSpriteShapeController);
            SerializedObject dstController = new SerializedObject(mirrorSpriteShapeController);
            SerializedProperty srcSpline = srcController.FindProperty("m_Spline");
            dstController.CopyFromSerializedProperty(srcSpline);
            dstController.ApplyModifiedProperties();
            EditorUtility.SetDirty(mirrorSpriteShapeController);
            return parentSpriteShapeController.spline.GetHashCode();
        }
#endif
        return hashCode;
    }
}