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
    private float _Azimuth = 0; // ���ʊp (�� = x ��0)
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
        // ���P�b�g�̍\���f�[�^���ύX���ꂽ��ēx�쐬
        if (!_RocketStructureBuff.Equals(rocket))
        {
            _RocketStructureBuff = rocket;
            MakeRocket();
            RotateComponets();
        }
    }





    public void MakeRocket()
    {
        var rocket = DataManager.Instance.rocket; // ���P�b�g�̍\��
        gameObject.SetActive(true);

        // �m�[�Y�̃��f���͍��� 1 [m], ��ʒ��a 1 [m]. ���_�͒�ʒ��S.
        nose.transform.localScale = new Vector3(rocket.bodyDiameter, rocket.noseLength, rocket.bodyDiameter);
        nosePosition = new Vector3(0, rocket.bodyLength/2.0f, 0); // �{�f�B�������I�t�Z�b�g. �|�W�V������RotateComponent�����Ŏw�肷��.
        nose.transform.SetParent(gameObject.transform);

        // �{�f�B (�~��) �͍����� 2 [m], ���a 1 [m]. �I�u�W�F�N�g���_�����[�J�����_�ɑ�����.
        body.transform.localScale = new Vector3(rocket.bodyDiameter, rocket.bodyLength / 2.0f, rocket.bodyDiameter);
        body.transform.SetParent(gameObject.transform);

        // �t�B��
        if (rocket.finPoints is null) return;

        var mesh = new Mesh();

        var vertList = new List<Vector3> (); // ���_���X�g
        var idxList = new List<int>() ;

        var angle = 360 / rocket.finCount;
        // �K���Ȋp�x��]�����邽�߂̃I�t�Z�b�g (���ӓI)
        float angleOffset = 0;
        if (rocket.finCount == 3) angleOffset = 180; // ��ʉE��Ƀt�B���������������������悳��
        else if (rocket.finCount == 4) angleOffset = 45; // �t�B���͍��E�Ώ̂̕����������


        for (int i = 0; i < rocket.finCount; i++)
        {
            var tempVerts = 
                Array.ConvertAll(rocket.finPoints, vert => Quaternion.AngleAxis(i * angle + angleOffset, Vector3.up) * vert); // ���_����]

            /* TODO : ���_�t�����H�v���� */
            vertList.AddRange(tempVerts);
            var firstIdx = rocket.finPoints.Length * i; // tempVert�̐擪�̒��_�̃C���f�b�N�X
            for (int j = 2; j < tempVerts.Length; j++)
            {
                idxList.AddRange(new[] { firstIdx, firstIdx + j - 1, firstIdx + j });
            } // ���_���X�g

        }


        mesh.SetVertices(vertList);
        mesh.SetIndices(idxList, MeshTopology.Triangles, 0);
        meshFilter.mesh = mesh;
    }


    // �e������]������
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
