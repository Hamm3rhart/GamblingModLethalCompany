# CHANGELOG

## 1.4.5
- Fix fork config namespace (Hamm3rhart)
- Centralize machine music to a single emitter at the grid midpoint
- Reduce debug logging noise (now gated by Debug Logging config)

## 1.4.4
- Fix multiplayer gambling sync: clients now see countdown, jingle, and scrap updates reliably
- Known issue: explosions are still buggy and currently only work for the host
- Config renamed (new config namespace for the fork)

## 1.4.3
- Fix client interaction: ensure machines set InteractableObject layer/scale on `OnNetworkSpawn`, so clients (including MoreCompany) can raycast/interact

## 1.4.2
- Fix machines only appearing for the host: register the gambling machine network prefab before host/client start so all peers (including MoreCompany clients) know the prefab and receive spawns

## 1.4.1
- Fix default chances totaling 101% by setting Halve to 49% (Explode stays 1%); delete/regenerate existing BepInEx config to pick up the new defaults

## 1.4.0
- Add explode outcome with configurable chance/multiplier; plays an "REDACTED" stinger, then explodes after a short delay
- Rebuild asset bundle to include the new stinger audio and updated machine music handling
- Centralize machine music with a shared emitter at the machine cluster midpoint; pause per-machine sources while honoring the client music toggle/volume to avoid overlapping/phasey music when several machines are nearby
- Expand machine layout controls in detail:
	- Machine spawn mode: AUTO spawns up to player count; MAX fills the grid capacity
	- Grid sizing: rows (X) and machines per row (Z) define the total grid; negatives in spacing let you flip directions
	- Spacing: row/column spacing support decimals; zero falls back to 5 to avoid overlap
	- Rotation: yaw per machine (0-359); invalid values fallback to 90
	- Offsets: X/Y/Z offsets shift the entire grid anchor
- Add max value limit option to cap scrap value after gambling (0 or negative disables the cap) so extreme multipliers can't blow past intended scrap values or hit integer limits
- Keep multiple-machine support while respecting cooldown and use-count settings

## 1.3.4
- Add configuration to add up to 3 gambling machines!
- Add configuration to limit the number of uses on a gambling machine
- Add a fix to prevent machines from getting triggered multiple times when everyone is trying to interact with one machine
- Fix gambling machine showing while in orbit
- Font size changes

## 1.2.3
- Updated the interaction text and font

## 1.1.3
- Updated the configurable multiplier fields to allow decimal numbers (previously only allowed whole numbers)
- Changing the machine's cooldown is now available in the configuration file! (Thanks Adam759 for working on this!)

## 1.1.2
- Fix gambling result audio not playing for other players

## 1.1.1
- Maybe fixed bug that allowed multiple people to constantly spam the machine (gambling addicts...) which caused strange gambling outcomes!
- Maybe helped alleviate some desync for scrap value
- For the default configuration, increase halve chance by 3% and decrease double by 3%

## 1.1.0
- Update readme

## 1.0.9
- For the default configuration, increase double chance by 3% and decrease zero chance by 3%

## 1.0.8
- Fix gambling machine activating multiple times in one interaction

## 1.0.7
- Fix interaction key showing up incorrectly

## 1.0.6

- Changed scrap value currency dollar icon to match the ingame currency icon
- Update interaction key displayed on screen to be the user's set interaction key

## 1.0.5

- Updated readme for feature request

## 1.0.4

- Updated readme

## 1.0.3

- Configurable fields to enable music and to set music volume

## 1.0.2

- Update readme format

## 1.0.1

- Update readme

## 1.0.0

- Release
