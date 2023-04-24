from __future__ import annotations
from typing import *
import math
# from termcolor import colored
import pygame
import random

class int2:
    def __init__(self, x: int, y: int) -> None:
        self.x: Final = x
        self.y: Final = y
        self._hash: Optional[int] = None
    
    # @staticmethod
    # def modulo(theta: int) -> int:
    #     """Returns an angle congruent to theta but in the range [-pi, pi]."""
    #     return (theta + math.pi) % (math.pi * 2) - math.pi
    
    def __add__(self, a: int2) -> int2:
        return int2(self.x+a.x, self.y+a.y)
    
    def __sub__(self, a: int2) -> int2:
        return int2(self.x-a.x, self.y-a.y)

    def __mul__(self, a: int):
        return int2(self.x*a, self.y*a)
    __rmul__ = __mul__
    
    # def __truediv__(self, a: int) -> int2:
    #     return int2(self.x/a, self.y/a)
    
    def __floordiv__(self, a: int) -> int2:
        return int2(self.x//a, self.y//a)
    
    def length(self) -> int:
        return (self.x**2 + self.y**2)**.5
    
    def length2(self) -> int:
        return (self.x**2 + self.y**2)
    
    # def normalized(self) -> int2:
    #     return self/self.length()

    def __repr__(self) -> str:
        return f"({self.x}, {self.y})"
    
    def __eq__(self, __value: object) -> bool:
        if isinstance(__value, int2):
            return self.x == __value.x and self.y == __value.y
        return False
    
    def __hash__(self) -> int:
        if self._hash is None:
            self._hash = hash((self.x, self.y))
        return self._hash

    # @staticmethod
    # def from_polar(r: int, theta: int) -> int2:
    #     return r*int2(math.cos(theta), math.sin(theta))
    
    # def theta(self) -> int:
    #     if self.x == 0:
    #         if self.y < 0:
    #             return -.5 * math.pi
    #         elif self.y > 0:
    #             return .5 * math.pi
    #         else:
    #             raise ValueError
    #     else:
    #         return int2.modulo((math.pi * (self.x < 0)) + math.atan(self.y / self.x))
    
    # def rotated(self, theta: int) -> int2:
    #     mag = self.length()
    #     if mag == 0:
    #         return int2.zero()
    #     theta += self.theta()
    #     return int2.from_polar(mag, theta)
    
    # @staticmethod
    # def proj_scalar(a: int2, b: int2) -> int:
    #     return int2.dot(a, b)/b.length()
    
    # @staticmethod
    # def project(a: int2, b: int2) -> int2:
    #     return int2.proj_scalar(a, b)*b.normalized()
        
    @staticmethod
    def zero() -> int2:
        return int2(0, 0)
    
    @staticmethod
    def one() -> int2:
        return int2(1, 1)
    
    @staticmethod
    def random(width: int) -> int2:
        """inside of the square with length width"""
        return int2(random.randrange(width), random.randrange(width))

    # @staticmethod
    # def dot(a: int2, b: int2) -> int:
    #     return a.x*b.x + a.y*b.y

    # def draw(self, screen: pygame.surface.Surface, color: pygame.Color) -> None:
    #     pygame.draw.circle(screen, color, self.to_tuple(), 3)
    
    def draw_square(
        self,
        screen: pygame.surface.Surface,
        color: pygame.Color,
        screen_ratio: float,
    ) -> None:
        pygame.draw.rect(
            screen,
            color,
            pygame.Rect(
                screen_ratio*self.x,
                screen_ratio*self.y,
                screen_ratio,
                screen_ratio,
            )
        )
    
    def to_screen(self, screen_ratio: float) -> tuple[float, float]:
        return (
            screen_ratio*self.x,
            screen_ratio*self.y,
        )
    
    def to_tuple(self) -> tuple[int, int]:
        return self.x, self.y
        
    def __getitem__(self, key: int) -> int:
        if key == 0:
            return self.x
        elif key == 1:
            return self.y
        raise IndexError

    # def between(self, lo: int2, hi: int2) -> bool:
    #     return (lo.x < self.x < hi.x) and (lo.y < self.y < hi.y)
    
    # def __gt__(self, other: int2) -> bool:
    #     if self.x == other.x:
    #         return self.y > other.y
    #     return self.x > other.x
    
def color_lerp(
    color0: pygame.Color,
    color1: pygame.Color,
    t: float,
) -> pygame.Color:
    # hsla maybe better than hsva
    assert 0 <= t <= 1
    color = pygame.Color(0, 0, 0)
    color.hsla = tuple(
        c0*(1-t) + c1*t
        for c0, c1 in zip(color0.hsla, color1.hsla, strict=True)
    ) # type: ignore
    return color

Pixel = int2
Cell_i = int
Color_i = int

class RasterVoronoi:
    def __init__(
        self,
        width: int,
        pixels: list[list[Cell_i]],
        cells: dict[Cell_i, set[Pixel]],
        centroids: dict[Cell_i, Pixel],
        border: set[Pixel],
        neighborhood: dict[Cell_i, set[Cell_i]],
    ) -> None:
        self.width: Final = width
        self.pixels: Final = pixels
        self.cells: Final = cells
        self.centroids: Final = centroids
        self.border: Final = border
        self.neighborhood: Final = neighborhood
    
    @staticmethod
    def centroids_from_cells(
        cells: dict[Cell_i, set[Pixel]]
    ) -> dict[Cell_i, Pixel]:
        return {
            cell_i: sum(cell, start=int2.zero())//len(cell)
            for cell_i, cell in cells.items()
        }
    
    @staticmethod
    def from_seeds(width: int, seeds: list[int2]) -> RasterVoronoi:
        pixels: list[list[Optional[Cell_i]]] = [
            [None for col in range(width)]
            for row in range(width)
        ]
        cells: dict[Cell_i, set[Pixel]] = {
            i: set()
            for i in range(len(seeds))
        }
        border: set[Pixel] = set()
        neighborhood: dict[Cell_i, set[Cell_i]] = {
            i: set()
            for i in range(width)
        }
        for row in range(width):
            for col in range(width):
                pixel = int2(col, row)
                cell_i: Cell_i = min(
                    range(len(seeds)),
                    key=lambda i: int2(seeds[i].x-col, seeds[i].y-row).length2()
                )
                pixels[row][col] = cell_i
                cells[cell_i].add(pixel)
                # if row == 0 or col == 0:
                #     continue
                for neighbor_col, neighbor_row in [
                    (col, row-1),
                    (col-1, row),
                ]:
                    if neighbor_col < 0:
                        continue
                    if neighbor_row < 0:
                        continue
                    neighbor_i = pixels[neighbor_row][neighbor_col]
                    if neighbor_i == cell_i:
                        continue
                    assert neighbor_i is not None
                    border.add(pixel)
                    border.add(int2(neighbor_col, neighbor_row))
                    neighborhood[cell_i].add(neighbor_i)
                    neighborhood[neighbor_i].add(cell_i)

        # finish the border
        for pixel in border.copy():
            for neighbor_col, neighbor_row in [
                (pixel.x-1, pixel.y),
                (pixel.x+1, pixel.y),
                (pixel.x, pixel.y-1),
                (pixel.x, pixel.y+1),
            ]:
                if not (
                    0 <= neighbor_row < width and
                    0 <= neighbor_col < width
                ):
                    continue
                neighbor_i = pixels[neighbor_row][neighbor_col]
                if neighbor_i == pixels[pixel.y][pixel.x]:
                    continue
                assert neighbor_i is not None
                border.add(int2(neighbor_col, neighbor_row))

        assert all(
            all(
                cell_i is not None
                for cell_i in line
            )
            for line in pixels
        )
        return RasterVoronoi(
            width,
            pixels, # type: ignore
            cells,
            RasterVoronoi.centroids_from_cells(cells),
            border,
            neighborhood,
        )

class Board:
    # this should probably be the same class as PlanePartition
    def __init__(
        self,
        plane_partition: RasterVoronoi,
        coloring: dict[Cell_i, Color_i],
        colors: dict[Color_i, pygame.Color],
    ) -> None:
        self.width: Final = plane_partition.width
        self.plane_partition: Final = plane_partition
        self.coloring: Final = coloring
        self.colors: Final = colors
    
    @staticmethod
    def from_colors(
        plane_partition: RasterVoronoi,
        colors: dict[Color_i, pygame.Color],
    ) -> Board:
        # coloring = {
        #     cell_i: cell_i % len(colors)
        #     for cell_i in range(len(plane_partition.cells))
        # }
        coloring: dict[Cell_i, Optional[Color_i]] = {
            cell_i: None
            for cell_i in range(len(plane_partition.cells))
        }
        color_counts: dict[Color_i, int] = {
            color_i: 0
            for color_i in range(len(colors))
        }
        uncolored_cells: set[Cell_i] = set(range(len(plane_partition.cells)))
        while uncolored_cells:
            cell = uncolored_cells.pop()
            available_colors: set[Color_i] = set(range(len(colors)))
            for neighbor in plane_partition.neighborhood[cell]:
                neighbor_color = coloring[neighbor]
                if neighbor_color is None:
                    continue
                available_colors -= {neighbor_color}
            if len(available_colors) == 0:
                print('perfect coloring fail')
                color = min(color_counts.keys(), key=lambda color: color_counts[color])
            else:
                color = min(available_colors, key=lambda color: color_counts[color])
            coloring[cell] = color
            color_counts[color] += 1
        
        assert all(
            color_i is not None
            for color_i in coloring.values()
        )
        return Board(
            plane_partition,
            coloring, # type: ignore
            colors,
        )
    
    def draw(
        self,
        screen: pygame.surface.Surface,
        mouse: int2,
    ) -> None:
        screen_size: Final = screen.get_width()
        screen_ratio: Final = screen_size/self.width
        for cell_i, cell in self.plane_partition.cells.items():
            for pixel in cell:
                pixel.draw_square(
                    screen,
                    self.colors[self.coloring[cell_i]],
                    screen_ratio,
                )
        
        mouse_i: Final = self.plane_partition.pixels[mouse.y][mouse.x]
        
        # neighbor coloring the same/lerp between as the mouse's cell
        # for neighbor_i in self.plane_partition.neighborhood[mouse_i]:
        #     color = color_lerp(
        #         self.colors[self.coloring[mouse_i]],
        #         self.colors[self.coloring[neighbor_i]],
        #         .2
        #     )
        #     for pixel in self.plane_partition.cells[neighbor_i]:
        #         pixel.draw_square(
        #             screen,
        #             color,
        #             screen_ratio,
        #         )
        
        mouse_centroid: Final = self.plane_partition.centroids[mouse_i]
        for neighbor_i in self.plane_partition.neighborhood[mouse_i]:
            neighbor_centroid = self.plane_partition.centroids[neighbor_i]
            pygame.draw.line(
                screen,
                pygame.Color(0, 0, 0),
                mouse_centroid.to_screen(screen_ratio),
                neighbor_centroid.to_screen(screen_ratio),
            )
        
        for pixel in self.plane_partition.border:
            pixel.draw_square(
                screen,
                pygame.Color(0, 0, 0),
                screen_ratio,
            )