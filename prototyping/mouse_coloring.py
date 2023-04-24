from __future__ import annotations
import timeit
from typing import *
# from termcolor import colored
import pygame
from my_lib_rastor import *

rainbow: Final = [
    (228, 3, 3),
    (255, 140, 0),
    (255, 237, 0),
    (0, 128, 38),
    (0, 77, 255),
    (117, 7, 135),
]

def main() -> None:
    pygame.init()
    clock = pygame.time.Clock()
    screen_size = 700
    screen = pygame.display.set_mode((screen_size, screen_size))
    width = 350
    seed_n = 50
    start = timeit.default_timer()
    board = Board.from_colors(
        RasterVoronoi.from_seeds(
            width,
            [int2.random(width) for i in range(seed_n)]
        ),
        dict(enumerate(pygame.Color(c) for c in rainbow))
    )
    end = timeit.default_timer()
    print('time to gen board', end-start)
    while True:
        dt = clock.tick(60)
        # print(dt/1000)
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                return
            # if event.type == pygame.KEYDOWN:
            #     if event.key == pygame.K_UP:
            #         pass
            #     elif event.key == pygame.K_DOWN:
            #         pass

        screen.fill((0, 0, 0))
        
        mouse = (width*int2(*pygame.mouse.get_pos()))//screen_size
        board.draw(screen, mouse)
        pygame.display.update()

def speed_test(test_n: int) -> None:
    width = 350
    seed_n = 7
    print(f'testing with width {width} and seed count {seed_n}')
    start = timeit.default_timer()
    for _ in range(test_n):
        board = Board.from_colors(
            RasterVoronoi.from_seeds(
                width,
                [int2.random(width) for i in range(seed_n)]
            ),
            dict(enumerate(pygame.Color(c) for c in rainbow))
        )
    end = timeit.default_timer()
    print('average time to gen', (end-start)/test_n)

if __name__ == '__main__':
    main()
    # speed_test(10)