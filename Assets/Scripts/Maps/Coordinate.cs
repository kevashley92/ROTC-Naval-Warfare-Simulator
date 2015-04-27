using System;

[Serializable]
public class Coordinate : MapDebuggable {
    private double latitude;
    private double longitude;

    public Coordinate()
        : this(0.0, 0.0) {
    }

    public Coordinate(double latitude, double longitude) {
        Latitude = latitude;
        Longitude = longitude;
    }

    public Coordinate(int latitudeDegree, int latitudeMinute, int longitudeDegree, int longitudeMinute)
        : this(latitudeDegree, latitudeMinute, 0.0, longitudeDegree, longitudeMinute, 0.0) {
    }

    public Coordinate(int latitudeDegree, int latitudeMinute, double latitudeSecond, int longitudeDegree, int longitudeMinute, double longitudeSecond) {
        Latitude = latitudeDegree;
        Latitude += (latitudeMinute / 60.0);
        Latitude += (latitudeSecond / 3600.0);

        Longitude = longitudeDegree;
        Longitude += (longitudeMinute / 60.0);
        Longitude += (longitudeSecond / 60.0);
    }

    public double Latitude { get { return latitude; } set { latitude = value; } }
    public int LatitudeDegree { get { return (int)latitude; } }
    public int LatitudeMinute { get { return Math.Abs((int)((latitude % 1.0) * 60.0)); } }

    public double LatitudeSecond {
        get {
            double minute = (latitude % 1.0) * 60.0;
            return Math.Abs((int)((minute % 1.0) * 60.0));
        }
    }

    public double Longitude { get { return longitude; } set { longitude = value; } }
    public int LongitudeDegree { get { return (int)longitude; } }
    public int LongitudeMinute { get { return Math.Abs((int)((longitude % 1.0) * 60.0)); } }

    public double LongitudeSecond {
        get {
            double minute = (longitude % 1.0) * 60.0;
            return Math.Abs((int)((minute % 1.0) * 60.0));
        }
    }

    public static Coordinate operator +(Coordinate a, Coordinate b) {
        return new Coordinate(a.Latitude + b.Latitude, a.Longitude + b.Longitude);
    }

    public static Coordinate operator -(Coordinate a, Coordinate b) {
        return new Coordinate(a.Latitude - b.Latitude, a.Longitude - b.Longitude);
    }

    public static bool operator ==(Coordinate a, Coordinate b) {
        if (a == null || b == null) {
            return (a == null) && (b == null);
        }

        return a.Equals(b);
    }

    public static bool operator !=(Coordinate a, Coordinate b) {
        if (a == null || b == null) {
            return (a == null) ^ (b == null);
        }

        return !a.Equals(b);
    }

    public override bool Equals(object obj) {
        if (obj != null && obj is Coordinate) {
            var c = obj as Coordinate;
            return (c.latitude == latitude) && (c.longitude == longitude);
        }

        return false;
    }

    public override int GetHashCode() {
        return (latitude.GetHashCode() | longitude.GetHashCode());
    }

    public override string ToString() {
        return "[Coordinate { Latitude: " + latitude + ", Longitude: " + longitude + "} ]";
    }
}
