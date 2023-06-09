"""
This type stub file was generated by pyright.
"""

from shapely.geometry.base import BaseMultipartGeometry

"""Multi-part collections of geometries
"""
class GeometryCollection(BaseMultipartGeometry):
    """
    A collection of one or more geometries that may contain more than one type
    of geometry.

    Parameters
    ----------
    geoms : list
        A list of shapely geometry instances, which may be of varying
        geometry types.

    Attributes
    ----------
    geoms : sequence
        A sequence of Shapely geometry instances

    Examples
    --------
    Create a GeometryCollection with a Point and a LineString

    >>> from shapely import LineString, Point
    >>> p = Point(51, -1)
    >>> l = LineString([(52, -1), (49, 2)])
    >>> gc = GeometryCollection([p, l])
    """
    __slots__ = ...
    def __new__(self, geoms=...):
        ...
    
    @property
    def __geo_interface__(self): # -> dict[str, str | list[Unknown]]:
        ...
    


