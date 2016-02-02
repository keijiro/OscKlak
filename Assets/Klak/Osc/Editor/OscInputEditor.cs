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
        SerializedProperty _inputCurve;

        SerializedProperty _eventType;
        SerializedProperty _triggerType;
        SerializedProperty _outputValue0;
        SerializedProperty _outputValue1;
        SerializedProperty _interpolator;

        SerializedProperty _triggerEvent;
        SerializedProperty _gateOnEvent;
        SerializedProperty _gateOffEvent;
        SerializedProperty _valueEvent;

        void OnEnable()
        {
            _address = serializedObject.FindProperty("_address");
            _inputCurve = serializedObject.FindProperty("_inputCurve");

            _eventType = serializedObject.FindProperty("_eventType");
            _triggerType = serializedObject.FindProperty("_triggerType");
            _outputValue0 = serializedObject.FindProperty("_outputValue0");
            _outputValue1 = serializedObject.FindProperty("_outputValue1");
            _interpolator = serializedObject.FindProperty("_interpolator");

            _triggerEvent = serializedObject.FindProperty("_triggerEvent");
            _gateOnEvent = serializedObject.FindProperty("_gateOnEvent");
            _gateOffEvent = serializedObject.FindProperty("_gateOffEvent");
            _valueEvent = serializedObject.FindProperty("_valueEvent");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_address);
            EditorGUILayout.PropertyField(_inputCurve);

            EditorGUILayout.Space();

            var showAllOptions = _eventType.hasMultipleDifferentValues;
            var eventType = (OscInput.EventType)_eventType.enumValueIndex;

            EditorGUILayout.PropertyField(_eventType);

            if (showAllOptions || eventType == OscInput.EventType.Trigger)
            {
                EditorGUILayout.PropertyField(_triggerType);
                EditorGUILayout.PropertyField(_triggerEvent);
            }

            if (showAllOptions || eventType == OscInput.EventType.Gate)
            {
                EditorGUILayout.PropertyField(_gateOnEvent);
                EditorGUILayout.PropertyField(_gateOffEvent);
            }

            if (showAllOptions || eventType == OscInput.EventType.Value)
            {
                EditorGUILayout.PropertyField(_outputValue0);
                EditorGUILayout.PropertyField(_outputValue1);
                EditorGUILayout.PropertyField(_interpolator);
                EditorGUILayout.PropertyField(_valueEvent);
            }

            if (EditorApplication.isPlaying &&
                !serializedObject.isEditingMultipleObjects)
            {
                var instance = (OscInput)target;
                instance.debugInput =
                    EditorGUILayout.Slider("Debug", instance.debugInput, 0, 1);
                EditorUtility.SetDirty(target); // request repaint
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
