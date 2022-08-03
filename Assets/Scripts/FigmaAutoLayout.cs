using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace FigmaLoader
{
    public class FigmaAutoLayout : EditorWindow
    {
        private const string Token = "figd_Niah9mA9VNwIB7j_dqVQ8V8kiqs5NBRDqMdHM1Le";
        private const string URL = "https://api.figma.com/v1/files/";
        private const string FigmaToken = "X-Figma-Token";
        
        private string _fileKey = "tHYnGngWT4FBQH3gfAlZjP";

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
            response.Close();
        }
    }
}
