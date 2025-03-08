# CS2 Practice Mod

A practice mod for Counter-Strike 2, inspired by [splewis's practice mod](https://github.com/splewis/csgo-practice-mode) for CS:GO. Built using [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) and [MetaMod:Source](https://www.sourcemm.net/).

## Credits

This mod would not be possible without the amazing work of:
- **splewis** - Creator of the original [Practice Mode](https://github.com/splewis/csgo-practice-mode) plugin for CS:GO/SourceMod
- **roflmuffin** - Developer of [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- **MetaMod:Source Team** - For their [MetaMod:Source](https://www.sourcemm.net/) extension system

## Features & Roadmap

### Core Features
- [x] **Practice Mode**: Toggle practice mode with extended round time, infinite ammo, etc.
- [x] **Noclip**: Fly through walls and obstacles
- [x] **Position Saving**: Save your current position and view angles
- [x] **Position Loading**: Load a saved position and view angles
- [x] **Position Deletion**: Delete saved positions
- [x] **Auto Equipment**: Automatically receive weapons and grenades

### Grenade Practice
- [ ] **Grenade Trajectories**: Show the path of thrown grenades
- [ ] **Grenade Saving**: Save thrown grenades for later practice
- [ ] **Grenade History**: View previously thrown grenades
- [ ] **Grenade Playback**: Replay saved grenade throws

### Team Practice
- [ ] **Team Saving**: Save positions for an entire team
- [ ] **Team Loading**: Load positions for an entire team
- [ ] **Executes**: Practice site executes with bots
- [ ] **Strat Roulette**: Randomly select from predefined strategies

### Bot Control
- [ ] **Bot Placement**: Place bots at specific positions
- [ ] **Bot Mimic**: Make bots mimic player actions
- [ ] **Bot Grenades**: Make bots throw specific grenades

### Match Simulation
- [ ] **Fake Flashes**: Simulate flashbangs without actually blinding players
- [ ] **Damage Tracking**: Track damage dealt in practice sessions
- [ ] **Round Restore**: Save and restore round state

### Utility
- [ ] **Timer**: Measure execution times
- [ ] **Teleport**: Teleport to specific map locations
- [ ] **Bullet Impacts**: Show where bullets hit
- [ ] **Smoke Outlines**: Show outlines of smoke grenades

## Commands

| Chat Command | Console Command | Description |
|-------------|----------------|-------------|
| `!prac` | `css_prac` | Toggle practice mode |
| `!noclip` | `css_noclip` | Toggle noclip |
| `!save <name>` | `css_save <name>` | Save current position |
| `!load <name>` | `css_load <name>` | Load saved position |
| `!delete <name>` | `css_delete <name>` | Delete a saved position |

## Prerequisites

- Linux server running Counter-Strike 2 dedicated server (via SteamCMD)
- [MetaMod](https://www.metamodsource.net/)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

## Installation Guide

### 1. Install CS2 Dedicated Server (if not already done)

```bash
# Create directory for steamcmd
mkdir ~/steamcmd
cd ~/steamcmd

# Download and extract steamcmd
wget https://steamcdn-a.akamaihd.net/client/installer/steamcmd_linux.tar.gz
tar -xvzf steamcmd_linux.tar.gz

# Run steamcmd and install CS2 server
./steamcmd.sh
```

In the SteamCMD console:
```
force_install_dir ./cs2-server
login anonymous
app_update 730 validate
quit
```

### 2. Install MetaMod

```bash
# Navigate to CS2 server directory
cd ~/steamcmd/cs2-server/game/csgo

# Create addons directory if it doesn't exist
mkdir -p addons/metamod

# Download MetaMod (replace VERSION with latest version)
wget https://mms.alliedmods.net/mmsdrop/2.0/mmsource-2.0.0-git<VERSION>-linux.tar.gz
tar -xvzf mmsource-2.0.0-git<VERSION>-linux.tar.gz -C addons/metamod
```

### 3. Install CounterStrikeSharp

```bash
# Navigate to CS2 server directory
cd ~/steamcmd/cs2-server/game/csgo

# Download CounterStrikeSharp (replace VERSION with latest version)
wget https://github.com/roflmuffin/CounterStrikeSharp/releases/download/v<VERSION>/CounterStrikeSharp-linux-x64.zip
unzip CounterStrikeSharp-linux-x64.zip
```

### 4. Install Practice Mod

```bash
# Navigate to CounterStrikeSharp plugins directory
cd ~/steamcmd/cs2-server/game/csgo/addons/counterstrikesharp/plugins

# Create directory for practice mod
mkdir cs2-practice-mod
cd cs2-practice-mod

# Copy the practice mod files here
# You can use SFTP, SCP, or other file transfer methods
```

### 5. Configure the Server

Add these lines to your `gameinfo.gi` file (usually in `cs2-server/game/csgo/gameinfo.gi`):
```
Game    csgo
engine  Source 2

FileSystem
{
    SearchPaths
    {
        Game    csgo
        AddonsFolder addons
    }
}
```

### 6. Start the Server

```bash
cd ~/steamcmd/cs2-server
./cs2-server -game csgo -console -usercon +game_type 0 +game_mode 1 +map de_dust2
```

## Linux Server Installation

1. **Prerequisites**
   ```bash
   # Install .NET 7.0 SDK
   wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-7.0
   ```

2. **Install CounterStrikeSharp**
   - Download the latest release from [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp/releases)
   - Extract to your CS2 server directory
   ```bash
   # In CS2 server directory
   tar -xf counterstrikesharp.tar.gz
   ```

3. **Install the Mod**
   ```bash
   # In game/csgo/addons/counterstrikesharp/plugins
   git clone https://github.com/your-repo/CS2PracticeMod.git
   cd CS2PracticeMod
   dotnet build
   ```

4. **Server Configuration**
   - Add these lines to `game/csgo/cfg/server.cfg`:
   ```cfg
   // Allow mod commands
   sv_cheats 1
   sv_allowupload 1
   sv_allowdownload 1
   ```

## Configuration

The `config.json` file is automatically created on first launch with default settings:
```json
{
  "MaxSavedPositions": 50,
  "MaxSavedGrenades": 100,
  "EnableAutoNoclip": true
}
```

## Dependencies

- CounterStrikeSharp (v80 or higher)
- .NET 7.0 SDK
- MetaMod:Source

## Support

To report bugs or suggest features, please create an issue on the GitHub repository.

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
