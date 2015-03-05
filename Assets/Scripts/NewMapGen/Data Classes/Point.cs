using UnityEngine;
using System.Collections;

public class Point 
{

    //Point is a class, rather than a struct, because I want it to be nullable (without using ?).

    //This class is meant to be similar to a Vector2, but with int values.

    public int x, y;

    //These are the Points +1 in the given directions.
    public Point up
    {
        get { return new Point(x, y + 1); }
    }
    public Point right
    {
        get { return new Point(x + 1, y); }
    }
    public Point left
    {
        get { return new Point(x - 1, y); }
    }
    public Point down
    {
        get { return new Point(x, y - 1); }
    }

    #region Constructors

    public Point()
    {
        x = 0;
        y = 0;
    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    //Constructor for a point, taking in a Vector3, truncating its values to ints.
    public Point(Vector3 vec) : this((int)vec.x, (int)vec.y) { }

    #endregion


    #region Operators

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.x - b.x, a.y - b.y);
    }
    public static Point operator +(Point a, Point b)
    {
        return new Point(a.x + b.x, a.y + b.y);
    }
    public static Point operator *(Point a, int b)
    {
        return new Point(a.x * b, a.y * b);
    }
    public static Point operator *(int a, Point b)
    {
        return new Point(b.x * a, b.y * a);
    }
    public static bool operator ==(Point a, Point b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        if (a.x == b.x && a.y == b.y)
        {
            return true;
        }

        return false;
    }
    public static bool operator !=(Point a, Point b)
    {
        // If both are null, or both are same instance, return false.
        if (System.Object.ReferenceEquals(a, b))
        {
            return false;
        }

        // If one is null, but not both, return true.
        if (((object)a == null) || ((object)b == null))
        {
            return true;
        }

        if (a.x != b.x || a.y != b.y)
        {
            return true;
        }

        return false;
    }

    #endregion


    //Do not use this HashCode function. It's not set up efficiently.
    public override int GetHashCode()
    {
        return 1;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }
        else if (!(obj is Point))
        {
            return false;
        }
        else if ((Point) obj == this)
        {
            return true;
        }

        return false;
    }

    public override string ToString()
    {
        return ("(" + x.ToString() + ", " + y.ToString() + ")");
    }


    #region Public Methods

    //Creates a Vector3 from this point.
    public Vector3 toVector3()
    {
        return new Vector3(this.x, this.y, 0);
    }

    //If the input point is adjacent to this point, returns true.
    public bool isAdjacent(Point other)
    {
        int xdiff = Mathf.Abs(other.x - this.x);
        int ydiff = Mathf.Abs(other.y - this.y);

        if (xdiff > 1 || ydiff > 1 || (xdiff == 1 && ydiff == 1))
        {
            return false;
        }

        return true;
    }

    //Returns an array of the 4 Points (up, right, down, left in that order) adjacent to the point called on.
    //If EightDir is true, will return the 8 neighbors to this point, adjacent and diagonals.
    public Point[] getAdjacent(bool EightDir = false)
    {
        Point[] array;

        if (!EightDir)
        {
            array = new Point[4];

            array[0] = new Point(this.x, this.y + 1); //Up
            array[1] = new Point(this.x + 1, this.y); //Right
            array[2] = new Point(this.x, this.y - 1); //Down
            array[3] = new Point(this.x - 1, this.y); //Left
        }
        else
        {
            array = new Point[8];

            array[0] = new Point(this.x, this.y + 1); //Up
            array[1] = new Point(this.x + 1, this.y); //Right
            array[2] = new Point(this.x, this.y - 1); //Down
            array[3] = new Point(this.x - 1, this.y); //Left

            array[4] = new Point(this.x + 1, this.y + 1); //Upper Right
            array[5] = new Point(this.x + 1, this.y - 1); //Lower Right
            array[6] = new Point(this.x - 1, this.y - 1); //Lower Left
            array[7] = new Point(this.x - 1, this.y + 1); //Upper Left
        }

        return array;
    }

    //Returns the number of tiles between the two points.
    public int tileDiff(Point other)
    {
        Point temp = this - other;
        return (Mathf.Abs(temp.x) + Mathf.Abs(temp.y));
    }

    #endregion

}
