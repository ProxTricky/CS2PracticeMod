# Changelog

## v1.0.0 (2025-03-08)

### Added
- Initial release of CS2 Practice Mod
- Practice mode toggle with `!prac` command
  - Unlimited money
  - Extended round time
  - Infinite ammo
  - No freeze time
  - Buy anywhere
  - Cheats enabled
- Noclip functionality with `!noclip` command
- Position saving system
  - Save positions with `!save <name>` command
  - Load positions with `!load <name>` command
  - Delete positions with `!delete <name>` command
  - Positions saved to JSON files for persistence
  - Includes player position, view angles, and weapon
- Auto-equipment system
  - Players automatically receive weapons and grenades in practice mode
- Teleport command with `!tp` or `css_teleport`
- Strip weapons command with `!strip` or `css_strip`
- Comprehensive logging for debugging

### Technical
- Built with CounterStrikeSharp and MetaMod:Source
- Implemented for CS2 dedicated servers
- Designed for Linux server environments
- Position data serialization with System.Text.Json
- Automatic configuration file generation

### Known Issues
- Position saving may not work correctly on some custom maps
- Weapon restoration after loading a position may occasionally fail

### Future Plans
- Implement grenade trajectory visualization
- Add bot control features
- Develop team practice utilities
- Create round state saving and restoration

## [2.0.1] - 2025-03-08
### Added
- Bot Management System improvements:
  - Bots now look in the same direction as the player
  - Bot damage information display in chat (grenade damage)
  - Bot flash duration information display in chat

## [2.0.0] - 2025-03-08

### Added
- Bot Management System
  - `!addbot` / `css_addbot`: Adds a bot at the player's current position with the same view angle
  - `!delbot` / `css_delbot`: Removes the last added bot
  - `!delbots` / `css_delbots`: Removes all added bots
  - Bot damage information display in chat (grenade damage)
  - Bot flash duration information display in chat
- Bots are placed exactly at the player's position and do not move
- Bots are automatically assigned to the same team as the player
