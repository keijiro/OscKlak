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

namespace Klak.Osc
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(OscInput))]
    public class OscInputEditor : Editor
    {
        SerializedProperty _address;
        SerializedProperty _interpolator;

        SerializedProperty _bangEvent;
        SerializedProperty _onEvent;
        SerializedProperty _offEvent;
        SerializedProperty _valueEvent;

        void OnEnable()
        {
            _address = serializedObject.FindProperty("_address");
            _interpolator = serializedObject.FindProperty("_interpolator");

            _bangEvent = serializedObject.FindProperty("_bangEvent");
            _onEvent = serializedObject.FindProperty("_onEvent");
            _offEvent = serializedObject.FindProperty("_offEvent");
            _valueEvent = serializedObject.FindProperty("_valueEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_address);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_interpolator);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_bangEvent);
            EditorGUILayout.PropertyField(_onEvent);
            EditorGUILayout.PropertyField(_offEvent);
            EditorGUILayout.PropertyField(_valueEvent);

            if (EditorApplication.isPlaying &&
                !serializedObject.isEditingMultipleObjects)
            {
                var instance = (OscInput)target;

                instance.debugInput = EditorGUILayout.
                    Slider("Debug", instance.debugInput, 0, 1);

                if (GUILayout.Button("Bang")) instance.DebugBang();

                EditorUtility.SetDirty(target); // request repaint
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
