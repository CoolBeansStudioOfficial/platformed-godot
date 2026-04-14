[← Previous](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/editor/getting-started.md)

**Using Triggers**

---

# Introduction

Platformed implements a trigger sytem to allow you to add more complexity and mechanical depth to your levels.

A trigger is an intangible block that executes commands when the player passes through it.

|   Trigger One    | Invisible Trigger | Trigger Three     |
| :---:        |   :---:    |       :---:     |
| image      | image      | image   |


# Commands

There are several different types of commands that a trigger can be populated with.



## Command Types


### Toggle Red/Blue Blocks

The **toggle red/blue blocks** command will disable the currently active block type. If red is active, blue becomes active instead (and vice-versa).

![toggle](https://github.com/user-attachments/assets/ddaac2ec-33f4-47da-92fd-95a40b6342e4)

### Teleport Player

The **teleport player** command will teleport the player to position `X, Y`.

![teleport](https://github.com/user-attachments/assets/7f1d9a9c-908c-45bf-95d8-eacfb4202af2)


### Rotate Block

The **rotate block** command will set the rotation of the block at position `X, Y` to `Rotation`.

![rotate](https://github.com/user-attachments/assets/87777001-e621-48b9-9189-cd1cc1ce3e71)

### Change Block

The **change block** command will set the block at position `X, Y` to `Block`, replacing whatever block was there.

![change](https://github.com/user-attachments/assets/240faba5-9e83-4e29-88e6-e036a2ab582b)

### Delay

The **delay** command will pause the trigger from executing commands for `number` milliseconds.

![delay](https://github.com/user-attachments/assets/ec5e09a9-69b7-4044-99cd-81845e50567e)

> [!NOTE]
> 1 second = 1,000 milliseconds

### Fill Area

The **fill area** command will replace all the blocks from start position `X, Y` to end position `X, Y` with `Block`.

![fill](https://github.com/user-attachments/assets/0e687dd8-4bbf-4157-b159-13f0f0168afc)

## Adding Commands

To add a command to a trigger

---

[Return to Main Page →](https://github.com/CoolBeansStudioOfficial/platformed-godot)
