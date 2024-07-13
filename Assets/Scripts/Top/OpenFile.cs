using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using SFB; //ダイアログ出す用
using System.IO;
using UnityEditor;
using System.Xml.Linq;
using System.Linq;
using System.Net.Sockets;


// コピペ元
// https://qiita.com/Butterfly-Dream/items/fab63700439f6b88d932

public class OpenFile : MonoBehaviour
{
    [SerializeField] GameObject _RocektObj;

    // ボタンが押された場合
    public void onClick() {

        // フィルタはここで追加操作する
        // new ExtensionFilter("表示名", "拡張子1", "拡張子2", ...)
        var extensions = new[] {
            new ExtensionFilter("OpenRocketファイル", "ork"),
        };

        // ダイアログを表示する
        // StandaloneFileBrowser.OpenFilePanel("タイトル", "最初のディレクトリ場所", [フィルタ設定], [複数選択可能かどうか(true or false)])
        string[] path = StandaloneFileBrowser.OpenFilePanel("orkファイルを選択", "", extensions, false);

        if (path.Length != 1 || path[0] == null) return; // 入力ファイルが不正
#if UNITY_EDITOR
        // コピー先の一時フォルダ
        var dest_path = FileUtil.GetUniqueTempPathInProject();
        Directory.CreateDirectory(dest_path);

        // zipファイルに拡張子変更・伸張
        FileUtil.CopyFileOrDirectory(path[0], dest_path + "/rocket.zip");
        System.IO.Compression.ZipFile.ExtractToDirectory(dest_path + "/rocket.zip", dest_path);
#else

        var dest_path = System.Environment.CurrentDirectory + "/temp";
        Directory.CreateDirectory(dest_path);
        Debug.Log(dest_path);

        File.Copy(path[0], dest_path + "/rocket.zip", false);

        System.IO.Compression.ZipFile.ExtractToDirectory(dest_path + "/rocket.zip", dest_path);
#endif

        // xmlファイル読み込み
        XDocument xml = XDocument.Load(dest_path + "/rocket.ork");


        // xmlファイル解析・データ取得
        ReadXmlData(xml);

        // 一時ファイルを忘れずに削除
        Directory.Delete(dest_path, true);
    }

    private void ReadXmlData(XDocument xmlOrkData)
    {
        try {
            // イニシャライズ
            DataManager.Instance.trajectory = new FlightData() { time = new List<float>(), coord = new List<Vector3>(), vel = new List<float>(), zenith = new List<float>(), azimuth = new List<float>() };
            DataManager.Instance.events = new List<Event>();
            DataManager.Instance.profile = new Profile();
            DataManager.Instance.rocket = new RocketStructure();

            var rocket = xmlOrkData.Element("openrocket").Element("rocket");

            var simulation = xmlOrkData.Element("openrocket").Element("simulations").Element("simulation");
            var condition = simulation.Element("conditions");
            var flightData = simulation.Element("flightdata");

            // 飛行プロファイル取得
            DataManager.Instance.profile.rocket_name = rocket.Element("name").Value;
            DataManager.Instance.profile.flight_time = float.Parse(flightData.Attribute("flighttime").Value);
            DataManager.Instance.profile.apogee = float.Parse(flightData.Attribute("maxaltitude").Value);
            DataManager.Instance.profile.max_vel = float.Parse(flightData.Attribute("maxvelocity").Value);
            DataManager.Instance.profile.angle = 90 - float.Parse(condition.Element("launchrodangle").Value); // 水平からの射角に変換
            DataManager.Instance.profile.direction = float.Parse(condition.Element("launchroddirection").Value);

            // イベント取得
            var events = flightData.Element("databranch").Elements("event");
            DataManager.Instance.events = new List<Event>();
            foreach (var event_data in events)
            {

                float time = float.Parse(event_data.Attribute("time").Value);
                string name = event_data.Attribute("type").Value;
                if (DataManager.Instance.events.Count > 0 &&
                    DataManager.Instance.events.Last().time == time)
                    continue; // 同じ時刻のデータは破棄
                DataManager.Instance.events.Add(new Event() { time = time, name = name });
            }

            // 飛行データ取得
            var datapoint = flightData.Element("databranch").Elements("datapoint");
            DataManager.Instance.trajectory = new FlightData() { time = new List<float>(), coord = new List<Vector3>(), vel = new List<float>(), zenith = new List<float>(), azimuth = new List<float>() };
            for (int i = 0; i < datapoint.Count(); i++)
            {
                var data = datapoint.ElementAt(i).Value.Split(","); // データはcsv形式なので , で分割
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
                // サブコンポーネントはあるか？ (フィン.etc.)
                if (stage.Element("bodytube").Elements("subcomponents").Count() == 0) return; // 無ければ読み取りを終了

                var bodySubComponents = stage.Element("bodytube").Element("subcomponents");
                float axialOffset = 0;
                string offsetMethod = "";
                ;                // 台形フィン
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
                // 楕円形フィン
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
                            -float.Parse(elm.Attribute("x").Value), // オプロケとは違い, 機首側を正
                            0));
                    }
                }

                Debug.Log(finPoints.Count());

                if (finPoints.Count() < 3) return; // finがないなら終了

                var _rootChord = Vector3.Distance(finPoints[0], finPoints.Last()); // フィン翼根長
                switch (offsetMethod)
                {
                    case "absolute": // 機体頂点から
                        axialOffset = -axialOffset + DataManager.Instance.rocket.noseLength + DataManager.Instance.rocket.bodyLength / 2.0f;
                        break;
                    case "top": // 親の上部から
                        axialOffset = -axialOffset + DataManager.Instance.rocket.bodyLength / 2.0f;
                        break;
                    case "middle": // 親中央から
                        axialOffset = -axialOffset + _rootChord / 2.0f;
                        break;

                    case "bottom": // 親の底部から
                        axialOffset = -axialOffset - DataManager.Instance.rocket.bodyLength / 2.0f + _rootChord;
                        break;
                    default: // middleも
                        axialOffset = -axialOffset; break;
                }
                Debug.Log(axialOffset);
                // 機軸法線方向に半径分だけオフセット
                Vector3 finOffset = new Vector3(DataManager.Instance.rocket.bodyDiameter / 2.0f, axialOffset, 0);
                for (int i = 0; i < finPoints.Count(); i++) finPoints[i] += finOffset;

                // 翼端を機体に埋め込むために伸ばす
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

    // 楕円形フィンの頂点を計算する関数.
    // 翼根機首側を原点として y 軸を機軸に, 機体法線方向を x 軸に採っている (底部・動径方向をそれぞれ正の方向とする).
    private List<Vector3> CalculateEllipticalFinPoints(float tipChord, float height, int nPoints = 10)
    {
        List<Vector3> points = new List<Vector3>();
        float a = tipChord / 2.0f; // 楕円の方程式のパラメタ
        float b = height; // 楕円の方程式のパラメタ
        float dx = 2 * a / (nPoints - 1); // x(機軸)方向の刻み幅
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
