# UnityCoverFlow
Unity3D UI CoverFlow and other Layout options

# Overview
Some years ago I was required to build a simple Cover Flow Layout (think, iTunes Carousel). Originally I build the project in a Windows Forms application for a client we were working with. Sometime later we then needed a similar system in a project we were doing in Unity3D. 

This is just a simple recreation of that work.

Its very old, and it wasn't originally done in GitHub so I've just commited the whole project in one commit.

There are some simple layouts included to demonstrate the flexibility of the system,

 * The classic Cover Flow layout (iTunes Album Artwork style)
 * A Carousel Layout (Z-Depth carousel)
 * A "Messy Paper" Layout - Cells shift from 1 messy pile to another

# Further Features
Cell reuse is supported using a simple Cell Pool with UICollectionCells registering Prefabs as "nibs" to be reused.

Data "binding" can be expanded upon with the cell reuse.

All layouts have various settings to tweak positions, speeds, snapping, wrapping and the like. These can also be updated at runtime in the editor to see results in real time.
