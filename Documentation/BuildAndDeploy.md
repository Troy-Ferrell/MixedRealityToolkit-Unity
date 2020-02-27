# Building and deploying MRTK

Deploying an app on a desired platform with MRTK is generally the same process as building any other app for that platform through Unity. Learn more about building for other platforms at [Publishing Builds](https://docs.unity3d.com/Manual/PublishingBuilds.html).

## Build/deploy recommendations

Building can be a time intensive process depending on the target platform, workflow, and size of project. It is recommended to utilize SSDs, or similar high-speed hard disks, for storing the project and build folders when compiling. 

Furthermore, if [building for IL2CPP](https://docs.unity3d.com/Manual/IL2CPP-OptimizingBuildTimes.html), use incremental building by re-building the Unity project to the same target build folder. This will ensure Unity does not re-build the entire project and only the revelant bits that have changed and need to be updated in that folder. 

It is also recommended to [disable any anti-malware software scans over folders](https://support.microsoft.com/en-us/help/4028485/windows-10-add-an-exclusion-to-windows-security) being utilized during the compilation process or disable anti-malware scanning in general.

## Building shaders

- optimize mesh data
- including standard shader variants in build


## Building and deploying to Windows Mixed Reality

Instructions on how to build and deploy for the Windows Mixed Reality platform (UWP app) can be found at [building your application to device](https://docs.microsoft.com/windows/mixed-reality/mrlearning-base-ch1#build-your-application-to-your-device). This includes apps for HoloLens 1 and HoloLens 2. Furthermore, there is information for [how to debug Mixed Reality apps via Visual Studio](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio).

When building via the build window in Unity, the following settings are recommended. Other properties are to digression of the developer. 

- **Target SDK Version**: *Latest Installed*
- **Minimum Platform Version**: "10.0.10240.0

![Build window](../Documentation/Images/getting_started/BuildWindow.png)

> [!IMPORTANT]
> HoloLens 2 apps require the Windows SDK, build 18362 or later. Make sure that the "Target SDK Version" dropdown includes the option "10.0.18362.0" - if this is missing, [the latest Windows SDK](https://developer.microsoft.com/windows/downloads/windows-10-sdk) needs to be installed.

## See also:

- [Windows Mixed Reality - Install the Tools](https://docs.microsoft.com/en-us/windows/mixed-reality/install-the-tools)
- [Windows Mixed Reality - Debugging via Visual Studio](https://docs.microsoft.com/en-us/windows/mixed-reality/using-visual-studio)
