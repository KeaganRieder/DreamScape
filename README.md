# DreamScape

## Contents
- [Overview](#Overview)
- [Assignment Overview](#Assignment-Overview)
- [Assignment Display](#Assignment-Display)
- [How To Use](#How-To-Use)

## Overview
DreamScape is the programming half of my groups [Assignment 1](#Assignment-Overview) for New Media 3680-Interaction design.
The program is meant to provide both visual and auditory feed back based on how users interacted with the other half of the project, which was a [physical object](#Assignment-Display) that was design by my group members. 

### Assignment Overview
This group project challenges you to explore the dynamic interplay between the tangible 
(physical) and the ineffable (intangible or abstract) using the Makey Makey platform. Your 
work should investigate and create meaningful relationships between physical interactions 
and virtual outputs, engaging both your group and the audience in imaginative and critical 
ways

#### Key Goals: 
- Experiment with physical and virtual affordances. 
- Create an interactive work that embodies the theme Tangible & Ineffable. 
- Showcase curiosity, imagination, and critical thinking in both process and outcome.

## Assignment Display
The assignment was split into two half a programming and physical elements. the physical element was a cloud which made up of different segments that allowed for a user to interact. 

![Project Set up](/docs/IMG_4418.jpg)

These interactions were then turned into computer input by a Makey Makey, which then sent the input to the program and used to provide feedback to the user, in the form of visual changes to an image that was display on a project, and random sound playing from speakers.

## How To Use
The program can be used with out the physical element. this requires downloading the program and opening in [godot](https://godotengine.org/download/windows/), if you need to download it ensure it's the .net version.

once you the program in godot you then can hit the compile and play, to run the program. The controls are as followed:
```C#
// mouse click, - plays ambient music and creates an image that stays
// and allows for other inputs to act differently, by altering the currently display 
// image
{"PlayAmbientMusic",new InputEventMouseButton { ButtonIndex = MouseButton.Left}}, 

// makes new random img with perlin noise
{"NewPerlinDream", new InputEventKey{PhysicalKeycode = Key.Down}}, 

// randomizes frequency used by generator noise 
{"NewDream",  new InputEventKey{PhysicalKeycode = Key.Up}},

// uses last generated img
{"MostRecentDream", new InputEventKey{PhysicalKeycode = Key.Space}},

// randomizes the color pallet
{"NewPalette",  new InputEventKey{PhysicalKeycode = Key.Left}}, 

// inverts the images colors
{"InvertedDream", new InputEventKey{PhysicalKeycode = Key.Right}}, 

// exits out of the program
{"Quit", new InputEventKey{PhysicalKeycode = Key.Escape}}, 
```
