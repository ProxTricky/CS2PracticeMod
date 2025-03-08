# Script pour nettoyer et publier le mod CS2 Practice

# Compiler en mode Release
dotnet publish -c Release -r linux-x64 --self-contained false

# Créer un dossier pour le résultat final
$publishDir = "bin\Release\net7.0\linux-x64\publish"
$cleanDir = "publish"

if (Test-Path $cleanDir) {
    Remove-Item -Path $cleanDir -Recurse -Force
}

New-Item -Path $cleanDir -ItemType Directory

# Copier uniquement les fichiers nécessaires
Copy-Item "$publishDir\cs2-practice-mod.dll" -Destination $cleanDir
Copy-Item "$publishDir\config.json" -Destination $cleanDir -ErrorAction SilentlyContinue

# Créer le dossier pour les positions sauvegardées
New-Item -Path "$cleanDir\saved_positions" -ItemType Directory

# Afficher le résultat
Write-Host "\nPublication terminée ! Les fichiers suivants ont été créés dans le dossier 'publish' :"
Get-ChildItem -Path $cleanDir -Recurse | Format-Table Name, Length

Write-Host "\nPour installer le mod sur votre serveur CS2 :"
Write-Host "1. Copiez le contenu du dossier 'publish' dans le dossier 'game/csgo/addons/counterstrikesharp/plugins/cs2-practice-mod' sur votre serveur"
Write-Host "2. Redémarrez votre serveur ou rechargez les plugins avec la commande 'css_plugins_reload'"
