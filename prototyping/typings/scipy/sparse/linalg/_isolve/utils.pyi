"""
This type stub file was generated by pyright.
"""

__docformat__ = ...
__all__ = []
_coerce_rules = ...
def coerce(x, y): # -> str:
    ...

def id(x):
    ...

def make_system(A, M, x0, b): # -> tuple[LinearOperator | MatrixLinearOperator, IdentityOperator | LinearOperator | MatrixLinearOperator, NDArray[Any] | ndarray[Any, Unknown] | ndarray[Any, dtype[Unknown]] | Unbound, ndarray[Any, dtype[Any]], (x: Unknown) -> Unknown]:
    """Make a linear system Ax=b

    Parameters
    ----------
    A : LinearOperator
        sparse or dense matrix (or any valid input to aslinearoperator)
    M : {LinearOperator, Nones}
        preconditioner
        sparse or dense matrix (or any valid input to aslinearoperator)
    x0 : {array_like, str, None}
        initial guess to iterative method.
        ``x0 = 'Mb'`` means using the nonzero initial guess ``M @ b``.
        Default is `None`, which means using the zero initial guess.
    b : array_like
        right hand side

    Returns
    -------
    (A, M, x, b, postprocess)
        A : LinearOperator
            matrix of the linear system
        M : LinearOperator
            preconditioner
        x : rank 1 ndarray
            initial guess
        b : rank 1 ndarray
            right hand side
        postprocess : function
            converts the solution vector to the appropriate
            type and dimensions (e.g. (N,1) matrix)

    """
    ...
