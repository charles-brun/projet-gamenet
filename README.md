# Projet Gamenet

## Fonctionnalités

Jeu tour par tour à deux joueurs en réseau. Les joueurs se connectent via un serveur au sein d'un même LAN.

## Prérequis

Avoir un IDE et dotnet 6.0+ installé.

## Installation

Cloner le projet depuis `https://github.com/charles-brun/projet-gamenet`.

Selon si la machine est serveur ou client, se rendre dans le dossier "Server" ou "Client". Y établir un environnement dotnet avec la commande `dotnet new console`. 

Supprimer le fichier "Program.cs" nouvellement créé.

## Déploiement

### Machine serveur :

Pour chaque partie, depuis le dossier "Server", lancer le programme grâce à la commande `dotnet run`.

### Machine client :

Identifier l'adresse IP <SERVER_IP> de votre serveur.
Pour chaque partie, depuis le dossier "Client", lancer le programme grâce à la commande `dotnet run SERVER_IP`.

Vous pouvez également remplacer le contenu du fichier "serverIP.txt" par l'IP de votre serveur. Le client pourra alors se lancer simplement avec la commande `dotnet run`.

## Licence

GNU General Public License version 3