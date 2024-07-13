using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using SFB; //�_�C�A���O�o���p
using System.IO;
using UnityEditor;
using System.Xml.Linq;
using System.Linq;
using System.Net.Sockets;


// �R�s�y��
// https://qiita.com/Butterfly-Dream/items/fab63700439f6b88d932

public class OpenFile : MonoBehaviour
{
    [SerializeField] GameObject _RocektObj;

    // �{�^���������ꂽ�ꍇ
    public void onClick() {

        // �t�B���^�͂����Œǉ����삷��
        // new ExtensionFilter("�\����", "�g���q1", "�g���q2", ...)
        var extensions = new[] {
            new ExtensionFilter("OpenRocket�t�@�C��", "ork"),
        };

        // �_�C�A���O��\������
        // StandaloneFileBrowser.OpenFilePanel("�^�C�g��", "�ŏ��̃f�B���N�g���ꏊ", [�t�B���^�ݒ�], [�����I���\���ǂ���(true or false)])
        string[] path = StandaloneFileBrowser.OpenFilePanel("ork�t�@�C����I��", "", extensions, false);

        if (path.Length != 1 || path[0] == null) return; // ���̓t�@�C�����s��
#if UNITY_EDITOR
        // �R�s�[��̈ꎞ�t�H���_
        var dest_path = FileUtil.GetUniqueTempPathInProject();
        Directory.CreateDirectory(dest_path);

        // zip�t�@�C���Ɋg���q�ύX�E�L��
        FileUtil.CopyFileOrDirectory(path[0], dest_path + "/rocket.zip");
        System.IO.Compression.ZipFile.ExtractToDirectory(dest_path + "/rocket.zip", dest_path);
#else

        var dest_path = System.Environment.CurrentDirectory + "/temp";
        Directory.CreateDirectory(dest_path);
        Debug.Log(dest_path);

        File.Copy(path[0], dest_path + "/rocket.zip", false);

        System.IO.Compression.ZipFile.ExtractToDirectory(dest_path + "/rocket.zip", dest_path);
#endif

        // xml�t�@�C���ǂݍ���
        XDocument xml = XDocument.Load(dest_path + "/rocket.ork");


        // xml�t�@�C����́E�f�[�^�擾
        ReadXmlData(xml);

        // �ꎞ�t�@�C����Y�ꂸ�ɍ폜
        Directory.Delete(dest_path, true);
    }

    private void ReadXmlData(XDocument xmlOrkData)
    {
        try {
            // �C�j�V�����C�Y
            DataManager.Instance.trajectory = new FlightData() { time = new List<float>(), coord = new List<Vector3>(), vel = new List<float>(), zenith = new List<float>(), azimuth = new List<float>() };
            DataManager.Instance.events = new List<Event>();
            DataManager.Instance.profile = new Profile();
            DataManager.Instance.rocket = new RocketStructure();

            var rocket = xmlOrkData.Element("openrocket").Element("rocket");

            var simulation = xmlOrkData.Element("openrocket").Element("simulations").Element("simulation");
            var condition = simulation.Element("conditions");
            var flightData = simulation.Element("flightdata");

            // ��s�v���t�@�C���擾
            DataManager.Instance.profile.rocket_name = rocket.Element("name").Value;
            DataManager.Instance.profile.flight_time = float.Parse(flightData.Attribute("flighttime").Value);
            DataManager.Instance.profile.apogee = float.Parse(flightData.Attribute("maxaltitude").Value);
            DataManager.Instance.profile.max_vel = float.Parse(flightData.Attribute("maxvelocity").Value);
            DataManager.Instance.profile.angle = 90 - float.Parse(condition.Element("launchrodangle").Value); // ��������̎ˊp�ɕϊ�
            DataManager.Instance.profile.direction = float.Parse(condition.Element("launchroddirection").Value);

            // �C�x���g�擾
            var events = flightData.Element("databranch").Elements("event");
            DataManager.Instance.events = new List<Event>();
            foreach (var event_data in events)
            {

                float time = float.Parse(event_data.Attribute("time").Value);
                string name = event_data.Attribute("type").Value;
                if (DataManager.Instance.events.Count > 0 &&
                    DataManager.Instance.events.Last().time == time)
                    continue; // ���������̃f�[�^�͔j��
                DataManager.Instance.events.Add(new Event() { time = time, name = name });
            }

            // ��s�f�[�^�擾
            var datapoint = flightData.Element("databranch").Elements("datapoint");
            DataManager.Instance.trajectory = new FlightData() { time = new List<float>(), coord = new List<Vector3>(), vel = new List<float>(), zenith = new List<float>(), azimuth = new List<float>() };
            for (int i = 0; i < datapoint.Count(); i++)
            {
                var data = datapoint.ElementAt(i).Value.Split(","); // �f�[�^��csv�`���Ȃ̂� , �ŕ���
                float time = float.Parse(data[0]);
                var alt = float.Parse(data[1]);
                var east = float.Parse(data[6]);
                var north = float.Parse(data[7]);
                var vel = float.Parse(data[4]);
                var zenith = Mathf.Rad2Deg * float.Parse(data[46]);
                var azimuth = Mathf.Rad2Deg * float.Parse(data[47]);
                DataManager.Instance.trajectory.time.Add(time);
                DataManager.Instance.trajectory.coord.Add(new Vector3(east, alt, north));
                DataManager.Instance.trajectory.vel.Add(vel);
                DataManager.Instance.trajectory.zenith.Add(zenith);
                DataManager.Instance.trajectory.azimuth.Add(azimuth);
            }

            var stage = rocket.Element("subcomponents").Element("stage").Element("subcomponents");


            if (stage.Elements("nosecone").Count() > 0)
            {
                DataManager.Instance.rocket.noseLength =
                    float.Parse(stage.Element("nosecone").Element("length").Value);
            }

            if (stage.Elements("bodytube").Count() > 0)
            {
                DataManager.Instance.rocket.bodyLength =
                    float.Parse(stage.Element("bodytube").Element("length").Value);
                DataManager.Instance.rocket.bodyDiameter =
                    float.Parse(stage.Element("bodytube").Element("radius").Value);
                DataManager.Instance.rocket.finType = "";
                DataManager.Instance.rocket.finCount = 0;
                var finPoints = new List<Vector3>();
                // �T�u�R���|�[�l���g�͂��邩�H (�t�B��.etc.)
                if (stage.Element("bodytube").Elements("subcomponents").Count() == 0) return; // ������Γǂݎ����I��

                var bodySubComponents = stage.Element("bodytube").Element("subcomponents");
                float axialOffset = 0;
                string offsetMethod = "";
                ;                // ��`�t�B��
                if (bodySubComponents.Elements("trapezoidfinset").Count() > 0)
                {
                    DataManager.Instance.rocket.finType = "trapezoidfinset";
                    DataManager.Instance.rocket.finCount =
                        int.Parse(bodySubComponents.Element("trapezoidfinset").Element("fincount").Value);
                    float rootChord =
                        float.Parse(bodySubComponents.Element("trapezoidfinset").Element("rootchord").Value);
                    float tipChord =
                        float.Parse(bodySubComponents.Element("trapezoidfinset").Element("tipchord").Value);
                    float height =
                        float.Parse(bodySubComponents.Element("trapezoidfinset").Element("height").Value);
                    float sweepLength =
                        float.Parse(bodySubComponents.Element("trapezoidfinset").Element("sweeplength").Value);
                    axialOffset =
                        float.Parse(bodySubComponents.Element("trapezoidfinset").Element("axialoffset").Value);
                    offsetMethod = bodySubComponents.Element("trapezoidfinset").Element("axialoffset").Attribute("method").Value;
                    finPoints = new List<Vector3> {
                new Vector3(0, 0, 0),
                new Vector3(height, -sweepLength, 0),
                new Vector3(height, -sweepLength - tipChord, 0),
                new Vector3(0, -rootChord, 0)
                };
                    Debug.Log(finPoints);
                }
                // �ȉ~�`�t�B��
                else if (bodySubComponents.Elements("ellipticalfinset").Count() > 0)
                {
                    DataManager.Instance.rocket.finType = "ellipticalfinset";
                    DataManager.Instance.rocket.finCount =
                        int.Parse(bodySubComponents.Element("ellipticalfinset").Element("fincount").Value);
                    float rootChord =
                        float.Parse(bodySubComponents.Element("ellipticalfinset").Element("rootchord").Value);
                    float height =
                        float.Parse(bodySubComponents.Element("ellipticalfinset").Element("height").Value);
                    axialOffset =
                        float.Parse(bodySubComponents.Element("ellipticalfinset").Element("axialoffset").Value);
                    offsetMethod = bodySubComponents.Element("ellipticalfinset").Element("axialoffset").Attribute("method").Value;
                    finPoints = CalculateEllipticalFinPoints(rootChord, height);
                    Debug.Log(finPoints);
                }
                else if (bodySubComponents.Elements("freeformfinset").Count() > 0)
                {
                    DataManager.Instance.rocket.finType = "freeformfinset";
                    DataManager.Instance.rocket.finCount =
                        int.Parse(bodySubComponents.Element("freeformfinset").Element("fincount").Value);
                    axialOffset =
                        float.Parse(bodySubComponents.Element("freeformfinset").Element("axialoffset").Value);
                    offsetMethod = bodySubComponents.Element("freeformfinset").Element("axialoffset").Attribute("method").Value;
                    foreach (var elm in bodySubComponents.Element("freeformfinset").Element("finpoints").Elements("point"))
                    {
                        finPoints.Add(new Vector3(
                            float.Parse(elm.Attribute("y").Value),
                            -float.Parse(elm.Attribute("x").Value), // �I�v���P�Ƃ͈Ⴂ, �@�񑤂�
                            0));
                    }
                }

                Debug.Log(finPoints.Count());

                if (finPoints.Count() < 3) return; // fin���Ȃ��Ȃ�I��

                var _rootChord = Vector3.Distance(finPoints[0], finPoints.Last()); // �t�B��������
                switch (offsetMethod)
                {
                    case "absolute": // �@�̒��_����
                        axialOffset = -axialOffset + DataManager.Instance.rocket.noseLength + DataManager.Instance.rocket.bodyLength / 2.0f;
                        break;
                    case "top": // �e�̏㕔����
                        axialOffset = -axialOffset + DataManager.Instance.rocket.bodyLength / 2.0f;
                        break;
                    case "middle": // �e��������
                        axialOffset = -axialOffset + _rootChord / 2.0f;
                        break;

                    case "bottom": // �e�̒ꕔ����
                        axialOffset = -axialOffset - DataManager.Instance.rocket.bodyLength / 2.0f + _rootChord;
                        break;
                    default: // middle��
                        axialOffset = -axialOffset; break;
                }
                Debug.Log(axialOffset);
                // �@���@�������ɔ��a�������I�t�Z�b�g
                Vector3 finOffset = new Vector3(DataManager.Instance.rocket.bodyDiameter / 2.0f, axialOffset, 0);
                for (int i = 0; i < finPoints.Count(); i++) finPoints[i] += finOffset;

                // ���[���@�̂ɖ��ߍ��ނ��߂ɐL�΂�
                finPoints.Insert(0, finPoints[0] - finOffset.x * Vector3.right);
                finPoints.Add(finPoints.Last() - finOffset.x * Vector3.right);

                DataManager.Instance.rocket.finPoints = finPoints.ToArray();
            }
        }
        catch
        {
            return;
        }
        }

    // �ȉ~�`�t�B���̒��_���v�Z����֐�.
    // �����@�񑤂����_�Ƃ��� y �����@����, �@�̖@�������� x ���ɍ̂��Ă��� (�ꕔ�E���a���������ꂼ�ꐳ�̕����Ƃ���).
    private List<Vector3> CalculateEllipticalFinPoints(float tipChord, float height, int nPoints = 10)
    {
        List<Vector3> points = new List<Vector3>();
        float a = tipChord / 2.0f; // �ȉ~�̕������̃p�����^
        float b = height; // �ȉ~�̕������̃p�����^
        float dx = 2 * a / (nPoints - 1); // x(�@��)�����̍��ݕ�
        for (int i = 0; i < nPoints; i++)
        {
            points.Add(new Vector3(
                b * Mathf.Sqrt(Mathf.Abs(1 - Mathf.Pow((i * dx - a) / a, 2))),
                -i * dx, 
                0));
        }
        return points;
    }
}   
