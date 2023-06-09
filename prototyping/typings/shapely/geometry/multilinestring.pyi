"""
This type stub file was generated by pyright.
"""

from shapely.geometry.base import BaseMultipartGeometry

"""Collections of linestrings and related utilities
"""
__all__ = ["MultiLineString"]
class MultiLineString(BaseMultipartGeometry):
    """
    A collection of one or more LineStrings.

    A MultiLineString has non-zero length and zero area.

    Parameters
    ----------
    lines : sequence
        A sequence LineStrings, or a sequence of line-like coordinate
        sequences or array-likes (see accepted input for LineString).

    Attributes
    ----------
    geoms : sequence
        A sequence of LineStrings

    Examples
    --------
    Construct a MultiLineString containing two LineStrings.

    >>> lines = MultiLineString([[[0, 0], [1, 2]], [[4, 4], [5, 6]]])
    """
    __slots__ = ...
    def __new__(self, lines=...): # -> MultiLineString:
        ...
    
    @property
    def __geo_interface__(self): # -> dict[str, str | tuple[tuple[Unknown, ...], ...]]:
        ...
    
    def svg(self, scale_factor=..., stroke_color=..., opacity=...): # -> LiteralString | Literal['<g />']:
        """Returns a group of SVG polyline elements for the LineString geometry.

        Parameters
        ==========
        scale_factor : float
            Multiplication factor for the SVG stroke-width.  Default is 1.
        stroke_color : str, optional
            Hex string for stroke color. Default is to use "#66cc99" if
            geometry is valid, and "#ff3333" if invalid.
        opacity : float
            Float number between 0 and 1 for color opacity. Default value is 0.8
        """
        ...
    


