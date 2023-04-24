## goals/todo
- make things more readable
- current board types
    - square
    - slanted square
    - voronoi
- future board types
    - polygon (preserve adjacency and convexity while adding noise to lattice)
    - polygon (starting from rand voronoi and optimizing things)
    - voronoi with the adaptive meshing thing where some areas have smaller cells
- jiggle animation like https://www.networkpages.nl/CustomMedia/Animations/Games/Netsweeper/netsweeper.html
- graph coloring (and maybe 2nd order graph coloring)

## make it more readable by making small edges rare (voronoi)
- voronoi
    - move seeds while preserving adjacency
    - apply force from the smallest edge in the graph to its 2 seeds
    - apply force from the smallest edge in each cell to that 1 seed
    - apply force from all edges to its 2 seeds
        - force function considers all edges in the graph
    - apply force from all edges to its 1 seed
        - force function considers all edges in the cell
- polygon
    - get initial (convex) polygonization via voronoi probably
    - move vertexes while preserving adjacency and convexity
    - apply force from the smallest edge in the graph to its 2 endpoints
    - apply force from all edges to its 2 endpoints
        - force function considers all edges in the graph
- do these use {force and velocity preserved between time steps} or just {~force with no vel}

is it a good assumptions that polygons/shapes/cells should always be convex?
also read other things :)