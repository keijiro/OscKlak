//
// OscKlak - OSC (Open Sound Control) extension for Klak
//
// Copyright (C) 2016 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Klak.Osc
{
    /// OSC message monitor window
    class OscMonitorWindow : EditorWindow
    {
        Dictionary<string, float> _dataMap;
        bool _updated;

        [MenuItem("Window/OSC Monitor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<OscMonitorWindow>("OSC Monitor");
        }

        void OnEnable()
        {
            _dataMap = new Dictionary<string, float>();
            OscMaster.messageHandler.AddMessageMonitor(OnProcessMessage);
        }

        void OnDisable()
        {
            OscMaster.messageHandler.RemoveMessageMonitor(OnProcessMessage);
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            lock (_dataMap)
                foreach (var pair in _dataMap)
                    EditorGUILayout.LabelField(pair.Key, pair.Value.ToString());

            EditorGUILayout.EndVertical();
        }

        void Update()
        {
            if (_updated) {
                Repaint();
                _updated = false;
            }
        }

        void OnProcessMessage(string address, float data)
        {
            lock (_dataMap) _dataMap[address] = data;
            _updated = true;
        }
    }
}
