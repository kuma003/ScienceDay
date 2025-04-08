using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject nose;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject fin;
    [SerializeField] private Material finMaterial;

    [SerializeField] private GameObject nozzle; 

    private Nozzle nozzleBuff;


    private Vector3 nosePosition = Vector3.zero;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private RocketStructure _RocketStructureBuff = new RocketStructure();

    private float _Zenith = -30f;
    public float zenith
    {
        set 
        {
            if (value is float.NaN) return;
            _Zenith = value;
            RotateComponets();
        }
        get { return _Zenith; }
    }
    private float _Azimuth = 0; // 方位角 (東 = x が0)
    public float azimuth
    {
        set
        {
            if (value is float.NaN) return;
            _Azimuth = value;
            RotateComponets();
        }
        get { return _Azimuth; }
    }

    private void Start()
    { 
        meshRenderer = fin.AddComponent<MeshRenderer>();
        meshFilter = fin.AddComponent<MeshFilter>();
        if (finMaterial != null )
            meshRenderer.material = finMaterial;

        nozzleBuff = nozzle.GetComponent<Nozzle>();

    }
    void Update()
    {
        var rocket = DataManager.Instance.rocket;
        // ロケットの構造データが変更されたら再度作成
        if (!_RocketStructureBuff.Equals(rocket))
        {
            _RocketStructureBuff = rocket;
            MakeRocket();
            RotateComponets();
        }
    }





    public void MakeRocket()
    {
        var rocket = DataManager.Instance.rocket; // ロケットの構造
        gameObject.SetActive(true);

        // ノーズのモデルは高さ 1 [m], 底面直径 1 [m]. 原点は底面中心.
        nose.transform.localScale = new Vector3(rocket.bodyDiameter, rocket.noseLength, rocket.bodyDiameter);
        nosePosition = new Vector3(0, rocket.bodyLength/2.0f, 0); // ボディ分だけオフセット. ポジションはRotateComponent内部で指定する.
        nose.transform.SetParent(gameObject.transform);

        // ボディ (円柱) は高さが 2 [m], 直径 1 [m]. オブジェクト原点をローカル原点に揃える.
        body.transform.localScale = new Vector3(rocket.bodyDiameter, rocket.bodyLength / 2.0f, rocket.bodyDiameter);
        body.transform.SetParent(gameObject.transform);

        // フィン
        if (rocket.finPoints is null) return;

        var mesh = new Mesh();

        var vertList = new List<Vector3> (); // 頂点リスト
        var idxList = new List<int>() ;

        var angle = 360 / rocket.finCount;
        // 適当な角度回転させるためのオフセット (恣意的)
        float angleOffset = 0;
        if (rocket.finCount == 3) angleOffset = 180; // 画面右手にフィンが見えた方がかっこよさげ
        else if (rocket.finCount == 4) angleOffset = 45; // フィンは左右対称の方がいいよね


        for (int i = 0; i < rocket.finCount; i++)
        {
            var tempVerts = 
                Array.ConvertAll(rocket.finPoints, vert => Quaternion.AngleAxis(i * angle + angleOffset, Vector3.up) * vert); // 頂点を回転

            /* TODO : 頂点付けを工夫する */
            vertList.AddRange(tempVerts);
            var firstIdx = rocket.finPoints.Length * i; // tempVertの先頭の頂点のインデックス
            for (int j = 2; j < tempVerts.Length; j++)
            {
                idxList.AddRange(new[] { firstIdx, firstIdx + j - 1, firstIdx + j });
            } // 頂点リスト

        }

        mesh.


        mesh.SetVertices(vertList);
        mesh.SetIndices(idxList, MeshTopology.Triangles, 0);
        meshFilter.mesh = mesh;
    }


    // 各部を回転させる
    private void RotateComponets()
    {
        Quaternion rotation = Quaternion.AngleAxis(azimuth, Vector3.up) * Quaternion.AngleAxis(-90+zenith, Vector3.forward);
        nose.transform.transform.position = gameObject.transform.position + rotation * nosePosition;
        nose.transform.rotation = rotation;

        body.transform.transform.position = gameObject.transform.position;
        body.transform.rotation = rotation;

        fin.transform.transform.position = gameObject.transform.position;
        fin.transform.rotation = rotation;

        nozzle.transform.transform.position = gameObject.transform.position - (rotation * nosePosition);
    }

    public void SetThrust(float thrust)
    {
        nozzleBuff.thrust = thrust;
    }

    
}
