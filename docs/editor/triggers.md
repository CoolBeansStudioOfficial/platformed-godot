[← Previous](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/editor/getting-started.md)

**Using Triggers**

---

# Introduction

Platformed implements a trigger sytem to allow you to add more complexity and mechanical depth to your levels.

# Commands

A trigger contains a list of commands to execute when the player touches it.

The different types of commands are as follows:


## Toggle Red/Blue Blocks

The **toggle red/blue blocks** command will disable the currently active block type. If red is active, blue becomes active instead (and vice-versa).

## Teleport Player

The **teleport player** command will teleport the player to position `X, Y`.

## Rotate Block

The **rotate block** command will set the rotation of the block at position `X, Y` to `Rotation`.

## Change Block

The **change block** command will set the block at position `X, Y` to `Block`, replacing whatever block was there.

## Delay

The **delay** command will pause the trigger from executing commands for `number` milliseconds.

> [!NOTE]
> 1 second = 1,000 milliseconds

## Fill Area

The **fill area** command will replace all the blocks from start position `X, Y` to end position `X, Y` with `Block`.

---

[Return to Main Page →](https://github.com/CoolBeansStudioOfficial/platformed-godot)
