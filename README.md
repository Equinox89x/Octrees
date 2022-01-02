# Octrees in Unity

This project is about the implementation of a modular Octree that generates itself on am object, or withoout one.

## Introduction

The project contains a small world that has been divided into sections until a certain recursive depth using Octrees.
These sections can then be classified to any sort of area you want (an area = multiple octree world nodes).
These areas are defined by all the objects that are in it that are under an Area group gameobject.

Furthermore, this project also has dynamic (moving items) like a plane and players, to show that you can remove/add/get any info from any node, or from parent nodes.
This gives you the power of spatial partinioning then as you can divide many players into each node(s) and only loop through the players (or other things) in those nodes.

There is more to tell, so just check out the project.