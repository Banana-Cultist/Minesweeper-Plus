"""
This type stub file was generated by pyright.
"""

from enum import IntEnum

class ParamEnum(IntEnum):
    """Wraps IntEnum to provide validation of a requested item.

    Intended for enums used for function parameters.

    Use enum.get_value(item) for this behavior instead of builtin enum[item].
    """
    @classmethod
    def get_value(cls, item): # -> int:
        """Validate incoming item and raise a ValueError with valid options if not present."""
        ...
    


