# Gambling Is the Only Way

This is a fork of a fork.
First fork: [GamblingMachineAtTheCompanyFork](https://thunderstore.io/c/lethal-company/p/Kaza/GamblingMachineAtTheCompanyFork)
Original: [GamblingMachineAtTheCompany](https://thunderstore.io/c/lethal-company/p/JunLethalCompany/GamblingMachineAtTheCompany)

I wanted to add more customization to the mod and had some feature/quality-of-life ideas.

All credit goes to the original creator and the first fork’s author for updating it to the current version.

## Description

"To make your ambitions come true, you have to take risks."

Adds gambling machines at "The Company" moon. You'll see them right in front of you when you land.

The gamble roll can have 6 results (all configurable):

- Jackpot = 10x
- Triple = 3x
- Double = 2x
- Halve = 0.5x (scrap reduced by half)
- Zero = 0x (scrap is worthless)
- Explode = 0x (what do you think will happen?)

The default gambling chances (these are configurable):

- Jackpot = 3%
- Triple = 11%
- Double = 27%
- Halve = 49%
- Zero = 9%
- Explode = 1%

## How to use the gambling machine

While holding a scrap, walk up to the machine and press "E" (or your interaction key).
![Gambling machine](https://i.ibb.co/BV9BWRc/Gamba-machine.png)

## Features

- Gambling
- Configurable gambling chances
- Configurable gambling scrap multipliers
- Configurable machine layout (grid size, spacing, rotation, spawn mode, offsets)
- Configurable number of uses for each gambling machine
- Optional max value cap to prevent runaway scrap values
- Client configuration automatically syncs with the host
- Centralized music emitter with per-machine music toggle and volume control

## Configurable Fields

- General Settings
  - Cooldown Time (reducing this may desync the drumroll and can increase latency)
  - Number of uses per machine
  - Max value limit (cap scrap value after gambling; 0 disables the cap)

- Machine Layout
  - Machine spawn mode: AUTO (up to player count) or MAX (fill the grid capacity)
  - Number of rows (along X)
  - Machines per row (along Z)
  - Row spacing (distance between rows; decimals allowed; 0 defaults to 5)
  - Column spacing (distance between machines; decimals allowed; 0 defaults to 5)
  - Machine rotation (yaw 0-359; invalid values fall back to 90)
  - Layout offsets X/Y/Z (shift the whole grid anchor)

- Gambling Chances (make sure all values add up to 100 for sensible odds)
  - Jackpot Chance
  - Triple Chance
  - Double Chance
  - Halve Chance
  - Zero Chance
  - Explode Chance

- Gambling Multipliers
  - Jackpot Multiplier
  - Triple Multiplier
  - Double Multiplier
  - Halve Multiplier
  - Zero Multiplier
  - Explode Multiplier

- Audio (Client side)
  - Enable machine music
  - Machine music volume

## Bugs

Please report any bugs here:
https://github.com/Hamm3rhart/GamblingModLethalCompany/issues

Click on the <span style="color:green">*Green*</span> button that says <span style="color:green">*"New Issue"*</span> and type in the details of your bug!
