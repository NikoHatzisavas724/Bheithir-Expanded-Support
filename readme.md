# Bheithir

## What is Bheithir?
Bheithir is a program that sets Discord Rich Presence (RPC) status for various emulators. 

## What emulators are supported right now?
- [Citron](https://citron-emu.org/)
- [mGBA](https://mgba.io/)
- [Mesen](https://www.mesen.ca/)
- [Ares](https://ares-emu.net/)
- [BSNES](https://bsnes.org/)
- [PPSSPP](https://www.ppsspp.org/)
- [redream](https://redream.io/)
- [DOSBox](https://www.dosbox.com/)
    - [MS-DOS](https://en.wikipedia.org/wiki/MS-DOS) emulator primarilly designed for playing games
- [DOSBox-X](https://dosbox-x.com/)
    - Fork of [DOSBox](https://www.dosbox.com/) an [MS-DOS](https://en.wikipedia.org/wiki/MS-DOS) emulator primarilly designed for playing games
- [FCEUX](http://www.fceux.com/web/home.html)
    - [NES](https://en.wikipedia.org/wiki/Nintendo_Entertainment_System)
- [SNES9X](http://www.snes9x.com/)
    - [SNES](https://en.wikipedia.org/wiki/Super_Nintendo_Entertainment_System)
- [Fusion](https://segaretro.org/Kega_Fusion)
    - SEGA 8 and 16-bit systems including the [Master System / Mark III](https://en.wikipedia.org/wiki/Super_Nintendo_Entertainment_System) and [Genesis / Mega Drive](https://en.wikipedia.org/wiki/Sega_Genesis)
- [VBA-M](https://github.com/visualboyadvance-m/visualboyadvance-m)
    - [Game Boy](https://en.wikipedia.org/wiki/Game_Boy), [Game Boy Color](https://en.wikipedia.org/wiki/Game_Boy_Color), [Game Boy Advance](https://en.wikipedia.org/wiki/Game_Boy_Advance)
- [MAME](https://www.mamedev.org/)
    - Emulates a variety of arcade systems and vintage computers
    - [List of supported systems](https://emulation.gametechwiki.com/index.php/MAME_compatibility_list)

## What do I need to do/know before I run it?
- You can get the latest binary from the [releases](https://github.com/NikoHatzisavas724/Bheithir-Expanded-Support/releases) pages.
- **All builds are framework-dependent!** Please follow the instructions for your OS to download .NET [here](https://dotnet.microsoft.com/download/dotnet/).
- Bheithir runs on it's own and is entirely seperate from all emulators supported. **This is not a plugin!**
- Bheithir works with all versions of these emulators after the first release where support was added.
    - Since this program is dependant on the text in the window title, it is possible earlier or future releases are not supported.
    - If the presence does not look right, please leave an [issue](https://github.com/NikoHatzisavas724/Bheithir-Expanded-Support/issues)!
- The presence isn't showing up at all!!
    - Setting RPC status is only supported if you have the desktop app (Spotify RPC is an exception).
    - Open Discord and go to Settings (click the cog wheel next to your username, and the mute and deafen buttons).
    - Select "Game Activity" under "Gaming Settings" in the sidebar.
    - Make sure this option is selected. If not, game RPC will not display.
![Display running game as status](https://i.imgur.com/jRIGHbK.png "Display running game as status")
### Earliest Known Supported Versions
- OS
    - Windows 7 x86 (32-bit)
        - x86_64 (64-bit) and ARM versions of Windows are supported as well, but builds target x86.
    - Anything older is not supported whatsoever.
- .NET
    - v5.0
    - Anything older is not supported whatsover.
- DOSBox
    - x86 (32-bit)
    - v0.74-3
- FCEUX
    - x86 (32-bit)
    - v2.2.3
- Snes9x
    - x86 (32-bit), x86_64 (64-bit)
    - v1.60
- Fusion
    - x86 (32-bit)
    - v3.64
- VBA-M
    - x86 (32-bit), x86_64 (64-bit)
    - v2.1.4
- MAME
    - x86_64 (64-bit)
        - 32-bit builds are no longer provided, but it may work with older 32-bit versions since the program looks for programs open that start with "mame", not "mame64".
    - v0.220

## How do I run it?
- Open Discord and make sure you are signed in.
- Open the emulator of your choice. You don't have to open a ROM/ISO yet.
- Open Bheithir either by double-clicking it in File Explorer or running it from the command line.
- Follow the instructions in the program, and if nothing goes wrong, you're all set!
- **You must open Bheithir after you open the emulator, as it checks to see if the emulator you selected is open. If not, the program will close!**
![CLI Execution](https://i.imgur.com/tct0D5Y.png "CLI Execution")

## Upcoming Features
- A program icon
- Running as a system tray application so that it can update the background and you don't need to worry about it

## Special Thanks
Special thanks to [BloodDragooner](https://github.com/BloodDragooner) for giving me the idea to do this sort of thing with [SumatraPDF](https://github.com/sumatrapdfreader/sumatrapdf)! A lot of code was borrowed from that project, Chilong, so check it out [here](https://github.com/MechaDragonX/Chilong)!
Also, special thanks to the original author of Bheithir, [MechaDragonX](https://github.com/MechaDragonX/), and the author of the Bheithir-DOSBox-X-Support, [LostGameDev](https://github.com/LostGameDev), both of their code are used in this project as this is a fork of Bheithir-DOSBox-X-Support.