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
using System;
using System.Collections.Generic;

namespace Klak.Osc
{
    public class MessageHandler
    {
        #region Delegate Classes

        /// Callback bound to a specific address
        public delegate void DataCallback(float data);

        /// Incoming message monitor delegate
        public delegate void MessageMonitor(string address, float data);

        #endregion

        #region Accessors

        public void AddDataCallback(string address, DataCallback callback)
        {
            lock (_delegatesLock)
            {
                if (_dataCallbackMap.ContainsKey(address))
                    _dataCallbackMap[address] += callback;
                else
                    _dataCallbackMap[address] = callback;
            }
        }

        public void RemoveDataCallback(string address, DataCallback callback)
        {
            lock (_delegatesLock)
            {
                var temp = _dataCallbackMap[address] - callback;
                if (temp != null)
                    _dataCallbackMap[address] = temp;
                else
                    _dataCallbackMap.Remove(address);
            }
        }

        public void AddMessageMonitor(MessageMonitor monitor)
        {
            lock (_delegatesLock) _messageMonitor += monitor;
        }

        public void RemoveMessageMonitor(MessageMonitor monitor)
        {
            lock (_delegatesLock) _messageMonitor -= monitor;
        }

        #endregion

        #region Handler Invokation

        public void ProcessMessage(string address, float data)
        {
            lock (_delegatesLock)
            {
                DataCallback callback;
                if (_dataCallbackMap.TryGetValue(address, out callback))
                    callback(data);

                if (_messageMonitor != null)
                    _messageMonitor(address, data);
            }
        }

        #endregion

        #region Private Objects

        Dictionary<string, DataCallback>
            _dataCallbackMap = new Dictionary<string, DataCallback>();

        MessageMonitor _messageMonitor;

        Object _delegatesLock = new Object();

        #endregion
    }
}
