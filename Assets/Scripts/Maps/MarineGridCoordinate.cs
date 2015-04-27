public class MarineGridCoordinate : MapDebuggable {
    private ushort x;
    private ushort y;

    public ushort X { get { return x; } set { x = value; } }
    public int XQ1 { get { return x / 1000; } }
    public int XQ2 { get { return (x % 1000) / 100; } }
    public int XQ3 { get { return (x % 100) / 10; } }
    public int XQ4 { get { return (x % 10); } }

    public ushort Y { get { return y; } set { y = value; } }
    public int YQ1 { get { return y / 1000; } }
    public int YQ2 { get { return (y % 1000) / 100; } }
    public int YQ3 { get { return (y % 100) / 10; } }
    public int YQ4 { get { return (y % 10); } }

    public MarineGridCoordinate(ushort x, ushort y) {
        X = x;
        Y = y;
    }

    public override string ToString() {
        return "[MarineGridCoordinate { " + x + ", " + y + " } ]";
    }

    public override bool Equals(object obj) {
        if (obj != null && obj is MarineGridCoordinate) {
            var mgc = obj as MarineGridCoordinate;
            return (mgc.x == x) && (mgc.y == y);
        }

        return false;
    }

    public override int GetHashCode() {
        return ((x << 16) | y);
    }

    public static bool operator ==(MarineGridCoordinate a, MarineGridCoordinate b) {
        if (a == null || b == null) {
            return (a == null) && (b == null);
        }

        return a.Equals(b);
    }

    public static bool operator !=(MarineGridCoordinate a, MarineGridCoordinate b) {
        if (a == null || b == null) {
            return (a == null) ^ (b == null);
        }

        return !a.Equals(b);
    }
}
