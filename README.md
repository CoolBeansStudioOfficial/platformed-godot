![Banner](https://github.com/user-attachments/assets/9e589bd7-5d03-401a-9920-278a9a73950c)

#### A native desktop app made in Godot for [Platformed](https://platformed.jmeow.net) that lets you play, create, and upload levels via its API.

## Features

The Platformed app boasts full feature parity with its web counterpart. It also adds exclusive quality-of-life features and improvements.

| Feature      | Desktop | Web     |
| :---        |   :---:    |       :---:     |
| Browse public levels      | ✅️       | ✅️   |
| Play any level   | ✅️        | ✅️      |
| Level editor   | ✅️        | ✅️      |
| Upload levels   | ✅️        | ✅️      |
| Custom themes   | ✅️        | ➖      |
| Easy account switching   | ✅️        | ❌      |
| Beginner-friendly editing workflow   | ✅️        | ➖      |
| Block category toggles   | ✅️        | ❌      |
| In-game tileset switching   | ✅️        | ❌      |
| 67   | ❌        | ❌      |

## Documentation

### Play

[Getting Started](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/play/getting-started.md)

[Account & My Levels](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/play/my-levels.md)

### Editor

[Getting Started](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/editor/getting-started.md)

[Using Triggers](https://github.com/CoolBeansStudioOfficial/platformed-godot/blob/main/docs/editor/triggers.md)

## Optimization

Making a desktop app for Platformed would be pretty pointless if it didn't perform any better than the web version, so I put a lot of effort into optimization.

### Play Mode

During early development, each individual block was its own node. This was inefficient because it resulted in thousands of instances, all with C# scripts attatched.

Levels with a particularly large number of blocks (like Level 2) took as long as 8 seconds to load.

https://github.com/user-attachments/assets/97ccdd66-58ae-40c8-990e-9d8e2869d5ef

To remedy this, I was able to take advantage of Godot 4's TileMapLayer system to spawn most blocks in as pre-defined tiles that do not require their own node instances.

As a result, level generation is now instant.

https://github.com/user-attachments/assets/036fdff7-355c-4a16-a461-1c7890059857

Blocks that needed to be intangible but still detect collisions (i.e. triggers) unfortunately still had to be their own nodes.

### Edit Mode

While play mode's initial performance was acceptable apart from loading, edit mode was not at all so.

https://github.com/user-attachments/assets/c9960bb5-0970-4b5f-88df-5c743a2b36a6

Luckily, I was able to apply the same optimization to the editor with a few extra performance savings. All tiles that normally rely upon being scenes in play mode are able to be spawned as pre-defined tiles, and the collision layer is omitted to skip physics calculations entirely.

Now, huge amounts of different blocks can all be mass edited smoothly

https://github.com/user-attachments/assets/e3337e16-b0b2-4110-83da-d9248300f2a0
