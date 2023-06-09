"""
This type stub file was generated by pyright.
"""

def next_fast_len(target, real=...): # -> None:
    """Find the next fast size of input data to ``fft``, for zero-padding, etc.

    SciPy's FFT algorithms gain their speed by a recursive divide and conquer
    strategy. This relies on efficient functions for small prime factors of the
    input length. Thus, the transforms are fastest when using composites of the
    prime factors handled by the fft implementation. If there are efficient
    functions for all radices <= `n`, then the result will be a number `x`
    >= ``target`` with only prime factors < `n`. (Also known as `n`-smooth
    numbers)

    Parameters
    ----------
    target : int
        Length to start searching from. Must be a positive integer.
    real : bool, optional
        True if the FFT involves real input or output (e.g., `rfft` or `hfft`
        but not `fft`). Defaults to False.

    Returns
    -------
    out : int
        The smallest fast length greater than or equal to ``target``.

    Notes
    -----
    The result of this function may change in future as performance
    considerations change, for example, if new prime factors are added.

    Calling `fft` or `ifft` with real input data performs an ``'R2C'``
    transform internally.

    Examples
    --------
    On a particular machine, an FFT of prime length takes 11.4 ms:

    >>> from scipy import fft
    >>> import numpy as np
    >>> rng = np.random.default_rng()
    >>> min_len = 93059  # prime length is worst case for speed
    >>> a = rng.standard_normal(min_len)
    >>> b = fft.fft(a)

    Zero-padding to the next regular length reduces computation time to
    1.6 ms, a speedup of 7.3 times:

    >>> fft.next_fast_len(min_len, real=True)
    93312
    >>> b = fft.fft(a, 93312)

    Rounding up to the next power of 2 is not optimal, taking 3.0 ms to
    compute; 1.9 times longer than the size given by ``next_fast_len``:

    >>> b = fft.fft(a, 131072)

    """
    ...

next_fast_len = ...
