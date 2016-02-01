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
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Klak.Math;

namespace Klak.Osc
{
    [AddComponentMenu("Klak/OSC/OSC Input")]
    public class OscInput : MonoBehaviour
    {
        #region Nested Public Classes

        public enum EventType { Trigger, Gate, Value }

        public enum TriggerType { Bang, Threshold }

        [Serializable]
        public class ValueEvent : UnityEvent<float> {}

        #endregion

        #region Editable Properties

        [SerializeField]
        string _address = "/1/fader1";

        [SerializeField]
        AnimationCurve _inputCurve = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField]
        EventType _eventType = EventType.Value;

        [SerializeField]
        TriggerType _triggerType = TriggerType.Threshold;

        [SerializeField]
        float _outputValue0 = 0.0f;

        [SerializeField]
        float _outputValue1 = 1.0f;

        [SerializeField]
        FloatInterpolator.Config _interpolator;

        [SerializeField]
        UnityEvent _triggerEvent;

        [SerializeField]
        UnityEvent _gateOnEvent;

        [SerializeField]
        UnityEvent _gateOffEvent;

        [SerializeField]
        ValueEvent _valueEvent;

        #endregion

        #region Private Variables And Methods

        enum EventRecord { Trigger, GateOn, GateOff }

        const float _triggerThreshold = 0.5f;

        string _registeredAddress;

        Queue<EventRecord> _eventQueue;

        FloatInterpolator _value;
        float _lastInputValue;

        float CalculateTargetValue(float inputValue)
        {
            var p = _inputCurve.Evaluate(inputValue);
            return BasicMath.Lerp(_outputValue0, _outputValue1, p);
        }

        bool CheckUpTrigger(float inputValue)
        {
            return
                _triggerThreshold >= _lastInputValue &&
                _triggerThreshold < inputValue;
        }

        bool CheckDownTrigger(float inputValue)
        {
            return
                _triggerThreshold < _lastInputValue &&
                _triggerThreshold >= inputValue;
        }

        void OscDataCallback(float data)
        {
            UpdateState(data);
        }

        void UpdateState(float inputValue)
        {
            if (_eventType == EventType.Value)
            {
                _value.targetValue = CalculateTargetValue(inputValue);
            }
            else if (_eventType == EventType.Trigger)
            {
                if (_triggerType == TriggerType.Bang || CheckUpTrigger(inputValue))
                {
                    lock (_eventQueue) _eventQueue.Enqueue(EventRecord.Trigger);
                }
            }
            else // EventType.Gate
            {
                if (CheckUpTrigger(inputValue))
                {
                    lock (_eventQueue) _eventQueue.Enqueue(EventRecord.GateOn);
                }
                else if (CheckDownTrigger(inputValue))
                {
                    lock (_eventQueue) _eventQueue.Enqueue(EventRecord.GateOff);
                }
            }

            _lastInputValue = inputValue;
        }

        #endregion

        #region MonoBehaviour Functions

        void OnEnable()
        {
            // register the osc data callback
            if (!String.IsNullOrEmpty(_address))
                OscMaster.messageHandler.
                    AddDataCallback(_address, OscDataCallback);

            _registeredAddress = _address;
        }

        void OnDisable()
        {
            // unregister the osc data callback
            if (!String.IsNullOrEmpty(_registeredAddress))
                OscMaster.messageHandler.
                    RemoveDataCallback(_registeredAddress, OscDataCallback);

            _registeredAddress = null;
        }

        void Start()
        {
            _value = new FloatInterpolator(0, _interpolator);
            _eventQueue = new Queue<EventRecord>();
        }

        void Update()
        {
            // invoke events in the queue
            lock (_eventQueue)
                while (_eventQueue.Count > 0)
                    switch (_eventQueue.Dequeue()) {
                        case EventRecord.Trigger: _triggerEvent.Invoke(); break;
                        case EventRecord.GateOn:  _gateOnEvent. Invoke(); break;
                        case EventRecord.GateOff: _gateOffEvent.Invoke(); break;
                    }

            // value interpolation and invokation
            if (_eventType == EventType.Value)
                _valueEvent.Invoke(_value.Step());

            #if UNITY_EDITOR

            // re-register the osc data callback if the address was changed
            if (_address != _registeredAddress)
            {
                OnDisable();
                OnEnable();
            }

            #endif
        }

        #endregion
    }
}
