using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using System.Text.Json;
using System.IO;

namespace CS2PracticeMod;

[MinimumApiVersion(80)]
public class CS2PracticeMod : BasePlugin
{
    public override string ModuleName => "CS2 Practice Mod";
    public override string ModuleVersion => "2.0.0";
    public override string ModuleAuthor => "Your Name";
    public override string ModuleDescription => "A comprehensive practice mod for CS2";

    // Dictionnaire pour stocker les positions sauvegardées par nom
    private Dictionary<string, PlayerSavedPosition> savedPositions = new();
    // Dictionnaire pour stocker les positions sauvegardées par joueur et par nom
    private Dictionary<string, Dictionary<string, PlayerSavedPosition>> playerSavedPositions = new();
    // Dossier pour stocker les positions sauvegardées
    private string savedPositionsFolder = "saved_positions";
    private bool practiceMode = false;

    // Liste pour stocker les bots ajoutés
    private List<CCSPlayerController> botPlayers = new List<CCSPlayerController>();

    public override void Load(bool hotReload)
    {
        // Commands are registered via attributes now, no need to call AddCommand
        
        // Register event handlers
        RegisterEventHandler<EventPlayerDeath>(OnPlayerDeath);
        
        Console.WriteLine(@"
   _____  _____ ___      _____  _____            _____ _______ _____ _____ ______     __  __  ____  _____  
  / ____|/ ____|__ \    |  __ \|  __ \     /\   / ____|__   __|_   _/ ____|  ____|   |  \/  |/ __ \|  __ \ 
 | |    | (___    ) |   | |__) | |__) |   /  \ | |       | |    | || |    | |__      | \  / | |  | | |  | |
 | |     \___ \  / /    |  ___/|  _  /   / /\ \| |       | |    | || |    |  __|     | |\/| | |  | | |  | |
 | |____ ____) |/ /_    | |    | | \ \  / ____ \ |____   | |   _| || |____| |____    | |  | | |__| | |__| |
  \_____|_____/|____|   |_|    |_|  \_\/_/    \_\_____|  |_|  |_____\_____|______|   |_|  |_|\____/|_____/ 

                    Practice Mod loaded successfully! 
                     Type '!prac' to start practicing!
");
        Console.WriteLine("CS2 Practice Mod loaded successfully!");
    }

    [ConsoleCommand("css_prac", "Toggle practice mode")]
    [ConsoleCommand("!prac", "Toggle practice mode")]
    public void OnCommandPrac(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_prac command received");
        
        // Allow command to be run from server console
        practiceMode = !practiceMode;
        
        if (practiceMode)
        {
            Console.WriteLine("Enabling practice mode");
            Server.ExecuteCommand("sv_cheats true");
            Server.ExecuteCommand("mp_limitteams 0");
            Server.ExecuteCommand("mp_autoteambalance 0");
            Server.ExecuteCommand("mp_roundtime 9999");
            Server.ExecuteCommand("mp_roundtime_defuse 9999");
            Server.ExecuteCommand("mp_maxmoney 60000");
            Server.ExecuteCommand("mp_startmoney 60000");
            Server.ExecuteCommand("mp_freezetime 0");
            Server.ExecuteCommand("mp_buytime 9999");
            Server.ExecuteCommand("sv_infinite_ammo 1");
            Server.ExecuteCommand("mp_warmup_end");
            
            // Activer les trajectoires de grenades
            Server.ExecuteCommand("sv_grenade_trajectory_prac_pipreview true");
            Server.ExecuteCommand("sv_grenade_trajectory_prac_trailtime 10");
            Server.ExecuteCommand("sv_grenade_trajectory_time_spectator 10");
            
            // Activer les impacts de balles
            Server.ExecuteCommand("sv_showimpacts 2");
            Server.ExecuteCommand("sv_showimpacts_time 10");
            
            // Temps infini
            Server.ExecuteCommand("mp_roundtime 60");
            Server.ExecuteCommand("mp_roundtime_defuse 60");
            Server.ExecuteCommand("mp_ignore_round_win_conditions 1");
            
            // Supprimer les bots
            Server.ExecuteCommand("bot_kick all");

            // God mod
            Server.ExecuteCommand("sv_regeneration_force_on true");
            
            // Acheter n'importe où
            Server.ExecuteCommand("mp_buy_anywhere 1");
            Server.ExecuteCommand("mp_buytime 60000");
            
            // Permettre de racheter des grenades même si on en a déjà
            Server.ExecuteCommand("ammo_grenade_limit_total 10000");
            Server.ExecuteCommand("mp_weapons_allow_typecount 10000");
            Server.ExecuteCommand("mp_death_drop_grenade 0");  // Empêcher les grenades de tomber à la mort
            Server.ExecuteCommand("mp_death_drop_gun 0");      // Empêcher les armes de tomber à la mort
            Server.ExecuteCommand("mp_buy_allow_grenades 1");  // Permettre l'achat de grenades
            Server.ExecuteCommand("mp_weapons_glow_on_ground 1"); // Faire briller les armes au sol
            Server.ExecuteCommand("mp_free_armor 1");          // Armure gratuite
            Server.ExecuteCommand("mp_items_prohibited 0");    // Aucun item interdit
            
            // Respawn instantané
            Server.ExecuteCommand("mp_respawn_on_death_t 1");
            Server.ExecuteCommand("mp_respawn_on_death_ct 1");
            Server.ExecuteCommand("mp_respawnwavetime_ct 0");
            Server.ExecuteCommand("mp_respawnwavetime_t 0");
            Server.ExecuteCommand("mp_respawn_immunitytime 0");
            
            // Give weapons to player if command was run by a player
            if (player != null && player.IsValid)
            {
                GivePlayerWeapons(player);
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode \u0007enabled\u0001!");
            }
            else
            {
                Console.WriteLine("Practice mode enabled from server console");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode \u0007enabled\u0001 from server console!");
            }
        }
        else
        {
            Console.WriteLine("Disabling practice mode");
            Server.ExecuteCommand("sv_cheats false");
            Server.ExecuteCommand("sv_grenade_trajectory 0");
            Server.ExecuteCommand("sv_showimpacts 0");
            Server.ExecuteCommand("mp_ignore_round_win_conditions 0");
            Server.ExecuteCommand("mp_buy_anywhere 0");
            Server.ExecuteCommand("ammo_grenade_limit_total 4");
            Server.ExecuteCommand("mp_weapons_allow_typecount 0");
            Server.ExecuteCommand("mp_death_drop_grenade 1");
            Server.ExecuteCommand("mp_death_drop_gun 1");
            Server.ExecuteCommand("mp_respawn_on_death_t 0");
            Server.ExecuteCommand("mp_respawn_on_death_ct 0");
            Server.ExecuteCommand("mp_restartgame 1");
            
            if (player != null && player.IsValid)
            {
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode \u0007disabled\u0001!");
            }
            else
            {
                Console.WriteLine("Practice mode disabled from server console");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode \u0007disabled\u0001 from server console!");
            }
        }
    }

    [ConsoleCommand("css_noclip", "Toggle noclip")]
    [ConsoleCommand("!noclip", "Toggle noclip")]
    public void OnCommandNoclip(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_noclip command received");
        if (player == null) 
        {
            Console.WriteLine("Player is null in OnCommandNoclip - must be run by a player");
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Noclip command must be run by a player!");
            return;
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (player.PlayerPawn.Value != null)
        {
            var currentMoveType = player.PlayerPawn.Value.MoveType;
            Console.WriteLine($"Current move type: {currentMoveType}");
            player.PlayerPawn.Value.MoveType = currentMoveType == MoveType_t.MOVETYPE_NOCLIP ? 
                MoveType_t.MOVETYPE_WALK : MoveType_t.MOVETYPE_NOCLIP;
            
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Noclip {(currentMoveType == MoveType_t.MOVETYPE_NOCLIP ? "disabled" : "enabled")} for {player.PlayerName}!");
        }
    }

    [ConsoleCommand("css_save", "Save current position")]
    [ConsoleCommand("!save", "Save current position")]
    public void OnCommandSave(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_save command received");
        
        // Try to find the player who issued the command
        if (player == null)
        {
            // For chat commands, try to get the player from the command
            string cmdName = command.GetArg(0).ToLower();
            if (cmdName.StartsWith("!") && Utilities.GetPlayers().Count > 0)
            {
                // For chat commands, get the first active player as a fallback
                player = Utilities.GetPlayers().FirstOrDefault(p => p != null && p.IsValid);
                Console.WriteLine($"Chat command detected, using player: {(player != null ? player.PlayerName : "none found")}");
            }
            
            if (player == null)
            {
                Console.WriteLine("No valid player found for save command");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Save command must be run by a player!");
                return;
            }
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (command.ArgCount < 2)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Usage: css_save <name>");
            return;
        }

        var posName = command.GetArg(1);
        Console.WriteLine($"Saving position with name: {posName}");
        if (player.PlayerPawn.Value != null)
        {
            if (!playerSavedPositions.ContainsKey(player.PlayerName))
            {
                playerSavedPositions[player.PlayerName] = new Dictionary<string, PlayerSavedPosition>();
            }
            
            var position = player.PlayerPawn.Value.CBodyComponent?.SceneNode?.AbsOrigin ?? new Vector(0, 0, 0);
            var angles = player.PlayerPawn.Value.EyeAngles;
            
            // Utiliser une nouvelle instance de Vector pour la vitesse
            var velocity = new Vector(0, 0, 0);
            try
            {
                if (player.PlayerPawn.Value.AbsVelocity != null)
                {
                    velocity = new Vector(
                        player.PlayerPawn.Value.AbsVelocity.X,
                        player.PlayerPawn.Value.AbsVelocity.Y,
                        player.PlayerPawn.Value.AbsVelocity.Z
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting velocity: {ex.Message}");
                // Utiliser une vélocité par défaut
                velocity = new Vector(0, 0, 0);
            }
            
            // Obtenir l'arme actuellement en main
            string weaponName = "";
            try
            {
                if (player.PlayerPawn.Value.WeaponServices != null && player.PlayerPawn.Value.WeaponServices.ActiveWeapon.IsValid)
                {
                    var activeWeapon = player.PlayerPawn.Value.WeaponServices.ActiveWeapon.Value;
                    if (activeWeapon != null)
                    {
                        weaponName = activeWeapon.DesignerName;
                        Console.WriteLine($"Active weapon: {weaponName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting active weapon: {ex.Message}");
                weaponName = "";
            }
            
            // Description optionnelle
            string description = "";
            if (command.ArgCount >= 3)
            {
                description = command.GetArg(2);
            }
            
            Console.WriteLine($"Saving position: {position}, angles: {angles}, velocity: {velocity}, weapon: {weaponName}");
            
            playerSavedPositions[player.PlayerName][posName] = new PlayerSavedPosition
            {
                Position = position,
                Angles = angles,
                Velocity = velocity,
                WeaponName = weaponName,
                MapName = Server.MapName,
                PlayerName = player.PlayerName,
                Description = description
            };
            
            // Sauvegarder la position dans un fichier
            SavePositionToFile(posName, playerSavedPositions[player.PlayerName][posName]);
            
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Position saved as \u0007{posName}\u0001 for {player.PlayerName}!");
        }
    }

    [ConsoleCommand("css_load", "Load saved position")]
    [ConsoleCommand("!load", "Load saved position")]
    public void OnCommandLoad(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_load command received");
        
        // Try to find the player who issued the command
        if (player == null)
        {
            // For chat commands, try to get the player from the command
            string cmdName = command.GetArg(0).ToLower();
            if (cmdName.StartsWith("!") && Utilities.GetPlayers().Count > 0)
            {
                // For chat commands, get the first active player as a fallback
                player = Utilities.GetPlayers().FirstOrDefault(p => p != null && p.IsValid);
                Console.WriteLine($"Chat command detected, using player: {(player != null ? player.PlayerName : "none found")}");
            }
            
            if (player == null)
            {
                Console.WriteLine("No valid player found for load command");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Load command must be run by a player!");
                return;
            }
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (command.ArgCount < 2)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Usage: css_load <name>");
            return;
        }

        var posName = command.GetArg(1);
        Console.WriteLine($"Loading position with name: {posName}");
        
        // Initialiser le dictionnaire si nécessaire
        if (!playerSavedPositions.ContainsKey(player.PlayerName))
        {
            playerSavedPositions[player.PlayerName] = new Dictionary<string, PlayerSavedPosition>();
        }
        
        // Toujours charger la position depuis le fichier pour éviter les problèmes de cache
        var savedPos = LoadPositionFromFile(posName, Server.MapName);
        if (savedPos != null)
        {
            // Mettre à jour la position en mémoire
            playerSavedPositions[player.PlayerName][posName] = savedPos;
            Console.WriteLine($"Loaded position from file: {posName}");
        }
        else
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Position \u0007{posName}\u0001 not found for map {Server.MapName}!");
            return;
        }

        var loadedPos = playerSavedPositions[player.PlayerName][posName];
        Console.WriteLine($"Loading saved position: {loadedPos}");
        
        if (player.PlayerPawn.Value != null)
        {
            Console.WriteLine($"Current player position before teleport: {player.PlayerPawn.Value.CBodyComponent?.SceneNode?.AbsOrigin}");
            
            // Utiliser les commandes du serveur pour téléporter le joueur
            string x = loadedPos.Position.X.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            string y = loadedPos.Position.Y.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            string z = loadedPos.Position.Z.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            string yaw = loadedPos.Angles.Y.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            string pitch = loadedPos.Angles.X.ToString("0.000", System.Globalization.CultureInfo.InvariantCulture);
            
            // Téléporter le joueur à la position sauvegardée
            Server.ExecuteCommand($"css_teleport {player.PlayerName} {x} {y} {z} {yaw} {pitch}");
            
            // Donner l'arme que le joueur avait en main lors de la sauvegarde
            if (!string.IsNullOrEmpty(loadedPos.WeaponName))
            {
                // Retirer toutes les armes actuelles
                Server.ExecuteCommand($"css_strip {player.PlayerName}");
                
                // Donner l'arme sauvegardée
                string weaponName = loadedPos.WeaponName;
                if (weaponName.StartsWith("weapon_"))
                {
                    weaponName = weaponName.Substring(7); // Enlever le préfixe "weapon_"
                }
                
                // Donner l'arme au joueur
                Server.ExecuteCommand($"give {player.PlayerName} weapon_{weaponName}");
                Console.WriteLine($"Gave weapon: weapon_{weaponName} to {player.PlayerName}");
                
                // Donner également des grenades si nécessaire
                if (weaponName.Contains("grenade") || weaponName.Contains("flash") || 
                    weaponName.Contains("smoke") || weaponName.Contains("molotov") || 
                    weaponName.Contains("decoy"))
                {
                    GivePlayerNades(player);
                }
            }
            
            // Vérifier la nouvelle position
            AddTimer(0.1f, () => {
                if (player != null && player.IsValid && player.PlayerPawn.Value != null)
                {
                    Console.WriteLine($"Player position after teleport: {player.PlayerPawn.Value.CBodyComponent?.SceneNode?.AbsOrigin}");
                }
            });
            
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Loaded position \u0007{posName}\u0001 for {player.PlayerName}!");
        }
        else
        {
            Console.WriteLine("Player pawn is null, cannot teleport");
        }
    }

    [ConsoleCommand("css_respawn", "Respawn yourself")]
    [ConsoleCommand("!respawn", "Respawn yourself")]
    public void OnCommandRespawn(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_respawn command received");
        if (player == null) 
        {
            Console.WriteLine("Player is null in OnCommandRespawn - must be run by a player");
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Respawn command must be run by a player!");
            return;
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        // Force respawn the player
        Server.ExecuteCommand($"mp_respawn_on_death_ct 1; mp_respawn_on_death_t 1");
        
        // Use RespawnPlayer method if available, otherwise kill and let respawn rules handle it
        if (player.PlayerPawn.Value != null && !player.PawnIsAlive)
        {
            // Force respawn by setting the player as alive
            Server.ExecuteCommand($"css_respawnplayer {player.PlayerName}");
            GivePlayerWeapons(player);
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Respawned {player.PlayerName}!");
        }
        else if (player.PlayerPawn.Value != null && player.PawnIsAlive)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 {player.PlayerName} is already alive!");
        }
    }

    [ConsoleCommand("css_nades", "Give grenades")]
    [ConsoleCommand("!nades", "Give grenades")]
    public void OnCommandGiveNades(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_nades command received");
        if (player == null) 
        {
            Console.WriteLine("Player is null in OnCommandGiveNades - must be run by a player");
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Grenades command must be run by a player!");
            return;
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        // Give all grenades to the player
        if (player.PlayerPawn.Value != null)
        {
            // Give new grenades directly to the player
            Server.ExecuteCommand($"give weapon_hegrenade");
            Server.ExecuteCommand($"give weapon_flashbang");
            Server.ExecuteCommand($"give weapon_smokegrenade");
            Server.ExecuteCommand($"give weapon_molotov");
            
            // Ensure unlimited grenades
            Server.ExecuteCommand("ammo_grenade_limit_total 10000");
            Server.ExecuteCommand("mp_weapons_allow_typecount 10000");
            Server.ExecuteCommand("mp_buy_allow_grenades 1");
            
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Gave grenades to {player.PlayerName}!");
        }
    }

    [ConsoleCommand("css_map", "Change map")]
    [ConsoleCommand("!map", "Change map")]
    public void OnCommandMap(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_map command received");
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (command.ArgCount < 2)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Usage: css_map <mapname> or !map <mapname>");
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Example maps: de_dust2, de_mirage, de_inferno, de_nuke, de_vertigo, de_ancient");
            return;
        }

        var mapName = command.GetArg(1);
        Console.WriteLine($"Changing map to: {mapName}");
        
        // Add 'de_' prefix if not already present
        if (!mapName.StartsWith("de_") && !mapName.StartsWith("cs_") && !mapName.StartsWith("aim_"))
        {
            mapName = "de_" + mapName;
            Console.WriteLine($"Added prefix, new map name: {mapName}");
        }
        
        // Announce map change
        Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Changing map to \u0007{mapName}\u0001 in 3 seconds...");
        
        // Change map after a short delay
        AddTimer(3.0f, () => 
        {
            Server.ExecuteCommand($"map {mapName}");
        });
    }

    [ConsoleCommand("css_positions", "List saved positions")]
    [ConsoleCommand("!positions", "List saved positions")]
    public void OnCommandPositions(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_positions command received");
        
        // Try to find the player who issued the command
        if (player == null)
        {
            // For chat commands, try to get the player from the command
            string cmdName = command.GetArg(0).ToLower();
            if (cmdName.StartsWith("!") && Utilities.GetPlayers().Count > 0)
            {
                // For chat commands, get the first active player as a fallback
                player = Utilities.GetPlayers().FirstOrDefault(p => p != null && p.IsValid);
                Console.WriteLine($"Chat command detected, using player: {(player != null ? player.PlayerName : "none found")}");
            }
            
            if (player == null)
            {
                Console.WriteLine("No valid player found for positions command");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Positions command must be run by a player!");
                return;
            }
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (!playerSavedPositions.ContainsKey(player.PlayerName) || playerSavedPositions[player.PlayerName].Count == 0)
        {
            // Lister toutes les positions sauvegardées pour la carte actuelle
            var positions = ListSavedPositions(Server.MapName);
            if (positions.Count == 0)
            {
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 No positions saved for {player.PlayerName}!");
            }
            else
            {
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Saved positions for {player.PlayerName}:");
                foreach (var posName in positions)
                {
                    Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 - \u0007{posName}\u0001");
                }
            }
            return;
        }

        Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Saved positions for {player.PlayerName}:");
        foreach (var posName in playerSavedPositions[player.PlayerName].Keys)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 - \u0007{posName}\u0001");
        }
    }

    [ConsoleCommand("css_teleport", "Teleport a player to a position")]
    public void OnCommandTeleport(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_teleport command received");
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (command.ArgCount < 6)
        {
            Console.WriteLine("Usage: css_teleport <player> <x> <y> <z> <yaw> <pitch>");
            return;
        }

        string targetPlayerName = command.GetArg(1);
        float x = float.Parse(command.GetArg(2), System.Globalization.CultureInfo.InvariantCulture);
        float y = float.Parse(command.GetArg(3), System.Globalization.CultureInfo.InvariantCulture);
        float z = float.Parse(command.GetArg(4), System.Globalization.CultureInfo.InvariantCulture);
        float yaw = float.Parse(command.GetArg(5), System.Globalization.CultureInfo.InvariantCulture);
        float pitch = float.Parse(command.GetArg(6), System.Globalization.CultureInfo.InvariantCulture);

        Console.WriteLine($"Teleporting {targetPlayerName} to ({x}, {y}, {z}) with angles ({pitch}, {yaw}, 0)");
        
        // Trouver le joueur cible
        var targetPlayer = Utilities.GetPlayers().FirstOrDefault(p => 
            p != null && p.IsValid && p.PlayerName.Equals(targetPlayerName, StringComparison.OrdinalIgnoreCase));
        
        if (targetPlayer == null || !targetPlayer.IsValid)
        {
            Console.WriteLine($"Player {targetPlayerName} not found or not valid");
            return;
        }
        
        if (targetPlayer.PlayerPawn.Value == null)
        {
            Console.WriteLine($"Player {targetPlayerName} pawn is null");
            return;
        }
        
        // Créer les vecteurs de position et d'angle
        Vector position = new Vector(x, y, z);
        QAngle angles = new QAngle(pitch, yaw, 0);
        
        // Téléporter le joueur
        targetPlayer.PlayerPawn.Value.Teleport(position, angles, new Vector(0, 0, 0));
        Console.WriteLine($"Teleported {targetPlayerName} to position: {position}, angles: {angles}");
    }

    [ConsoleCommand("css_strip", "Remove all weapons from a player")]
    public void OnCommandStrip(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_strip command received");
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        CCSPlayerController? targetPlayer = player;
        
        // Si un argument est fourni, utiliser ce joueur comme cible
        if (command.ArgCount >= 2)
        {
            string targetPlayerName = command.GetArg(1);
            targetPlayer = Utilities.GetPlayers().FirstOrDefault(p => 
                p != null && p.IsValid && p.PlayerName.Equals(targetPlayerName, StringComparison.OrdinalIgnoreCase));
                
            if (targetPlayer == null || !targetPlayer.IsValid)
            {
                Console.WriteLine($"Player {targetPlayerName} not found or not valid");
                return;
            }
        }
        
        if (targetPlayer == null || targetPlayer.PlayerPawn.Value == null)
        {
            Console.WriteLine("No valid player to strip weapons from");
            return;
        }
        
        // Retirer toutes les armes
        Server.ExecuteCommand("mp_drop_knife_enable 1");
        Server.ExecuteCommand($"ent_fire {targetPlayer.PlayerName} strip");
        Server.ExecuteCommand("mp_drop_knife_enable 0");
        
        Console.WriteLine($"Stripped all weapons from {targetPlayer.PlayerName}");
    }
    
    [ConsoleCommand("css_delete", "Delete a saved position")]
    [ConsoleCommand("!delete", "Delete a saved position")]
    public void OnCommandDelete(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_delete command received");
        
        // Try to find the player who issued the command
        if (player == null)
        {
            // For chat commands, try to get the player from the command
            string cmdName = command.GetArg(0).ToLower();
            if (cmdName.StartsWith("!") && Utilities.GetPlayers().Count > 0)
            {
                // For chat commands, get the first active player as a fallback
                player = Utilities.GetPlayers().FirstOrDefault(p => p != null && p.IsValid);
                Console.WriteLine($"Chat command detected, using player: {(player != null ? player.PlayerName : "none found")}");
            }
            
            if (player == null)
            {
                Console.WriteLine("No valid player found for delete command");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Delete command must be run by a player!");
                return;
            }
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        if (command.ArgCount < 2)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Usage: css_delete <name>");
            return;
        }

        var posName = command.GetArg(1);
        Console.WriteLine($"Deleting position with name: {posName}");
        
        // Supprimer la position du dictionnaire en mémoire
        if (playerSavedPositions.ContainsKey(player.PlayerName) && 
            playerSavedPositions[player.PlayerName].ContainsKey(posName))
        {
            playerSavedPositions[player.PlayerName].Remove(posName);
            Console.WriteLine($"Removed position from memory: {posName}");
        }
        
        // Supprimer le fichier de position
        bool fileDeleted = DeletePositionFile(posName, Server.MapName);
        
        if (fileDeleted)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Position \u0007{posName}\u0001 deleted successfully!");
        }
        else
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Position \u0007{posName}\u0001 not found for map {Server.MapName}!");
        }
    }
    
    [ConsoleCommand("css_addbot", "Add a bot at your current position")]
    [ConsoleCommand("!addbot", "Add a bot at your current position")]
    public void OnCommandAddBot(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_addbot command received");
        
        // Try to find the player who issued the command
        if (player == null)
        {
            // For chat commands, try to get the player from the command
            string cmdName = command.GetArg(0).ToLower();
            if (cmdName.StartsWith("!") && Utilities.GetPlayers().Count > 0)
            {
                // For chat commands, get the first active player as a fallback
                player = Utilities.GetPlayers().FirstOrDefault(p => p != null && p.IsValid);
                Console.WriteLine($"Chat command detected, using player: {(player != null ? player.PlayerName : "none found")}");
            }
            
            if (player == null)
            {
                Console.WriteLine("No valid player found for addbot command");
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 AddBot command must be run by a player!");
                return;
            }
        }
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        // Obtenir la position et l'angle du joueur
        if (player.PlayerPawn.Value == null || player.PlayerPawn.Value.CBodyComponent?.SceneNode == null)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Cannot get player position!");
            return;
        }

        Vector position = player.PlayerPawn.Value.CBodyComponent.SceneNode.AbsOrigin;
        QAngle angles = player.PlayerPawn.Value.EyeAngles;
        
        // Ajouter un bot à la position actuelle
        string botName = $"PracticeBot_{botPlayers.Count + 1}";
        string team = player.TeamNum == 2 ? "T" : "CT";
        
        // Ajouter le bot avec la commande du serveur
        Server.ExecuteCommand($"bot_add {team} {botName}");
        
        // Attendre un peu que le bot soit créé
        AddTimer(0.5f, () => {
            // Trouver le bot nouvellement créé
            var newBot = Utilities.GetPlayers()
                .FirstOrDefault(p => p != null && p.IsValid && p.IsBot && !botPlayers.Contains(p));
            
            if (newBot != null)
            {
                // Téléporter le bot à la position du joueur
                if (newBot.PlayerPawn.Value != null)
                {
                    // Téléporter le bot à la position exacte du joueur
                    newBot.PlayerPawn.Value.Teleport(position, angles, new Vector(0, 0, 0));
                    
                    // Empêcher le bot de bouger
                    Server.ExecuteCommand($"bot_stop 1");
                    
                    // Ajouter le bot à notre liste
                    botPlayers.Add(newBot);
                    
                    Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Added bot \u0007{newBot.PlayerName}\u0001 at your position!");
                    Console.WriteLine($"Added bot {newBot.PlayerName} at position {position}");
                }
                else
                {
                    Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Failed to teleport bot!");
                }
            }
            else
            {
                Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Failed to find newly created bot!");
            }
        });
    }
    
    [ConsoleCommand("css_delbot", "Remove the last added bot")]
    [ConsoleCommand("!delbot", "Remove the last added bot")]
    public void OnCommandDelBot(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_delbot command received");
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        // Vérifier s'il y a des bots à supprimer
        if (botPlayers.Count == 0)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 No bots to remove!");
            return;
        }

        // Prendre le dernier bot ajouté
        var botToRemove = botPlayers[botPlayers.Count - 1];
        
        // Vérifier si le bot est toujours valide
        if (botToRemove != null && botToRemove.IsValid)
        {
            string botName = botToRemove.PlayerName;
            
            // Supprimer le bot
            Server.ExecuteCommand($"bot_kick {botName}");
            
            // Retirer le bot de notre liste
            botPlayers.RemoveAt(botPlayers.Count - 1);
            
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Removed bot \u0007{botName}\u0001!");
            Console.WriteLine($"Removed bot {botName}");
        }
        else
        {
            // Si le bot n'est plus valide, le retirer de la liste quand même
            botPlayers.RemoveAt(botPlayers.Count - 1);
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Bot was already removed!");
        }
    }
    
    [ConsoleCommand("css_delbots", "Remove all added bots")]
    [ConsoleCommand("!delbots", "Remove all added bots")]
    public void OnCommandDelAllBots(CCSPlayerController? player, CommandInfo command)
    {
        Console.WriteLine("css_delbots command received");
        
        if (!practiceMode)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Practice mode must be enabled first!");
            return;
        }

        // Vérifier s'il y a des bots à supprimer
        if (botPlayers.Count == 0)
        {
            Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 No bots to remove!");
            return;
        }

        int botCount = botPlayers.Count;
        
        // Supprimer tous les bots
        Server.ExecuteCommand("bot_kick all");
        
        // Vider notre liste de bots
        botPlayers.Clear();
        
        Server.PrintToChatAll($" \u0004[Practice Mod]\u0001 Removed all \u0007{botCount}\u0001 bots!");
        Console.WriteLine($"Removed all {botCount} bots");
    }
    
    // Méthode pour supprimer un fichier de position
    private bool DeletePositionFile(string posName, string mapName)
    {
        try
        {
            // Construire le chemin du fichier
            string filePath = Path.Combine(savedPositionsFolder, mapName, $"{posName}.json");
            
            // Vérifier si le fichier existe
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Position file not found: {filePath}");
                return false;
            }
            
            // Supprimer le fichier
            File.Delete(filePath);
            Console.WriteLine($"Position file deleted: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting position file: {ex.Message}");
            return false;
        }
    }
    
    // Méthode pour sauvegarder une position dans un fichier
    private void SavePositionToFile(string posName, PlayerSavedPosition position)
    {
        try
        {
            // Afficher les détails de la position à sauvegarder
            Console.WriteLine($"Saving to file - Position: {position.Position.X},{position.Position.Y},{position.Position.Z}");
            
            // Créer un objet de position sérialisable
            var serializablePosition = new SerializablePosition
            {
                PosX = position.Position.X,
                PosY = position.Position.Y,
                PosZ = position.Position.Z,
                AngX = position.Angles.X,
                AngY = position.Angles.Y,
                AngZ = position.Angles.Z,
                VelX = position.Velocity.X,
                VelY = position.Velocity.Y,
                VelZ = position.Velocity.Z,
                WeaponName = position.WeaponName,
                MapName = position.MapName,
                PlayerName = position.PlayerName,
                Description = position.Description
            };
            
            // Créer le dossier pour la carte si nécessaire
            string mapFolder = Path.Combine(savedPositionsFolder, position.MapName);
            if (!Directory.Exists(mapFolder))
            {
                Directory.CreateDirectory(mapFolder);
            }
            
            // Sauvegarder dans un fichier JSON
            string filePath = Path.Combine(mapFolder, $"{posName}.json");
            string jsonString = JsonSerializer.Serialize(serializablePosition, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
            
            // Afficher le contenu du fichier pour débogage
            Console.WriteLine($"Position saved to file: {filePath}");
            Console.WriteLine($"File content: {jsonString}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving position to file: {ex.Message}");
        }
    }
    
    // Méthode pour charger une position depuis un fichier
    private PlayerSavedPosition? LoadPositionFromFile(string posName, string mapName)
    {
        try
        {
            // Construire le chemin du fichier
            string filePath = Path.Combine(savedPositionsFolder, mapName, $"{posName}.json");
            
            // Vérifier si le fichier existe
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Position file not found: {filePath}");
                return null;
            }
            
            // Lire le fichier JSON
            string jsonString = File.ReadAllText(filePath);
            Console.WriteLine($"Loaded file content: {jsonString}");
            
            var serializablePosition = JsonSerializer.Deserialize<SerializablePosition>(jsonString);
            
            if (serializablePosition == null)
            {
                Console.WriteLine($"Failed to deserialize position from file: {filePath}");
                return null;
            }
            
            // Afficher les détails de la position chargée
            Console.WriteLine($"Loaded from file - Position: {serializablePosition.PosX},{serializablePosition.PosY},{serializablePosition.PosZ}");
            
            // Convertir en PlayerSavedPosition
            return new PlayerSavedPosition
            {
                Position = new Vector(serializablePosition.PosX, serializablePosition.PosY, serializablePosition.PosZ),
                Angles = new QAngle(serializablePosition.AngX, serializablePosition.AngY, serializablePosition.AngZ),
                Velocity = new Vector(serializablePosition.VelX, serializablePosition.VelY, serializablePosition.VelZ),
                WeaponName = serializablePosition.WeaponName,
                MapName = serializablePosition.MapName,
                PlayerName = serializablePosition.PlayerName,
                Description = serializablePosition.Description
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading position from file: {ex.Message}");
            return null;
        }
    }
    
    // Méthode pour lister toutes les positions sauvegardées pour une carte
    private List<string> ListSavedPositions(string mapName)
    {
        try
        {
            List<string> positions = new List<string>();
            
            // Chemin du dossier de la carte
            string mapFolder = Path.Combine(savedPositionsFolder, mapName);
            
            // Vérifier si le dossier existe
            if (!Directory.Exists(mapFolder))
            {
                Console.WriteLine($"Map folder not found: {mapFolder}");
                return positions;
            }
            
            // Lister tous les fichiers .json dans le dossier
            foreach (var file in Directory.GetFiles(mapFolder, "*.json"))
            {
                // Extraire le nom de la position (nom du fichier sans extension)
                string posName = Path.GetFileNameWithoutExtension(file);
                positions.Add(posName);
                
                // Essayer de charger la description
                try
                {
                    var pos = LoadPositionFromFile(posName, mapName);
                    if (pos != null && !string.IsNullOrEmpty(pos.Description))
                    {
                        Console.WriteLine($"Position {posName}: {pos.Description}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading position description: {ex.Message}");
                }
            }
            
            return positions;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error listing positions: {ex.Message}");
            return new List<string>();
        }
    }

    // Event handler for player death to give them weapons when they respawn
    private HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (!practiceMode) return HookResult.Continue;
        
        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;
        
        // Give weapons to player after a short delay to ensure they've respawned
        AddTimer(0.5f, () => 
        {
            if (player != null && player.IsValid && player.PawnIsAlive)
            {
                GivePlayerWeapons(player);
            }
        });
        
        return HookResult.Continue;
    }

    // Méthode pour donner des armes au joueur
    private void GivePlayerWeapons(CCSPlayerController player)
    {
        if (player == null || player.PlayerPawn.Value == null) return;
        
        AddTimer(0.1f, () =>
        {
            // Ensure the player has maximum money
            Server.ExecuteCommand("mp_maxmoney 60000");
            Server.ExecuteCommand("mp_startmoney 60000");
            
            // Ensure unlimited grenades
            Server.ExecuteCommand("ammo_grenade_limit_total 10000");
            Server.ExecuteCommand("mp_weapons_allow_typecount 10000");
            Server.ExecuteCommand("mp_buy_allow_grenades 1");
            
            // Give weapons directly to the player
            if (player != null && player.IsValid && player.PlayerPawn.Value != null)
            {
                // Use player-specific commands
                Server.ExecuteCommand($"give weapon_ak47");
                Server.ExecuteCommand($"give weapon_m4a1");
                Server.ExecuteCommand($"give weapon_hegrenade");
                Server.ExecuteCommand($"give weapon_flashbang");
                Server.ExecuteCommand($"give weapon_smokegrenade");
                Server.ExecuteCommand($"give weapon_molotov");
            }
        });
    }
    
    // Méthode pour donner des grenades au joueur
    private void GivePlayerNades(CCSPlayerController player)
    {
        if (player == null || player.PlayerPawn.Value == null) return;
        
        AddTimer(0.1f, () =>
        {
            // Give new grenades directly to the player
            Server.ExecuteCommand($"give weapon_hegrenade");
            Server.ExecuteCommand($"give weapon_flashbang");
            Server.ExecuteCommand($"give weapon_smokegrenade");
            Server.ExecuteCommand($"give weapon_molotov");
        });
    }
}

public class PlayerSavedPosition
{
    public Vector Position { get; set; } = new Vector(0, 0, 0);
    public QAngle Angles { get; set; } = new QAngle(0, 0, 0);
    public Vector Velocity { get; set; } = new Vector(0, 0, 0);
    public string WeaponName { get; set; } = "";
    public string MapName { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string Description { get; set; } = "";
    
    public override string ToString()
    {
        return $"Position: {Position}, Angles: {Angles}, Weapon: {WeaponName}, Map: {MapName}";
    }
}

public class SerializablePosition
{
    public float PosX { get; set; } = 0;
    public float PosY { get; set; } = 0;
    public float PosZ { get; set; } = 0;
    public float AngX { get; set; } = 0;
    public float AngY { get; set; } = 0;
    public float AngZ { get; set; } = 0;
    public float VelX { get; set; } = 0;
    public float VelY { get; set; } = 0;
    public float VelZ { get; set; } = 0;
    public string WeaponName { get; set; } = "";
    public string MapName { get; set; } = "";
    public string PlayerName { get; set; } = "";
    public string Description { get; set; } = "";
}
