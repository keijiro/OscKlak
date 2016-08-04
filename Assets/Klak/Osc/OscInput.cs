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
using System.Collections.Generic;
using Klak.Math;
using Klak.Wiring;

namespace Klak.Osc
{
    [AddComponentMenu("Klak/Wiring/Input/OSC Input")]
    public class OscInput : NodeBase
    {
        #region Editable properties

        [SerializeField]
        string _address = "/1/fader1";

        [SerializeField]
        FloatInterpolator.Config _interpolator = new FloatInterpolator.Config(
            FloatInterpolator.Config.InterpolationType.DampedSpring, 30
        );

        #endregion

        #region Node I/O

        [SerializeField, Outlet]
        VoidEvent _bangEvent = new VoidEvent();

        [SerializeField, Outlet]
        VoidEvent _onEvent = new VoidEvent();

        [SerializeField, Outlet]
        VoidEvent _offEvent = new VoidEvent();

        [SerializeField, Outlet]
        FloatEvent _valueEvent = new FloatEvent();

        #endregion

        #region Private members

        enum EventRecord { Bang, On, Off }

        const float _threshold = 0.5f;

        string _registeredAddress;

        Queue<EventRecord> _eventQueue;

        FloatInterpolator _value;
        float _lastInputValue;

        bool CheckUpTrigger(float inputValue)
        {
            return
                _threshold >= _lastInputValue && _threshold < inputValue;
        }

        bool CheckDownTrigger(float inputValue)
        {
            return
                _threshold < _lastInputValue && _threshold >= inputValue;
        }

        void OscDataCallback(float inputValue)
        {
            _value.targetValue = inputValue;

            lock (_eventQueue) _eventQueue.Enqueue(EventRecord.Bang);

            if (CheckUpTrigger(inputValue))
                lock (_eventQueue) _eventQueue.Enqueue(EventRecord.On);
            else if (CheckDownTrigger(inputValue))
                lock (_eventQueue) _eventQueue.Enqueue(EventRecord.Off);

            _lastInputValue = inputValue;
        }

        #endregion

        #region MonoBehaviour functions

        void Awake()
        {
            _value = new FloatInterpolator(0, _interpolator);
            _eventQueue = new Queue<EventRecord>();
        }

        void OnEnable()
        {
            // Register the OSC data callback.
            if (!string.IsNullOrEmpty(_address))
                OscMaster.messageHandler.AddDataCallback(_address, OscDataCallback);
            _registeredAddress = _address;
        }

        void OnDisable()
        {
            // Unregister the OSC data callback.
            if (!string.IsNullOrEmpty(_registeredAddress))
                OscMaster.messageHandler.RemoveDataCallback(_registeredAddress, OscDataCallback);
            _registeredAddress = null;
        }

        void Update()
        {
            // Invoke events in the queue.
            lock (_eventQueue)
                while (_eventQueue.Count > 0)
                    switch (_eventQueue.Dequeue()) {
                        case EventRecord.Bang: _bangEvent.Invoke(); break;
                        case EventRecord.On:   _onEvent.  Invoke(); break;
                        case EventRecord.Off:  _offEvent. Invoke(); break;
                    }

            // Value interpolation and invokation.
            _valueEvent.Invoke(_value.Step());

            #if UNITY_EDITOR

            // Re-register the osc data callback if the address was changed.
            if (_address != _registeredAddress) {
                OnDisable();
                OnEnable();
            }

            #endif
        }

        #endregion

        #if UNITY_EDITOR

        #region Editor Interface

        public float debugInput {
            get { return _lastInputValue; }
            set { OscDataCallback(value); }
        }

        public void DebugBang()
        {
            _bangEvent.Invoke();
        }

        #endregion

        #endif
    }
}
