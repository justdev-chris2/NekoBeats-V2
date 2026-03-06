# NekoBeats 
> v2.1

<p align="center">
  <img src="NekoBeatsLogo.png" width="150" height="150">
</p>

A sleek audio visualizer that turns your music into floating light bars. Revived and better than ever.

![NekoBeats](https://img.shields.io/badge/NekoBeats-V2-blueviolet)

## Features 🌟

- **Real-time audio visualization** using system output
- **Fullscreen floating bars** that don't interrupt your workflow
- **Click-through mode** (bars won't block clicks)
- **Customizable themes** with color picker
- **Adjustable bars**: count, height, opacity
- **Draggable window** when needed
- **Separate control panel** for easy adjustments
- **Built-in recording** to save visualizations as video

## Controls 🎮

| Control | Function |
|---------|----------|
| **Bar Color** | Change bar color (any color you want!) |
| **Opacity** | Make bars more/less transparent |
| **Height** | Adjust how tall bars grow |
| **Bar Count** | 32-512 bars across your screen |
| **Click Through** | Toggle if bars block mouse clicks |
| **Draggable** | Move the visualizer around |
| **Exit** | Close the application |

## Recording 🎥

NekoBeats includes a built-in video recorder in the **Recording** tab. Choose your resolution, FPS, and duration, then record your visualizations.

**For recordings with audio**, we recommend using [OBS Studio](https://obsproject.com/):
1. Add NekoBeats window as a source in OBS
2. Configure audio input
3. Hit record
4. OBS will capture both video and audio seamlessly

This gives you higher quality control and better audio-video sync.

## Installation ⚡

1. Download the latest `NekoBeats.zip` from [Releases](https://github.com/justdev-chris/NekoBeats-V2/releases)
2. Run it (requires Windows 10/11)
3. Play some music 🎶
4. Adjust settings in the control panel

## Build from Source 🛠️

```
git clone https://github.com/justdev-chris/NekoBeats-V2.git
cd NekoBeats-V2
dotnet restore
dotnet publish -c Release -r win-x64 --self-contained true
```

## Requirements 📋

- Windows 10/11
- .NET 8.0 Runtime (included in self-contained build)
- Audio output playing music

## How it Works 🔬

NekoBeats captures your system audio output using NAudio, performs FFT analysis to extract frequencies, and visualizes them as colorful bars that pulse to the beat. The bars are rendered in a transparent overlay window that sits above everything else.

## V2 Improvements over V1 🚀

- ✅ **Proper FFT processing** (smoother visualization)
- ✅ **Real color picker** (not just preset themes)
- ✅ **Click-through technology** (use PC while visualizing)
- ✅ **More bars** (up to 512 for detailed spectrum)
- ✅ **Better performance** (60 FPS rendering)
- ✅ **Modern UI** (separate control panel)
- ✅ **Single EXE** (no dependencies needed)
- ✅ **Built-in recording** (save visualizations as MP4)

## Troubleshooting 🔧

**No bars showing?**
- Make sure audio is playing through your default output
- Check that your audio isn't muted

**Visualizer laggy?**
- Reduce bar count in settings
- Close other intensive applications

**Can't click through?**
- Enable "Click Through" in control panel
- Make sure no other apps are forcing focus

**Recording not working?**
- Make sure SharpAvi is installed via NuGet
- Check that you have write permissions to the save location

## License 📄

MIT License - do whatever you want with it!

## Credits 👏

- **NAudio** for audio capture
- **SharpAvi** for video encoding
- **FFT algorithm** for frequency analysis
- **Original NekoBeats V1** for inspiration
- **You** for using it! 🎧

---

*Made with ❤️ for music lovers everywhere. Turn up the volume and watch the magic happen!*
