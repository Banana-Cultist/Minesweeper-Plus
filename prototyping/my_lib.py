from __future__ import annotations
from collections import defaultdict
import itertools
from typing import *
import math
# from termcolor import colored
import pygame
import random
import numpy as np
import matplotlib.pyplot as plt # type: ignore
from scipy.spatial import Voronoi
import shapely.geometry

class float2:    
    def __init__(self, x: float, y: float) -> None:
        self.x: Final = x
        self.y: Final = y
        self._hash: Optional[int] = None
    
    @staticmethod
    def modulo(theta: float) -> float:
        """Returns an angle congruent to theta but in the range [-pi, pi]."""
        return (theta + math.pi) % (math.pi * 2) - math.pi
    
    def __add__(self, a: float2) -> float2:
        return float2(self.x+a.x, self.y+a.y)
    
    def __sub__(self, a: float2) -> float2:
        return float2(self.x-a.x, self.y-a.y)

    def __mul__(self, a: float):
        return float2(self.x*a, self.y*a)
    __rmul__ = __mul__
    
    def __truediv__(self, a: float) -> float2:
        return float2(self.x/a, self.y/a)
    
    # def from_screen(self, screen_size: int) -> float2:
    #     return float2(x/screen_size_mul, y/screen_size_mul)
    
    # def to_screen_int(self) -> tuple[int, int]:
    #     return int(self.x*screen_size_mul), int(self.y*screen_size_mul)
    
    # def to_screen_float(self) -> tuple[float, float]:
    #     return self.x*screen_size_mul, self.y*screen_size_mul
    
    def length(self) -> float:
        return (self.x**2 + self.y**2)**.5
    
    def length2(self) -> float:
        return (self.x**2 + self.y**2)
    
    def normalized(self) -> float2:
        return self/self.length()

    def __repr__(self) -> str:
        return f"({self.x:.5f}, {self.y:.5f})"
    
    def __eq__(self, __value: object) -> bool:
        if isinstance(__value, float2):
            return self.x == __value.x and self.y == __value.y
        return False
    
    def __hash__(self) -> int:
        if self._hash is None:
            self._hash = hash((self.x, self.y))
        return self._hash

    @staticmethod
    def from_polar(r: float, theta: float) -> float2:
        return r*float2(math.cos(theta), math.sin(theta))
    
    def theta(self) -> float:
        if self.x == 0:
            if self.y < 0:
                return -.5 * math.pi
            elif self.y > 0:
                return .5 * math.pi
            else:
                raise ValueError
        else:
            return float2.modulo((math.pi * (self.x < 0)) + math.atan(self.y / self.x))
    
    def rotated(self, theta: float) -> float2:
        mag = self.length()
        if mag == 0:
            return float2.zero()
        theta += self.theta()
        return float2.from_polar(mag, theta)
    
    @staticmethod
    def proj_scalar(a: float2, b: float2) -> float:
        return float2.dot(a, b)/b.length()
    
    @staticmethod
    def project(a: float2, b: float2) -> float2:
        return float2.proj_scalar(a, b)*b.normalized()
        
    @staticmethod
    def zero() -> float2:
        return float2(0, 0)
    
    @staticmethod
    def one() -> float2:
        return float2(1, 1)
    
    @staticmethod
    def random() -> float2:
        """inside of the unit square"""
        return float2(random.random(), random.random())

    @staticmethod
    def dot(a: float2, b: float2) -> float:
        return a.x*b.x + a.y*b.y

    def draw(self, screen: pygame.surface.Surface, color: pygame.Color) -> None:
        pygame.draw.circle(screen, color, self.to_screen(screen), 3)
    
    def to_screen(self, screen: pygame.surface.Surface) -> tuple[float, float]:
        """mapping from unit square to square screen"""
        return (screen.get_width()*self).to_tuple()

    def to_tuple(self) -> tuple[float, float]:
        return self.x, self.y
    
    def to_tuple_int(self) -> tuple[int, int]:
        return int(self.x), int(self.y)

    @staticmethod
    def from_tuple(p: tuple[float, float]) -> float2:
        return float2(p[0], p[1])
        
    
    def to_complex(self) -> complex:
        return complex(self.x, self.y)
    
    def __getitem__(self, key: int) -> float:
        if key == 0:
            return self.x
        elif key == 1:
            return self.y
        raise IndexError

    def between(self, lo: float2, hi: float2) -> bool:
        return (lo.x < self.x < hi.x) and (lo.y < self.y < hi.y)

    def in_square(self, l: float = 1) -> bool:
        return (
            (0 <= self.x <= 1) and
            (0 <= self.y <= 1)
        )
    
    # def __gt__(self, other: float2) -> bool:
    #     if self.x == other.x:
    #         return self.y > other.y
    #     return self.x > other.x


def voronoi_finite_polygons_2d(vor: Voronoi) -> tuple[list[list[int]], list[tuple[float, float]]]:
    """
    Reconstruct infinite voronoi regions in a 2D diagram to finite
    regions.
    Parameters
    ----------
    vor : Voronoi
        Input diagram
    radius : float, optional
        Distance to 'points at infinity'.
    Returns
    -------
    regions : list of tuples
        Indices of vertices in each revised Voronoi regions.
    vertices : list of tuples
        Coordinates for revised Voronoi vertices. Same as coordinates
        of input vertices, with 'points at infinity' appended to the
        end.
    """

    if vor.points.shape[1] != 2:
        raise ValueError("Requires 2D input")

    new_regions: list[list[int]] = []
    new_vertices = vor.vertices.tolist()

    center = vor.points.mean(axis=0)
    # radius = vor.points.ptp().max()*2
    radius = 2

    # Construct a map containing all ridges for a given point
    # all_ridges: dict[int, list[tuple[int, int, int]]] = {}
    # for (p1, p2), (v1, v2) in zip(vor.ridge_points, vor.ridge_vertices):
    #     all_ridges.setdefault(p1, []).append((p2, v1, v2))
    #     all_ridges.setdefault(p2, []).append((p1, v1, v2))
    all_ridges: dict[int, list[tuple[int, int, int]]] = defaultdict(list)
    for (p1, p2), (v1, v2) in zip(vor.ridge_points, vor.ridge_vertices):
        all_ridges[p1].append((p2, v1, v2))
        all_ridges[p2].append((p1, v1, v2))

    # Reconstruct infinite regions
    region: int
    for p1, region in enumerate(vor.point_region):
        vertices: list[int] = vor.regions[region]

        if all(v >= 0 for v in vertices):
            # finite region
            new_regions.append(vertices)
            continue

        # reconstruct a non-finite region
        ridges = all_ridges[p1]
        new_region = [v for v in vertices if v >= 0]

        for p2, v1, v2 in ridges:
            if v2 < 0:
                v1, v2 = v2, v1
            if v1 >= 0:
                # finite ridge: already in the region
                continue

            # Compute the missing endpoint of an infinite ridge

            t = vor.points[p2] - vor.points[p1] # tangent
            t /= np.linalg.norm(t)
            n = np.array([-t[1], t[0]]) # normal

            midpoint = vor.points[[p1, p2]].mean(axis=0)
            direction = np.sign(np.dot(midpoint - center, n)) * n
            far_point = vor.vertices[v2] + direction * radius

            new_region.append(len(new_vertices))
            new_vertices.append(far_point.tolist())

        # sort region counterclockwise
        vs = np.asarray([new_vertices[v] for v in new_region])
        c = vs.mean(axis=0)
        angles = np.arctan2(vs[:,1] - c[1], vs[:,0] - c[0])
        new_region = np.array(new_region)[np.argsort(angles)].tolist()

        # finish
        new_regions.append(new_region)
    # print(type(new_regions[0][0]))
    # print(type(new_vertices[0][0]))
    # print(type(list(all_ridges.items())[0][1][0][0]))
    # dict[np.int32, list[tuple[np.int32, ...]]]
    return new_regions, new_vertices

Vertex_i = int
Edge_i = int
Cell_i = int
Color_i = int

class Edge:
    def __init__(
        self,
        index0: Vertex_i,
        index1: Vertex_i,
    ) -> None:
        if index0 > index1:
            index0, index1 = index1, index0
        self.index0: Final = index0
        self.index1: Final = index1
    
    def __getitem__(self, key: int) -> float:
        if key == 0:
            return self.index0
        elif key == 1:
            return self.index1
        raise IndexError
    

class Polygon:
    def __init__(
        self,
        # center: float2, # average of vertices bc thats cheap
        vertexes: list[Vertex_i], # sorted s.t. it is convex
        edges: set[Edge_i],
        neighbors: set[Cell_i],
        second_neighbors: set[Cell_i],
        color_i: Color_i,
    ) -> None:
        # self.center = center
        self.vertexes: Final = vertexes
        self.edges: Final = edges
        self.neighbors: Final = neighbors
        self.second_neighbors: Final = second_neighbors
        self.color_i: Final = color_i
    
    def center(self, vertexes: dict[Vertex_i, float2]) -> float2:
        return sum(
            (
                vertexes[vertex_i]
                for vertex_i in self.vertexes
            ),
            start=float2.zero()
        ) / len(self.vertexes)
    
class VorPolygon(Polygon):
    def __init__(
        self,
        seed: float2,
        # center: float2, # average of vertices
        vertexes: list[Vertex_i], # sorted s.t. it is convex
        edges: set[Edge_i],
        neighbors: set[Cell_i],
        second_neighbors: set[Cell_i],
        color_i: Color_i,
    ) -> None:
        super().__init__(
            # center,
            vertexes,
            edges,
            neighbors,
            second_neighbors,
            color_i,
        )
        self.seed = seed

class Polygonization:
    def __init__(
        self,
        vertexes: dict[Vertex_i, float2],
        edges: dict[Edge_i, Any],
        cells: dict[Cell_i, Polygon],
        colors: dict[Color_i, pygame.Color],
    ) -> None:
        self.vertexes = vertexes
        self.edges: Final = edges
        self.cells: Final = cells
        self.colors: Final = colors

Seed_i = int

class VorPolygonization(Polygonization):
    def __init__(
        self,
        seeds: dict[Seed_i, float2],
        vertexes: dict[Vertex_i, float2],
        edges: dict[Edge_i, Any],
        cells: dict[Cell_i, VorPolygon],
        colors: dict[Color_i, pygame.Color],
    ) -> None:
        super().__init__(
            vertexes,
            edges,
            cells,
            colors,
        )
        self.seeds = seeds
    
    @staticmethod
    def from_seeds(seeds: list[float2]) -> VorPolygonization:
        unbounded_polygons, unbounded_vertexes = voronoi_finite_polygons_2d(Voronoi([
            seed.to_tuple()
            for seed in seeds
        ]))

        # vertexes: list[float2] = [
        #     float2.from_tuple(v)
        #     for v in unbounded_vertexes
        #     if float2.from_tuple(v).in_square()
        # ]
        # polygons: list[set[int]] = list()
        
        clip_domain: Final = shapely.geometry.Polygon([[0, 0], [0, 1], [1, 1], [1, 0]])
        clipped_polygons: list[list[float2]] = list()
        for region in unbounded_polygons:
            poly: shapely.geometry.Polygon = shapely.geometry.Polygon([
                unbounded_vertexes[vertex_i]
                for vertex_i in region
            ]).intersection(clip_domain)
            polygon = [float2.from_tuple(p) for p in poly.exterior.coords]
            clipped_polygons.append(polygon)
            
        vertexes: list[float2] = list()
        polygons: list[set[int]] = list()
        for polygon in clipped_polygons:
            polygons.append(set())
            for vertex in polygon:
                try:
                    vertex_i = vertexes.index(vertex)
                except ValueError:
                    vertexes.append(vertex)
                    vertex_i = vertexes.index(vertex)
                polygons[-1].add(vertex_i)

        for vertex0, vertex1 in itertools.combinations(vertexes, r=2):
            assert (vertex0-vertex1).length2() > .0001
        
        # center_polygons: list[tuple[float2, set[int]]]]
        
        sorted_polygons: list[tuple[float2, list[int]]] = [
            (
                center := sum(
                    (
                        vertexes[vertex_i]
                        for vertex_i in polygon
                    ),
                    start=float2.zero()
                ) / len(polygon),
                sorted(
                    polygon,
                    key=lambda vertex_i: (vertexes[vertex_i]-center).theta()
                )
            )
            for polygon in polygons
        ]
        print()
        edges: dict[Edge_i, Edge]
        cells: dict[Cell_i, Polygon]
        # return Board(
        #     {
        #         vertex_i: float2(*vertex)
        #         for vertex_i, vertex in enumerate(vertexes)
        #     },
            
        # )

def main() -> None:
    board = Polygonization.as_voronoi([
        float2.random()
        for _ in range(10)
    ])

# def test() -> None:
#     # make up data points
#     np.random.seed(1234)
#     seeds = np.random.rand(15, 2)

#     # compute Voronoi tesselation
#     vor = Voronoi(seeds)

#     # plot
#     regions, vertices = voronoi_finite_polygons_2d(vor)

#     # mins = np.tile([0, 0], (len(vertices), 1))
#     # bounded_vertices = np.max((vertices, mins), axis=0)
#     # maxs = np.tile([1, 1], (len(vertices), 1))
#     # bounded_vertices = np.min((bounded_vertices, maxs), axis=0)

#     clip_domain = Polygon([[0, 0], [0, 1], [1, 1], [1, 0]])

#     # colorize
#     for region in regions:
#         # polygon = vertices[region]
#         polygon: list[tuple[float, float]] = [vertices[vertex_i] for vertex_i in region]
        
#         # Clipping polygon
#         poly = Polygon(polygon)
#         poly = poly.intersection(clip_domain)
#         polygon = [p for p in poly.exterior.coords]
#         plt.fill(*zip(*polygon), alpha=0.4)

#     # plt.plot(points[:, 0], points[:, 1], 'ko')
#     plt.axis('equal')
#     plt.xlim(-.1, 1.1)
#     plt.ylim(-.1, 1.1)

#     plt.savefig('voro.png')
#     plt.show()

if __name__ == '__main__':
    main()
    # test()