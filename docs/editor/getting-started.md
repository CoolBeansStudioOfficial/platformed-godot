[← Previous](https://github.com/CoolBeansStudioOfficial/platformed-godot)

---

# Getting Started

## Introduction

The Platformed app implements a robust, beginner-friendly level editor. 

> [!IMPORTANT]
> If you are already familiar with the web editor, be aware that the app has significant changes compared to it. These changes were made in the interest of making level editing more intuitive for new users.

## The Interface

### Top Bar

#### Level Name

The name of the level you are editing is displayed on the very left of the top bar. 

You can click on the name to edit it:

![GIF](https://github.com/user-attachments/assets/8a0ad64c-5833-453c-bc7d-7cff5c1ab934)

#### Zoom Control

The zoom controls are located closest to the center of the top bar. The controls provide two methods for adjusting how zoomed in your view of the level is.

Increase/decrease zoom buttons:

![zoom-click](https://github.com/user-attachments/assets/4168f3b8-5d77-4eae-8c27-4e43ea2302aa)

Drag slider to zoom:

![zoom-drag](https://github.com/user-attachments/assets/cc2ba1f8-7411-42ef-b2ce-1486a85f4082)

> [!TIP]
> You can also adjust your zoom level with the scroll wheel by holding `Ctrl`.

#### Undo/Redo

The undo and redo buttons are located immediately to the right of the zoom controls. They allow you to move back and forth in the chain of edits you have made to your level.

The buttons will grey out when you reach their respective ends of the chain.

![undo-redo](https://github.com/user-attachments/assets/52b730b6-4eaa-4db1-a8e4-f438f0f20f82)

> [!TIP]
> You can also use `Ctrl + Z` for undo and `Ctrl + Shift + Z` for redo.

#### Save Options

The save options button is located inbetween the redo and play buttons. Upon pressing it, you will be presented with three options for how you want to save your level.

![save-options](https://github.com/user-attachments/assets/5c2fe46e-5ce1-4fb4-af71-133c3cd1202d)

##### Save to levels folder:

This option will save your level as a .json file to the folder you have set as your levels folder. This method will fail if no levels folder has been set.

> [!TIP]
> You can also save in this manner by pressing `Ctrl + S`

##### Save as:

This option will save your level as a .json file to any location you choose. You may also change the filename during the selection.

##### Upload to web:

This option will upload your level **PUBLICLY** to the Platformed website. Uploading only works if you are signed in.

#### Play Level

The play button is the second rightmost button, inbetween the save options and level settings buttons. Pressing it puts you into play mode, allowing you to test your level without saving it.

![play-button](https://github.com/user-attachments/assets/4ea35d72-ce57-4cb3-8ec2-17baeb574dbe)

> [!WARNING]
> If the player's spawn point is not set with a spawn flag, they will spawn in the top left corner by default.

#### Level Settings

By clicking the level settings button in the very right of the top bar, you can change the configuration of your level in multiple ways.

##### Resize Level

You can resize your level up to a maximum of 200 x 100 blocks.

**GIF HERE**

> [!CAUTION]
> If you resize a level to be smaller, any blocks outside of the new size will be lost.

##### Wall Jumping

You can choose the way in which you want the player be able to wall jump. Your options are no wall jumps, wall jumps that allow you to scale walls, and wall jumps that only allow you to jump directly off of walls.

**GIF HERE**

### Side Bar

#### Blocks

#### Category Toggles

#### Eraser

### Context Menu

## Placing Blocks



## Moving Blocks

There are multiple methods you can use to move and duplicate groups of blocks.

### Using the mouse

Click and drag to select all of the blocks in an area.

**GIF HERE**

Once you have made your selection, you can drag it to where you want to move it.

**GIF HERE**

### Cut/Copy/Paste

---

[Next →](https://github.com/CoolBeansStudioOfficial/platformed-godot)
