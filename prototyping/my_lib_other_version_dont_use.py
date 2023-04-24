# def clip(lo: int, val: int, hi: int) -> int:
#     return max(lo, (min(val, hi)))

# def sorted_edge(p1: float2, p2: float2) -> tuple[float2, float2]:
#     if p1.x == p2.x:
#         return (p1, p2) if p1.y < p2.y else (p2, p1)
#     return (p1, p2) if p1.x < p2.x else (p2, p1)

# class float2:    
#     def __init__(self, x: float, y: float) -> None:
#         self.x: Final = x
#         self.y: Final = y
#         self._hash: Optional[int] = None
    
#     @staticmethod
#     def modulo(theta: float) -> float:
#         """Returns an angle congruent to theta but in the range [-pi, pi]."""
#         return (theta + math.pi) % (math.pi * 2) - math.pi
    
#     def __add__(self, a: float2) -> float2:
#         return float2(self.x+a.x, self.y+a.y)
    
#     def __sub__(self, a: float2) -> float2:
#         return float2(self.x-a.x, self.y-a.y)

#     def __mul__(self, a: float):
#         return float2(self.x*a, self.y*a)
#     __rmul__ = __mul__
    
#     def __truediv__(self, a: float) -> float2:
#         return float2(self.x/a, self.y/a)
    
#     # def from_screen(self, screen_size: int) -> float2:
#     #     return float2(x/screen_size_mul, y/screen_size_mul)
    
#     # def to_screen_int(self) -> tuple[int, int]:
#     #     return int(self.x*screen_size_mul), int(self.y*screen_size_mul)
    
#     # def to_screen_float(self) -> tuple[float, float]:
#     #     return self.x*screen_size_mul, self.y*screen_size_mul
    
#     def length(self) -> float:
#         return (self.x**2 + self.y**2)**.5
    
#     def length2(self) -> float:
#         return (self.x**2 + self.y**2)
    
#     def normalized(self) -> float2:
#         return self/self.length()

#     def __repr__(self) -> str:
#         return f"({self.x:.5f}, {self.y:.5f})"
    
#     def __eq__(self, __value: object) -> bool:
#         if isinstance(__value, float2):
#             return self.x == __value.x and self.y == __value.y
#         return False
    
#     def __hash__(self) -> int:
#         if self._hash is None:
#             self._hash = hash((self.x, self.y))
#         return self._hash

#     @staticmethod
#     def from_polar(r: float, theta: float) -> float2:
#         return r*float2(math.cos(theta), math.sin(theta))
    
#     def theta(self) -> float:
#         if self.x == 0:
#             if self.y < 0:
#                 return -.5 * math.pi
#             elif self.y > 0:
#                 return .5 * math.pi
#             else:
#                 raise ValueError
#         else:
#             return float2.modulo((math.pi * (self.x < 0)) + math.atan(self.y / self.x))
    
#     def rotated(self, theta: float) -> float2:
#         mag = self.length()
#         if mag == 0:
#             return float2.zero()
#         theta += self.theta()
#         return float2.from_polar(mag, theta)
    
#     @staticmethod
#     def proj_scalar(a: float2, b: float2) -> float:
#         return float2.dot(a, b)/b.length()
    
#     @staticmethod
#     def project(a: float2, b: float2) -> float2:
#         return float2.proj_scalar(a, b)*b.normalized()
        
#     @staticmethod
#     def zero() -> float2:
#         return float2(0, 0)
    
#     @staticmethod
#     def one() -> float2:
#         return float2(1, 1)
    
#     @staticmethod
#     def random() -> float2:
#         """inside of the unit square"""
#         return float2(random.random(), random.random())

#     @staticmethod
#     def dot(a: float2, b: float2) -> float:
#         return a.x*b.x + a.y*b.y

#     def draw(self, screen: pygame.surface.Surface, color: pygame.Color) -> None:
#         pygame.draw.circle(screen, color, self.to_screen(screen), 3)
    
#     def to_screen(self, screen: pygame.surface.Surface) -> tuple[float, float]:
#         """mapping from unit square to square screen"""
#         return (screen.get_width()*self).to_tuple()

#     def to_tuple(self) -> tuple[float, float]:
#         return self.x, self.y
    
#     def to_tuple_int(self) -> tuple[int, int]:
#         return int(self.x), int(self.y)
    
#     def to_complex(self) -> complex:
#         return complex(self.x, self.y)
    
#     def __getitem__(self, key: int) -> float:
#         if key == 0:
#             return self.x
#         elif key == 1:
#             return self.y
#         raise ValueError

#     def between(self, lo: float2, hi: float2) -> bool:
#         return (lo.x < self.x < hi.x) and (lo.y < self.y < hi.y)
    
#     def __gt__(self, other: float2) -> bool:
#         if self.x == other.x:
#             return self.y > other.y
#         return self.x > other.x

# class Triangle:
#     def __init__(self, a: float2, b: float2, c: float2) -> None:
#         # assert len(set(verts)) == 3
#         self.verts: Final = {a, b, c}
#         self.verts_tup: Final = tuple(sorted((a, b, c)))
#         self.hash: Final = hash(self.verts_tup)
#         # self.centroid: Final = sum(self.verts, start=float2.zero())/3
#         # self.verts: Final[tuple[float2, float2, float2]] = tuple(sorted(
#         #     verts,
#         #     key=lambda v: (v - self.centroid).theta()
#         # ))

#         D = (a.x - c.x) * (b.y - c.y) - (b.x - c.x) * (a.y - c.y)
#         if D == 0:
#             print(colored('colinear points', 'red'))
#         self.circumcenter: Final = float2(
#             ((((a.x - c.x) * (a.x + c.x) + (a.y - c.y) * (a.y + c.y)) / 2 * (b.y - c.y) -  ((b.x - c.x) * (b.x + c.x) + (b.y - c.y) * (b.y + c.y)) / 2 * (a.y - c.y)) / D),
#             ((((b.x - c.x) * (b.x + c.x) + (b.y - c.y) * (b.y + c.y)) / 2 * (a.x - c.x) -  ((a.x - c.x) * (a.x + c.x) + (a.y - c.y) * (a.y + c.y)) / 2 * (b.x - c.x)) / D)
#         )
#         self.radius2: Final = (a-self.circumcenter).length2()
        
#         # works probably but not much faster and im kinda scared of it
#         # A, B, C = a.to_complex(), b.to_complex(), c.to_complex()
#         # w = C-A
#         # w /= B-A
#         # _circumcenter = (A-B)*(w-abs(w)**2)/2j/w.imag-A
#         # self.radius = abs(_circumcenter+A)
#         # self.radius2: Final = self.radius**2
#         # self.circumcenter: Final = float2(-_circumcenter.real, -_circumcenter.imag)

#         # self.radius: Final = self.radius2**.5
#         # rad_square = float2(self.radius, self.radius)
#         # self.circumcircle_lo: Final = self.circumcenter-rad_square
#         # self.circumcircle_hi: Final = self.circumcenter+rad_square
    
#     # def __iter__(self) -> Iterator[float2]:
#     #     return self.verts.__iter__()

#     # def __getitem__(self, key: int) -> float2:
#     #     return self.verts[key]

#     def circumscribes(self, point: float2) -> bool:
#         assert point not in self.verts
#         # if not point.between(self.circumcircle_lo, self.circumcircle_hi):
#         #     return False
#         return (point-self.circumcenter).length2() < self.radius2

#     def draw(self, screen: pygame.surface.Surface, color: pygame.Color) -> None:
#         pygame.draw.lines(screen, color, True, [p.to_screen(screen) for p in self.verts])
        
#     def __eq__(self, __value: object) -> bool:
#         if isinstance(__value, Triangle):
#             return self.verts_tup == __value.verts_tup
#             # return self.verts == __value.verts
#         return False
    
#     def __reduce__(self) -> str:
#         return self.verts_tup.__repr__()
    
#     def __hash__(self) -> int:
#         return self.hash
#         return hash(self.verts)
    
#     def edges(self) -> set[tuple[float2, float2]]:
#         a, b, c = self.verts
#         return {
#             sorted_edge(a, b),
#             sorted_edge(b, c),
#             sorted_edge(c, a),
#         }