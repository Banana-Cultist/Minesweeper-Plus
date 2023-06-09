"""
This type stub file was generated by pyright.
"""

__all__ = ['save_npz', 'load_npz']
PICKLE_KWARGS = ...
def save_npz(file, matrix, compressed=...): # -> None:
    """ Save a sparse matrix to a file using ``.npz`` format.

    Parameters
    ----------
    file : str or file-like object
        Either the file name (string) or an open file (file-like object)
        where the data will be saved. If file is a string, the ``.npz``
        extension will be appended to the file name if it is not already
        there.
    matrix: spmatrix (format: ``csc``, ``csr``, ``bsr``, ``dia`` or coo``)
        The sparse matrix to save.
    compressed : bool, optional
        Allow compressing the file. Default: True

    See Also
    --------
    scipy.sparse.load_npz: Load a sparse matrix from a file using ``.npz`` format.
    numpy.savez: Save several arrays into a ``.npz`` archive.
    numpy.savez_compressed : Save several arrays into a compressed ``.npz`` archive.

    Examples
    --------
    Store sparse matrix to disk, and load it again:

    >>> import numpy as np
    >>> import scipy.sparse
    >>> sparse_matrix = scipy.sparse.csc_matrix(np.array([[0, 0, 3], [4, 0, 0]]))
    >>> sparse_matrix
    <2x3 sparse matrix of type '<class 'numpy.int64'>'
       with 2 stored elements in Compressed Sparse Column format>
    >>> sparse_matrix.toarray()
    array([[0, 0, 3],
           [4, 0, 0]], dtype=int64)

    >>> scipy.sparse.save_npz('/tmp/sparse_matrix.npz', sparse_matrix)
    >>> sparse_matrix = scipy.sparse.load_npz('/tmp/sparse_matrix.npz')

    >>> sparse_matrix
    <2x3 sparse matrix of type '<class 'numpy.int64'>'
       with 2 stored elements in Compressed Sparse Column format>
    >>> sparse_matrix.toarray()
    array([[0, 0, 3],
           [4, 0, 0]], dtype=int64)
    """
    ...

def load_npz(file): # -> Any:
    """ Load a sparse matrix from a file using ``.npz`` format.

    Parameters
    ----------
    file : str or file-like object
        Either the file name (string) or an open file (file-like object)
        where the data will be loaded.

    Returns
    -------
    result : csc_matrix, csr_matrix, bsr_matrix, dia_matrix or coo_matrix
        A sparse matrix containing the loaded data.

    Raises
    ------
    OSError
        If the input file does not exist or cannot be read.

    See Also
    --------
    scipy.sparse.save_npz: Save a sparse matrix to a file using ``.npz`` format.
    numpy.load: Load several arrays from a ``.npz`` archive.

    Examples
    --------
    Store sparse matrix to disk, and load it again:

    >>> import numpy as np
    >>> import scipy.sparse
    >>> sparse_matrix = scipy.sparse.csc_matrix(np.array([[0, 0, 3], [4, 0, 0]]))
    >>> sparse_matrix
    <2x3 sparse matrix of type '<class 'numpy.int64'>'
       with 2 stored elements in Compressed Sparse Column format>
    >>> sparse_matrix.toarray()
    array([[0, 0, 3],
           [4, 0, 0]], dtype=int64)

    >>> scipy.sparse.save_npz('/tmp/sparse_matrix.npz', sparse_matrix)
    >>> sparse_matrix = scipy.sparse.load_npz('/tmp/sparse_matrix.npz')

    >>> sparse_matrix
    <2x3 sparse matrix of type '<class 'numpy.int64'>'
        with 2 stored elements in Compressed Sparse Column format>
    >>> sparse_matrix.toarray()
    array([[0, 0, 3],
           [4, 0, 0]], dtype=int64)
    """
    ...

