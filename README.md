# Unity Collision Masking Demo
A small demo project showcasing a simple collision masking implementation in Unity. In this project, the colliders of two Tilemaps are able to be "masked" using a circular mask controlled by the mouse. One Tilemap only has collision within the circle, and the other Tilemap only has collision outside of the circle.

# How it Works
When the scene is first played, a couple of things happen:
- A pool of 'loose' colliders is spawned which will be used to handle collision within the mask. This pool needs to consist of enough colliders so that even in the most complicated scenarios every block will have collision as needed. The required amount of colliders to achieve this increases with the radius of the mask.
- A third 'mixed' Tilemap is filled with tiles based on the 'outer' Tilemap (the Tilemap that is visible and active only outside of the mask). This mixed tilemap is what the player (or any other physics objects) will actually collide with. The original inner and outer Tilemaps are used only for the visual aspect, and for a reference as to which tiles need collision. The physics layers of the inner and outer Tilemaps are set to not collide with the player.

After the initial setup, the circular mask (called the 'kaleidoscope' in the code due to a different project this functionality was made for) will follow the mouse and update collision every frame. It does this in a few different steps:
- Tiles on the mixed Tilemap must be updated based on their proximity to the mask. Tiles within the mask or overlapping the edges of it have collision disabled, as their collision will be handled by a loose collider from the pool. Tiles entirely outside of the mask have their collision enabled ONLY if their corresponding tile on the outer Tilemap has a collider.
- Loose colliders from the pool are activated and positioned into place as needed in order to build the colliders within the mask. Once a loose collider (called a PartialBlock in the code) is enabled, it will update its own shape every frame as needed. A PartialBlock can either be set as 'normal' or 'reversed'. If set to normal, it will represent a cut-off part of an outer tile along the edge of the mask, and will shape itself to line up with where the mask cuts off the tile. If set to reversed, it will represent an inner tile within the bounds of the mask. This can either be cut-off along the edge of the mask, or a full square within the mask. Either way, it will shape itself accordingly.

# Calculating the Shape of the PartialBlock
A PartialBlock calculates its own shape by determining which points need to be added to the shape. These points can include any of the four corners of the full tile square, or any points where the edge of the mask intersects the edge of the square tile. Whether or not a corner point is included is determined by whether or not that point is inside of the mask. If the PartialBlock is 'normal', it only includes corners outside of the mask. If the PartialBlock is 'reversed', it only includes corners inside of the mask. Any point where the mask intersects one of the edges of the PartialBlock is always included.

The points where the mask intersects the tile's edges are calculated based on the standard equation of a circle. This calculation is only performed for each edge if one of the corners touching that edge is inside the mask, and the other is outside the mask (meaning the mask is guaranteed to be intersecting that edge somewhere).

# Improvements That Could be Made
- The current version of this project is specific to a circular mask shape. But it could be modified to handle other simple mask shapes, such as rectangles. A similar concept of finding the points where the mask intersects the tile could be used, with an additional point used in the case that a corner of the rectangle is within a tile.
- It is possible that this could be optimized further so that not every collider has to update every frame.
