OscKlak
=======

*OscKlak* is an extension for [Klak][Klak], that provides functionality for
receiving OSC (Open Sound Control) messages sent over UDP.

![GIF][GIF]

System Requirements
-------------------

- Unity 5.0 or later versions

The *OscKlak* package doesn't include the base Klak modules. They should be
installed before importing *OscKlak*.

How To Use It
-------------

*OscKlak* provides only a single component named **OscInput**, which receives
OSC messages and invokes Unity events with input values. For further details
of use, please refer to the sample scenes in the "Samples" directory.

Additional Notes
----------------

- Currently, the UDP port number for incoming OSC messages is fixed at 9000.
  This number is hardcoded in OscMaster.cs. In case it has to be changed, please
  modify [this line][port].
- Incoming OSC messages are viewable on the OSC monitor window (main menu
  “Window” - “OSC Monitor”).

License
-------

Copyright (C) 2016 Keijiro Takahashi

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

[Klak]: https://github.com/keijiro/Klak
[GIF]: http://49.media.tumblr.com/29ca78fb1772d2b9620cf22b8cc66cd8/tumblr_o1tmdkFN8v1qio469o1_400.gif
[Port]: https://github.com/keijiro/OscKlak/blob/master/Assets/Klak/Osc/Internal/OscMaster.cs#L46
