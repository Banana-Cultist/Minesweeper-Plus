using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Timeline;
using Unity.VisualScripting;
using static UnityEngine.EventSystems.EventTrigger;

public static class VoronoiBoard
{
    public static List<TileController> initialize(BoardTypeDelegate board, int cells, RectTransform bounds, int relaxation)
	{
        // Initialize Sites
        float borderMargin = 0.025f;
        int seed = UnityEngine.Random.Range(0, 1000000);
        //int seed = 305357;
        UnityEngine.Random.InitState(seed);
        Debug.Log(seed);

        PriorityQueue<VoronoiEvent> events = new();
        Hashtable sites = new();

        // Sweeps board from bottom to top
        for (int i = 0; i < cells; i++)
		{
            decimal2 site = new decimal2(
                (decimal) UnityEngine.Random.Range(
                    bounds.rect.xMin + bounds.rect.width * borderMargin,
                    bounds.rect.xMax - bounds.rect.width * borderMargin),
                (decimal) UnityEngine.Random.Range(
                    bounds.rect.yMin + bounds.rect.height * borderMargin,
                    bounds.rect.yMax - bounds.rect.height * borderMargin)
            );

            sites.Add(site, new List<VoronoiEdge>());
			events.Insert(new VoronoiEvent()
			{
				point = site,
				isSiteEvent = true
			});

            //TileController sitePoint = board.createTile();
            //float width = 15;
            //sitePoint.points = new Vector2[]
            //{
            //    site.toVector2() + new Vector2(-width, -width),
            //    site.toVector2() + new Vector2(width, -width),
            //    site.toVector2() + new Vector2(width, width),
            //    site.toVector2() + new Vector2(-width, width)
            //};
            //sitePoint.borderColor = Color.blue;
            //sitePoint.borderController.sortingOrder = 10;
            //sitePoint.fillColor = sitePoint.borderColor;
            //sitePoint.shapeRenderer.sortingOrder = 10;
            //sitePoint.UpdateShape();
            //sitePoint.UpdateLabel();
        }

        List<List<decimal2>> diagram = new();
        for (int i = 0; i <= relaxation; i++)
        {
            List<VoronoiEdge> edges = new();
            decimal sweepLine = 0;
            VoronoiArc beachRoot = null;
            while (events.Length() > 0)
            {
                VoronoiEvent e = events.Pull();
                if (!e.valid) continue;
                sweepLine = (decimal) e.point.y;
                if (e.isSiteEvent)
                {
                    handleSite(e.point, ref beachRoot, sweepLine, events, sites);
                }
                else
                {
                    handleCircle(e, events, sweepLine, edges, beachRoot, sites);
                }
            }
            finishEdges(beachRoot, bounds.rect, edges);
            foreach (VoronoiEdge edge in edges)
            {
                if (edge.twin != null)
                {
                    edge.start = edge.twin.end;
                    edge.twin = null;
                }

                //TileController edgeTile = board.createTile();
                //edgeTile.points = new Vector2[]
                //{
                //    edge.start.toVector2(),
                //    edge.end.toVector2()
                //};
                //edgeTile.borderColor = Color.black;
                //tiles.Add(edgeTile);
            }

            diagram = processCells(sites, bounds.rect);
            sites.Clear();
            events.clear();
            foreach (List<decimal2> cell in diagram)
            {
                decimal2 centroid = decimal2.zero;
                foreach (decimal2 vertex in cell)
                {
                    centroid += vertex;
                }
                centroid /= cell.Count;

                sites.Add(centroid, new List<VoronoiEdge>());
                events.Insert(new VoronoiEvent()
                {
                    point = centroid,
                    isSiteEvent = true
                });
            }
        }

        List<TileController> tiles = new();
        Hashtable tilePoints = new();
        foreach (List<decimal2> cell in diagram)
        {
            TileController tile = board.createTile();

            tile.points = new Vector2[cell.Count];
            for (int i = 0; i < cell.Count; i++)
            {
                tile.points[i] = cell[i].toVector2();
            }

            tile.fillColor = Color.HSVToRGB(0, 0, 0.25f);
            //Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1, 1);
            tile.borderColor = Color.black;
            tiles.Add(tile);
            tile.UpdateShape();

            foreach (Vector2 point in tile.points)
            {
                if (tilePoints.Contains(point))
                {
                    (tilePoints[point] as List<TileController>).Add(tile);
                }
                else
                {
                    tilePoints.Add(point, new List<TileController> { tile });
                }
            }
        }

        foreach (TileController tile in tiles)
        {
            foreach (Vector2 point in tile.points)
            {
                foreach (TileController neighbor in (tilePoints[point] as List<TileController>))
                {
                    if (!neighbor.Equals(tile))
                    {
                        tile.adjacents.Add(neighbor);
                    }
                }
            }
        }

        return tiles;
	}

    private static List<List<decimal2>> processCells(Hashtable sites, Rect bounds)
    {
        List<List<decimal2>> cells = new();
        foreach (List<VoronoiEdge> entry in sites.Values)
        {
            HashSet<decimal2> points = new();
            foreach (VoronoiEdge edge in entry)
            {
                points.Add(edge.start);
                points.Add(edge.end);
            }

            List<decimal2> tile = new(points);
            cwSort(ref tile);
            clipTile(ref tile, bounds);
            cells.Add(tile);
        }

        return cells;
    }

    private static void finishEdges(VoronoiArc arc, Rect bounds, List<VoronoiEdge> edges)
    {
        if (!arc.isVertex)
        {
            return;
        }
        
        if ((arc.edge.start.x >= (decimal) bounds.xMax && arc.edge.direction.x < 0) ||
            (arc.edge.start.x <= (decimal) bounds.xMin && arc.edge.direction.x > 0) ||
            (arc.edge.start.y >= (decimal) bounds.yMax && arc.edge.direction.y < 0) ||
            (arc.edge.start.y <= (decimal) bounds.yMin && arc.edge.direction.y > 0))
        {
            arc.edge.end = arc.edge.start;
        }
        else
        {
            decimal x = (decimal) (arc.edge.direction.x < 0 ? bounds.xMax + bounds.width : bounds.xMin - bounds.width);
            decimal y = arc.edge.slope * x + arc.edge.yPos;
            if (y > (decimal) (bounds.yMax + bounds.height) || y < (decimal) (bounds.yMin - bounds.height))
            {
                if (y > (decimal) (bounds.yMax + bounds.height))
                {
                    y = (decimal) (bounds.yMax + bounds.height);
                }
                else if (y < (decimal) (bounds.yMin - bounds.height))
                {
                    y = (decimal) (bounds.yMin - bounds.height);
                }
                x = (y - arc.edge.yPos) / arc.edge.slope;
            }
            arc.edge.end = new decimal2(x, y);

            edges.Add(arc.edge);
        }

        finishEdges(arc.leftChild, bounds, edges);
        finishEdges(arc.rightChild, bounds, edges);
    }

    private static void cwSort(ref List<decimal2> vertices)
    {
        decimal2 center = decimal2.zero;
        foreach (decimal2 vertice in vertices)
        {
            center += vertice;
        }
        center /= vertices.Count;

        vertices.Sort((decimal2 a, decimal2 b) =>
        {
            if (a.x - center.x >= 0 && b.x - center.x < 0)
                return 1;
            if (a.x - center.x < 0 && b.x - center.x >= 0)
                return -1;
            if (a.x - center.x == 0 && b.x - center.x == 0)
            {
                if (a.y - center.y >= 0 || b.y - center.y >= 0)
                    return a.y > b.y ? 1 : -1;
                return b.y > a.y ? 1 : -1;
            }

            // compute the cross product of vectors (center -> a) x (center -> b)
            decimal det = (a.x - center.x) * (b.y - center.y) - (b.x - center.x) * (a.y - center.y);
            if (det < 0)
                return 1;
            if (det > 0)
                return -1;

            // points a and b are on the same line from the center
            // check which point is closer to the center
            decimal d1 = (a.x - center.x) * (a.x - center.x) + (a.y - center.y) * (a.y - center.y);
            decimal d2 = (b.x - center.x) * (b.x - center.x) + (b.y - center.y) * (b.y - center.y);
            return d1 > d2 ? 1 : -1;
        });
    }

    private static void clipTile(ref List<decimal2> vertices, Rect bounds)
    {
        List<decimal2> clipper = new() {
            new decimal2(bounds.xMax, bounds.yMax),
            new decimal2(bounds.xMax, bounds.yMin),
            new decimal2(bounds.xMin, bounds.yMin),
            new decimal2(bounds.xMin, bounds.yMax)
        };
        for (int i = 0; i < clipper.Count; i++)
        {
            decimal2 clipperStart = clipper[i];
            decimal2 clipperEnd = clipper[(i + 1) % clipper.Count];
            List<decimal2> clippedVertices = new();
            for(int j = 0; j < vertices.Count; j++)
            {
                decimal2 polyStart = vertices[j];
                decimal2 polyEnd = vertices[(j + 1) % vertices.Count];
                decimal startPos = (clipperEnd.x - clipperStart.x) * (polyStart.y - clipperStart.y) -
                    (clipperEnd.y - clipperStart.y) * (polyStart.x - clipperStart.x);
                decimal endPos = (clipperEnd.x - clipperStart.x) * (polyEnd.y - clipperStart.y) -
                    (clipperEnd.y - clipperStart.y) * (polyEnd.x - clipperStart.x);
                // both points inside clipping line
                if (startPos < 0 && endPos < 0)
                {
                    clippedVertices.Add(polyEnd);
                    continue;
                }
                // both points outside of clipping line
                else if (startPos >= 0 && endPos >= 0)
                {
                    continue;
                }
                decimal numX = (clipperStart.x * clipperEnd.y - clipperStart.y * clipperEnd.x) *
                    (polyStart.x - polyEnd.x) - (clipperStart.x - clipperEnd.x) *
                    (polyStart.x * polyEnd.y - polyStart.y * polyEnd.x);
                decimal numY = (clipperStart.x * clipperEnd.y - clipperStart.y * clipperEnd.x) *
                    (polyStart.y - polyEnd.y) - (clipperStart.y - clipperEnd.y) *
                    (polyStart.x * polyEnd.y - polyStart.y * polyEnd.x);
                decimal den = (clipperStart.x - clipperEnd.x) * (polyStart.y - polyEnd.y) -
                    (clipperStart.y - clipperEnd.y) * (polyStart.x - polyEnd.x);
                decimal2 intersection = new decimal2(numX / den, numY / den);
                clippedVertices.Add(intersection);
                if (startPos >= 0 && endPos < 0) clippedVertices.Add(polyEnd);
            }
            vertices = clippedVertices;
        }
    }

    // processes site event
    private static void handleSite(decimal2 p, ref VoronoiArc beachRoot, decimal sweepLine, PriorityQueue<VoronoiEvent> events, Hashtable sites)
    {
        // base case
        if (beachRoot == null)
        {
            beachRoot = new()
            {
                point = p,
                isVertex = false
            };
            return;
        }

        // find parabola on beach line right above p
        VoronoiArc par = getParabolaByX(p.x, sweepLine, beachRoot);
        if (par.circleEvent != null) par.circleEvent.valid = false;


        // create new dangling edge; bisects parabola focus and p
        decimal2 start = new decimal2(p.x, getY(par.point, p.x, sweepLine));
        VoronoiEdge el = new VoronoiEdge(start, par.point, p);
        VoronoiEdge er = new VoronoiEdge(start, p, par.point);
        el.twin = er;
		er.twin = el;
		par.edge = el;
        par.isVertex = true;

        ((List<VoronoiEdge>)sites[p]).Add(el);
        ((List<VoronoiEdge>)sites[par.point]).Add(er);

        // replace original parabola par with p0, p1, p2
        VoronoiArc p0 = new()
        {
            point = par.point,
            isVertex = false
        };
        VoronoiArc p1 = new()
        {
            point = p,
            isVertex = false
        };
        VoronoiArc p2 = new()
        {
            point = par.point,
            isVertex = false
        };

        par.setLeftChild(p0);
		par.setRightChild(new() {
            isVertex = true
        });
		par.rightChild.edge = er;
        par.rightChild.setLeftChild(p1);
		par.rightChild.setRightChild(p2);

		checkCircleEvent(p0, sweepLine, events);
        checkCircleEvent(p2, sweepLine, events);
    }

    // process circle event
    private static void handleCircle(VoronoiEvent e, PriorityQueue<VoronoiEvent> events, decimal sweepLine, List<VoronoiEdge> edges, VoronoiArc beachRoot, Hashtable sites)
    {
        if (!e.valid) return;

        // find p0, p1, p2 that generate this event from left to right
        VoronoiArc p1 = e.arc;
        VoronoiArc xl = VoronoiArc.getLeftParent(p1);
        VoronoiArc xr = VoronoiArc.getRightParent(p1);
        VoronoiArc p0 = VoronoiArc.getLeftChild(xl);
        VoronoiArc p2 = VoronoiArc.getRightChild(xr);

        // remove associated events since the points will be altered
        if (p0.circleEvent != null)
        {
            p0.circleEvent.valid = false;
        }
        if (p2.circleEvent != null)
        {
            p2.circleEvent.valid = false;
        }

        decimal2 p = new(e.point.x, getY(p1.point, e.point.x, sweepLine)); // new vertex

        // end edges!
        xl.edge.end = p;
		xr.edge.end = p;
		edges.Add(xl.edge);
		edges.Add(xr.edge);

        // start new bisector (edge) from this vertex on which ever original edge is higher in tree
        VoronoiArc higher = new() {
            isVertex = true
        };
        VoronoiArc par = p1;
 		while (par != beachRoot)
        {
			par = par.parent;
			if (par == xl) higher = xl;
			if (par == xr) higher = xr;
		}
        higher.edge = new VoronoiEdge(p, p0.point, p2.point);
        ((List<VoronoiEdge>)sites[p0.point]).Add(higher.edge);
        ((List<VoronoiEdge>)sites[p2.point]).Add(higher.edge);

        // delete p1 and parent (boundary edge) from beach line
        VoronoiArc gparent = p1.parent.parent;
        if (p1.parent.leftChild == p1)
        {
            if (gparent.leftChild == p1.parent) gparent.setLeftChild(p1.parent.rightChild);
            if (gparent.rightChild == p1.parent) gparent.setRightChild(p1.parent.rightChild);
        }
        else
        {
            if (gparent.leftChild == p1.parent) gparent.setLeftChild(p1.parent.leftChild);
            if (gparent.rightChild == p1.parent) gparent.setRightChild(p1.parent.leftChild);
        }

        e.arc.parent = null;
        e.arc = null;

        checkCircleEvent(p0, sweepLine, events);
        checkCircleEvent(p2, sweepLine, events);
	}

    // adds circle event if foci a, b, c lie on the same circle
    private static void checkCircleEvent(VoronoiArc b, decimal sweepLine, PriorityQueue<VoronoiEvent> events)
    {
        VoronoiArc lp = VoronoiArc.getLeftParent(b);
        VoronoiArc rp = VoronoiArc.getRightParent(b);

        if (lp == null || rp == null) return;

        VoronoiArc a = VoronoiArc.getLeftChild(lp);
        VoronoiArc c = VoronoiArc.getRightChild(rp);

        if (a == null || c == null || a.point == c.point) return;

        if (ccw(a.point, b.point, c.point) != 1) return;

        // edges will intersect to form a vertex for a circle event
        decimal2 start = (decimal2) getEdgeIntersection(lp.edge, rp.edge);
        if (start == null) return;

        // compute radius
        decimal dx = b.point.x - start.x;
        decimal dy = b.point.y - start.y;
        decimal d = Sqrt((dx * dx) + (dy * dy));
        if (start.y + d < sweepLine) return; // must be after sweep line


        decimal2 ep = new(start.x, start.y + d);
        // add circle event
        VoronoiEvent e = new()
        {
            point = ep,
            isSiteEvent = false,
            arc = b
        };
        b.circleEvent = e;
        events.Insert(e);
	}

    // returns intersection of the lines of with vectors a and b
    private static decimal2? getEdgeIntersection(VoronoiEdge a, VoronoiEdge b)
    {

        if (b.slope == a.slope && b.yPos != a.yPos)
        {
            return null;
        }

        decimal x = (b.yPos - a.yPos) / (a.slope - b.slope);
        decimal y = (a.slope * x) + a.yPos;

        return new decimal2(x, y);
    }

    private static int ccw(decimal2 a, decimal2 b, decimal2 c)
    {
        decimal area2 = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
        if (area2 < 0) return -1;
        else if (area2 > 0) return 1;
        else return 0;
    }

    // returns current x-coordinate of an unfinished edge
    private static decimal getXofEdge(VoronoiArc par, decimal sweepLine)
    {
        //find intersection of two parabolas

        VoronoiArc left = VoronoiArc.getLeftChild(par);
        VoronoiArc right = VoronoiArc.getRightChild(par);

        decimal2 p = left.point;
        decimal2 r = right.point;

        decimal dp = 2 * (p.y - sweepLine);
        decimal a1 = 1 / dp;
        decimal b1 = -2 * p.x / dp;
        decimal c1 = ((p.x * p.x) + (p.y * p.y) - (sweepLine * sweepLine)) / dp;

        decimal dp2 = 2 * (r.y - sweepLine);
        decimal a2 = 1 / dp2;
        decimal b2 = -2 * r.x / dp2;
        decimal c2 = ((r.x * r.x) + (r.y * r.y) - (sweepLine * sweepLine)) / dp2;

        decimal a = a1 - a2;
        decimal b = b1 - b2;
        decimal c = c1 - c2;

        decimal disc = (b * b) - (4 * a * c);
        decimal x1 = (-b + Sqrt(disc)) / (2 * a);
        decimal x2 = (-b - Sqrt(disc)) / (2 * a);

        decimal ry;
        if (p.y > r.y) ry = Math.Max(x1, x2);
        else ry = Math.Min(x1, x2);

        return ry;
    }

    // returns parabola above this x coordinate in the beach line
    private static VoronoiArc getParabolaByX(decimal xx, decimal sweepLine, VoronoiArc beachLineRoot)
    {
        VoronoiArc par = beachLineRoot;
        decimal x;
        while (par.isVertex)
        {
            x = getXofEdge(par, sweepLine);
            if (x > xx) par = par.leftChild;
            else par = par.rightChild;
        }
        return par;
    }

    // find corresponding y-coordinate to x on parabola with focus p
    private static decimal getY(decimal2 p, decimal x, decimal sweep)
    {
        // determine equation for parabola around focus p
        decimal dp = 2 * (p.y - sweep);
        decimal a1 = 1 / dp;
        decimal b1 = -2 * p.x / dp;
        decimal c1 = ((p.x * p.x) + (p.y * p.y) - (sweep * sweep)) / dp;
        return (a1 * x * x) + (b1 * x) + c1;
    }
    
    public class VoronoiEvent : IComparable
    {
		public bool isSiteEvent;
        public decimal2 point;
		public VoronoiArc arc;
		public bool valid = true;

        public int CompareTo(object obj)
        {
			VoronoiEvent other = (VoronoiEvent) obj;
			if (point.y > other.point.y)
			{
				return 1;
			}
			else if (point.y < other.point.y)
			{
				return -1;
			}
			else
			{
                if (point.x > other.point.x)
                {
                    return 1;
                }
                else if (point.x < other.point.x)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        }
    }

	public class VoronoiEdge
	{
		public decimal2 start, leftSite, rightSite, direction;
		public decimal2? end;

		public VoronoiEdge twin;

		public decimal slope, yPos;

        public VoronoiEdge(decimal2 first, decimal2 left, decimal2 right)
        {
            start = first;
            leftSite = left;
            rightSite = right;
            direction = new decimal2(right.y - left.y, -(right.x - left.x));
            end = null;
            slope = (right.x - left.x) / (left.y - right.y);
            decimal2 mid = new decimal2((right.x + left.x) / 2, (left.y + right.y) / 2);
            yPos = mid.y - slope * mid.x;
        }
    }

	public class VoronoiArc
	{
		public decimal2 point;
		public VoronoiEdge edge;
		public bool isVertex;
		public VoronoiEvent circleEvent;

		public VoronoiArc parent, leftChild, rightChild;

		public void setLeftChild(VoronoiArc p)
		{
			leftChild = p;
			p.parent = this;
		}

        public void setRightChild(VoronoiArc p)
        {
            rightChild = p;
            p.parent = this;
        }

		public static VoronoiArc getLeftParent(VoronoiArc p)
		{
			VoronoiArc parent = p.parent;
			if (parent == null) return null;
			// retrieves highest order left parent
			VoronoiArc last = p;
			while (parent.leftChild == last)
			{
				if (parent.parent == null) return null;
				last = parent;
				parent = parent.parent;
			}
			return parent;
		}

        public static VoronoiArc getRightParent(VoronoiArc p)
        {
            VoronoiArc parent = p.parent;
            if (parent == null) return null;
            VoronoiArc last = p;
            // retrieves highest order right parent
            while (parent.rightChild == last)
            {
                if (parent.parent == null) return null;
                last = parent;
                parent = parent.parent;
            }
            return parent;
        }

		public static VoronoiArc getLeftChild(VoronoiArc p)
		{
			if (p == null) return null;
			VoronoiArc child = p.leftChild;
			while(child.isVertex)
			{
				child = child.rightChild;
			}
			return child;
		}

        public static VoronoiArc getRightChild(VoronoiArc p)
        {
            if (p == null) return null;
            VoronoiArc child = p.rightChild;
            while (child.isVertex)
            {
                child = child.leftChild;
            }
            return child;
        }

		public static VoronoiArc getLeft(VoronoiArc p)
		{
			return getLeftChild(getLeftParent(p));
		}

        public static VoronoiArc getRight(VoronoiArc p)
        {
            return getRightChild(getRightParent(p));
        }
    }

    public static decimal Sqrt(decimal x, decimal? guess = null)
    {
        var ourGuess = guess.GetValueOrDefault(x / 2m);
        var result = x / ourGuess;
        var average = (ourGuess + result) / 2m;

        if (average == ourGuess) // This checks for the maximum precision possible with a decimal.
            return average;
        else
            return Sqrt(x, average);
    }

}

public class decimal2
{
    public decimal x, y;
    public static decimal2 zero = new decimal2(0m, 0m);

    public decimal2(decimal x, decimal y)
    {
        this.x = x;
        this.y = y;
    }
    public decimal2(float x, float y)
    {
        this.x = (decimal) x;
        this.y = (decimal) y;
    }

    public decimal2(Vector2 v)
    {
        this.x = (decimal) v.x;
        this.y = (decimal) v.y;
    }

    public Vector2 toVector2()
    {
        return new Vector2((float) x, (float) y);
    }

    public static decimal2 operator +(decimal2 a, decimal2 b)
    {
        return new decimal2(a.x + b.x, a.y + b.y);
    }

    public static decimal2 operator /(decimal2 a, decimal b)
    {
        return new decimal2(a.x / b, a.y / b);
    }
}