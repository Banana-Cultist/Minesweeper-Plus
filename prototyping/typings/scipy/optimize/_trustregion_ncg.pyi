"""
This type stub file was generated by pyright.
"""

from ._trustregion import BaseQuadraticSubproblem

"""Newton-CG trust-region optimization."""
__all__ = []
class CGSteihaugSubproblem(BaseQuadraticSubproblem):
    """Quadratic subproblem solved by a conjugate gradient method"""
    def solve(self, trust_radius): # -> tuple[Unknown, Literal[False]] | tuple[Unknown, Literal[True]]:
        """
        Solve the subproblem using a conjugate gradient method.

        Parameters
        ----------
        trust_radius : float
            We are allowed to wander only this far away from the origin.

        Returns
        -------
        p : ndarray
            The proposed step.
        hits_boundary : bool
            True if the proposed step is on the boundary of the trust region.

        Notes
        -----
        This is algorithm (7.2) of Nocedal and Wright 2nd edition.
        Only the function that computes the Hessian-vector product is required.
        The Hessian itself is not required, and the Hessian does
        not need to be positive semidefinite.
        """
        ...
    


