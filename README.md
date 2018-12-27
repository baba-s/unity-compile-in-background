# UnityCompileInBackground

You can start compiling without having to return focus to the Unity editor after changing the script.

[![](https://img.shields.io/github/release/baba-s/unity-compile-in-background.svg?label=latest%20version)](https://github.com/baba-s/unity-compile-in-background/releases)
[![](https://img.shields.io/github/release-date/baba-s/unity-compile-in-background.svg)](https://github.com/baba-s/unity-compile-in-background/releases)
![](https://img.shields.io/badge/Unity-2017.4%2B-red.svg)
![](https://img.shields.io/badge/.NET-3.5%2B-orange.svg)
[![](https://img.shields.io/github/license/baba-s/unity-compile-in-background.svg)](https://github.com/baba-s/unity-compile-in-background/blob/master/LICENSE)

## Version

- Unity 2018.3.0f2

## Usage

![](https://cdn-ak.f.st-hatena.com/images/fotolife/b/baba_s/20181227/20181227150651.gif)

For example, if you edit and save the code in Visual Studio,   
compilation will start without returning to the Unity editor.  

## Options

![](https://cdn-ak.f.st-hatena.com/images/fotolife/b/baba_s/20181227/20181227150738.png)

You can change the setting by selecting "UnityCompileInBackground" from  
"Edit> Preferences ..." in the Unity menu.  
(Select "Compile in BG" for Unity 2018.2 and below)  

| Item | Description |
| :-- | :-- |
| Enabled | Whether to enable UnityCompileInBackground |
| Wait Time (sec) | Interval to monitor whether script has been modified |
| Enabled Debug Log | Whether to output logs when script is monitored |

Since the setting of UnityCompileInBackground is saved with EditorPrefs,  
the settings are shared when using UnityCompileInBackground in other Unity projects.  
