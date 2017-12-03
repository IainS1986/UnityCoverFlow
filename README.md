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


# Demos

Here's a few GIFs showing the layouts in action in the editor (GIFs are only at 30fps and appear to have bad artifacts in them, running in the editor is obviously at full FPS with no rendering issues).

* Cover Flow Layout
![Cover Flow](https://github.com/IainS1986/UnityCoverFlow/blob/master/GIFs/CoverFlow.gif)

* Carousel Layout
![Carousel Flow](https://github.com/IainS1986/UnityCoverFlow/blob/master/GIFs/CarouselFlow.gif)

* Messy Paper Layout
![Messy Paper Flow](https://github.com/IainS1986/UnityCoverFlow/blob/master/GIFs/MessyPaperFlow.gif)

* Layouts can have multiple configurable elements, here's an example of the Cover Flow properties being edited at runtime...
![Editable Properties](https://github.com/IainS1986/UnityCoverFlow/blob/master/GIFs/CoverFlowEditor.gif)