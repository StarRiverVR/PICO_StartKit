#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal.Internal;
using com.vivo.codelibrary;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.render
{
    public class UrpRenderUtil
    {
        static Mesh fullScreenPlantMesh;

        public static Mesh FullScreenPlantMesh
        {
            get
            {
                if (fullScreenPlantMesh==null)
                {
                    fullScreenPlantMesh = Resources.Load<Mesh>("FBX/FullScreenPlantMesh");
                }
                return fullScreenPlantMesh;
            }
        }

        public static int BaseColorID = Shader.PropertyToID(string.Intern("_BaseColor"));

        public static int BaseMapID = Shader.PropertyToID(string.Intern("_BaseMap"));

        //[MenuItem("Tools/JJJJJJ")]
        //static void CreateFullScreenMesh()
        //{
        //    Mesh mesh = new Mesh();

        //    Vector3 posLeftTop = new Vector3(-1f, 0, 1f);
        //    Vector3 posRightTop = new Vector3(1f, 0, 1f);
        //    Vector3 posRightDown = new Vector3(1f, 0, -1f);
        //    Vector3 posLeftDown = new Vector3(-1f, 0, -1f);

        //    Vector3[] vertexs = new Vector3[] { posLeftTop, posRightTop, posRightDown, posLeftDown };
        //    int[] triangs = new int[] { 0, 1, 2, 3, 0, 2 };
        //    Vector2[] uv = new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), Vector2.zero };

        //    mesh.vertices = vertexs;
        //    mesh.triangles = triangs;
        //    mesh.uv = uv;

        //    mesh.RecalculateBounds();
        //    mesh.RecalculateNormals();
        //    mesh.RecalculateTangents();

        //    AssetDatabase.CreateAsset(mesh, "Assets/KKLL.mesh");

        //}
    }
}


