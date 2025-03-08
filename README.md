<div align="center">

# 🎮 CS2 Practice Mod

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/ProxTricky/CS2PracticeMod)](https://github.com/ProxTricky/CS2PracticeMod/releases/latest)
[![GitHub license](https://img.shields.io/github/license/ProxTricky/CS2PracticeMod)](https://github.com/ProxTricky/CS2PracticeMod/blob/main/LICENSE)

**A comprehensive practice mode plugin for Counter-Strike 2 dedicated servers.**

[Key Features](#-key-features) • 
[Installation](#-installation) • 
[Commands](#-commands) • 
[Roadmap](#-roadmap) • 
[Contributing](#-contributing)

<img src="https://raw.githubusercontent.com/ProxTricky/CS2PracticeMod/main/assets/Banner.png" alt="CS2 Practice Mod Banner" width="650">

</div>

## 📋 Overview

CS2 Practice Mod enhances the practice experience on CS2 servers by providing essential tools for individual and team practice sessions. Inspired by [splewis's popular CS:GO practice mode](https://github.com/splewis/csgo-practice-mode), this plugin brings similar functionality to CS2 using CounterStrikeSharp and MetaMod:Source.

## ✨ Key Features

- **Practice Mode**: Toggle server settings optimized for practice
  - Unlimited money
  - Extended round time
  - Infinite ammo
  - No freeze time
  - Buy anywhere
  - Cheats enabled

- **Position System**: Save, load, and delete positions with associated weapons
  - Persistent storage across server restarts
  - Includes player position, view angles, and weapon

- **Bot Control**: Place and manage bots for practice scenarios
  - Add bots at your exact position
  - Remove individual bots or all bots at once
  - Bots remain stationary for consistent practice

- **Noclip**: Easily navigate through the map to explore strategies

- **Auto-Equipment**: Automatically receive essential weapons and utilities

## 🚀 Roadmap

### Core Features
- [x] **Practice Mode**: Toggle practice mode with extended round time, infinite ammo, etc.
- [x] **Noclip**: Fly through walls and obstacles
- [x] **Position Saving**: Save your current position and view angles
- [x] **Position Loading**: Load a saved position and view angles
- [x] **Position Deletion**: Delete saved positions
- [x] **Auto Equipment**: Automatically receive weapons and grenades

### Bot Control
- [x] **Bot Placement**: Place bots at specific positions
- [x] **Bot Removal**: Remove individual or all bots
- [ ] **Bot Mimic**: Make bots mimic player actions
- [ ] **Bot Grenades**: Make bots throw specific grenades

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

### Match Simulation
- [ ] **Fake Flashes**: Simulate flashbangs without actually blinding players
- [ ] **Damage Tracking**: Track damage dealt in practice sessions
- [ ] **Round Restore**: Save and restore round state

### Utility
- [ ] **Timer**: Measure execution times
- [ ] **Teleport**: Teleport to specific map locations
- [ ] **Bullet Impacts**: Show where bullets hit
- [ ] **Smoke Outlines**: Show outlines of smoke grenades

## 💻 Installation

### Prerequisites

- Linux server running Counter-Strike 2 dedicated server
- [MetaMod:Source](https://www.metamodsource.net/)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (v80 or higher)
- .NET 7.0 SDK

### Quick Install

1. **Download the latest release** from the [Releases page](https://github.com/ProxTricky/CS2PracticeMod/releases)

2. **Extract the files** to your CS2 server:
   ```bash
   # Navigate to CounterStrikeSharp plugins directory
   cd ~/cs2-server/game/csgo/addons/counterstrikesharp/plugins
   
   # Create directory for practice mod
   mkdir cs2-practice-mod
   cd cs2-practice-mod
   
   # Extract the downloaded zip file here
   ```

3. **Restart your server** or reload plugins with:
   ```
   css_plugins_reload
   ```

### Detailed Installation Guide

For a complete installation guide including server setup, see the [Installation Wiki](https://github.com/ProxTricky/CS2PracticeMod/wiki/Installation).

## 🔧 Commands

| Chat Command | Console Command | Description |
|-------------|----------------|-------------|
| `!prac` | `css_prac` | Toggle practice mode |
| `!noclip` | `css_noclip` | Toggle noclip |
| `!save <name>` | `css_save <name>` | Save current position |
| `!load <name>` | `css_load <name>` | Load saved position |
| `!delete <name>` | `css_delete <name>` | Delete a saved position |
| `!addbot` | `css_addbot` | Add a bot at your current position |
| `!delbot` | `css_delbot` | Remove the last added bot |
| `!delbots` | `css_delbots` | Remove all added bots |

## ⚙️ Configuration

The `config.json` file is automatically created on first launch with default settings:
```json
{
  "MaxSavedPositions": 50,
  "MaxSavedGrenades": 100,
  "EnableAutoNoclip": true
}
```

## 👥 Contributing

Contributions are welcome! Feel free to submit issues or pull requests.

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Credits

This mod would not be possible without the amazing work of:
- **splewis** - Creator of the original [Practice Mode](https://github.com/splewis/csgo-practice-mode) plugin for CS:GO/SourceMod
- **roflmuffin** - Developer of [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- **MetaMod:Source Team** - For their [MetaMod:Source](https://www.sourcemm.net/) extension system
