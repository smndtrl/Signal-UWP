# Signal-UWP

Even though this project is called Signal, it is *by no means an official Whispersystems project*.
It is intended to have a working Signal-like client for the UWP (Universal Windows Platform).

## What to expect
* Nothing

## Subprojects
* curve25519 (external): by JeffR [https://github.com/langboost/libaxolotl-windows]
* libaxolotl: java port of version 1.3.4 [https://github.com/WhisperSystems/libaxolotl-java]
* libtextsecure: java port of version 1.6.0 [https://github.com/WhisperSystems/libtextsecure-java]
* libsettings: A library for UserControls like a simpler UI for settings
* SignalTask: Background processes needed for WNS push
* Signal: The app
* SignalTest: Unit tests for libaxolotl

Please excuse the code quality, lots of strange comments and all the other creepy stuff. 
All of my C# programming experience comes from porting these libraries.

## Contact
* smndtrl (xmpp:simon@ssl.tophostingteam.de)
