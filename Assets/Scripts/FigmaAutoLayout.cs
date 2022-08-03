using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEditor;
using UnityEngine;

namespace FigmaLoader
{
    public class FigmaAutoLayout : EditorWindow
    {
        private const string Token = "figd_Niah9mA9VNwIB7j_dqVQ8V8kiqs5NBRDqMdHM1Le";
        private const string URL = "https://api.figma.com/v1/files/";
        private const string FigmaToken = "X-Figma-Token";

        private File _file;
        private string _fileKey = "n94eWohksh0b0RxB5L90zZ";

        [MenuItem("FigmaAutoLayout/Show")]
        public static void ShowWindow()
        {
            FigmaAutoLayout window = (FigmaAutoLayout) GetWindow(typeof(FigmaAutoLayout));
            window.Show();
        }

        private void OnGUI()
        {
            _fileKey = EditorGUILayout.TextField("File key", _fileKey);

            if (GUILayout.Button("Get file"))
            {
                GetFile();
                CreatePrefab();
            }
        }

        private void GetFile()
        {
            WebRequest request = WebRequest.Create(URL + _fileKey);
            request.Headers[FigmaToken] = Token;
            
            WebResponse response = request.GetResponse();
            string json = "";
            
            using (Stream stream = response.GetResponseStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        json += line;
                    }
                }
            }
            
            Debug.Log(json);
            _file = JsonConvert.DeserializeObject<File>(json);
            response.Close();
        }

        private void CreatePrefab()
        {
            Layer frame = _file.Document.Children[0].Children[0];

            GameObject frameObject = new GameObject {name = frame.Name};
            RectTransform frameRectTransform = frameObject.AddComponent<RectTransform>();
            
            frameRectTransform.pivot = new Vector2(0, 1);
            frameRectTransform.anchorMax = new Vector2(0, 1);
            frameRectTransform.anchorMin = new Vector2(0, 1);
            frameRectTransform.anchoredPosition = Vector2.zero;
            frameRectTransform.sizeDelta =
                new Vector2(frame.AbsoluteBoundingBox.Width, frame.AbsoluteBoundingBox.Height);

            foreach (var child in frame.Children)
            {
                CreateLayer(child, frame, frameRectTransform);
            }
            
            if (!Directory.Exists("Assets/Prefabs"))
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            string localPath = "Assets/Prefabs/" + frame.Name + ".prefab";
            
            PrefabUtility.SaveAsPrefabAsset(frameObject, localPath);

        }

        private void CreateLayer(Layer layer, Layer frame, Transform parent)
        {
            GameObject gameObject = new GameObject {name = frame.Name};
            RectTransform rectTransform = gameObject.AddComponent<RectTransform>();

            var x = layer.AbsoluteBoundingBox.X - frame.AbsoluteBoundingBox.X;
            var y = frame.AbsoluteBoundingBox.Y - layer.AbsoluteBoundingBox.Y;
            
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(x, y);
            rectTransform.sizeDelta =
                new Vector2(layer.AbsoluteBoundingBox.Width, layer.AbsoluteBoundingBox.Height);
            
            rectTransform.SetParent(parent, true);

            if (layer.Children == null || layer.Children.Length == 0)
                return;

            foreach (var child in layer.Children)
            {
                CreateLayer(child, frame, rectTransform);
            }
        }

        private string GetPrefabPath(string name) => $"Assets/{name}.prefab";
    }
}
