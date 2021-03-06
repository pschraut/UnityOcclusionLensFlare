# Changelog
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).
 
## [1.1.0] - 2021-03-07
### Changed
 - Use [Camera.RenderWithShader](https://docs.unity3d.com/ScriptReference/Camera.RenderWithShader.html) over 
 [Camera.SetReplacementShader](https://docs.unity3d.com/ScriptReference/Camera.ResetReplacementShader.html) and [Camera.Render](https://docs.unity3d.com/ScriptReference/Camera.Render.html) calls.
 - Removed usage of [ColorMask RGB](https://docs.unity3d.com/Manual/SL-Pass.html) from ```OcclusionLensFlare.shader```, because according to [this Unity documentation](https://docs.unity3d.com/Manual/SL-ShaderPerformance.html) it can affect performance negatively on mobile gpu's.
 
## [1.0.0] - 2021-03-07
 - First public release
