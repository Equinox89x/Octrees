# Octrees in Unity

This project is about the implementation of a modular Octree that generates itself on am object, or withoout one.

## Introduction

The project contains a small world that has been divided into sections until a certain recursive depth using Octrees.
These sections can then be classified to any sort of area you want (an area = multiple octree world nodes).
These areas are defined by all the objects that are in it that are under an Area group gameobject.

Furthermore, this project also has dynamic (moving items) like a plane and players, to show that you can remove/add/get any info from any node, or from parent nodes.
This gives you the power of spatial partinioning then as you can divide many players into each node(s) and only loop through the players (or other things) in those nodes.

There is more to tell, so just check out the project.

## Content
### World
In the demo, you can find that there is an octree generated on the "floor" gameobject.
The octree will be generated using the width and depth of the floor gameobject, meaning the whole world will be covered with an octree.

### Static Objects
In the world, there are static objects, some of these are Areas, these are area in the game world this can be used to for example capture an area.
The areas in the world are also easily extendable. You'll see the each area in the game is in a gameobject called "Area (x)" x being a number, but you could call this any name you'd like. Then every object that is within this folder will make the worldoctree see the node where this object is in and classify that node as a node that is within "Area (x)" this is visualised in the debug, each area has their different color.

### Dynamic Objects
A dynamic object, for example a player, enemy, and any other moving object (a plane in my case) can also have it's own octree.
The octrees in these dynamic items can be used for many things. Meaning that when a bullet enters your octree node system, you can see this bullet, but you can only check the collision once the bullet enters the lowest level nodes within this tree, closest to the player. This allows the game to save calculations and thus performance.

## World and Dynamic object mixing
Every dynamic object is detectable by the world octree.
This means that the world octree knows where a certain object is.
This allows you to for example locate an object, but it can also be used to add a post processing filter whenever you enter a certain Area.
Say you enter an area that is radioactive, then you can add a vignette around the player that is light green, to visualize radioactive zones.

Besides that, 
