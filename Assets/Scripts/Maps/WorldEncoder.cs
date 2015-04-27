using UnityEngine;

public abstract class WorldEncoder : MapDebuggable {
    #region Static Utility Methods
    public static string DataSizeString(int bytes) {
        var kb = (bytes / 1024f);
        var mb = (kb / 1024f);
        var gb = (mb / 1024f);

        return (gb + "GB/" + mb + "MB/" + kb + "KB/" + bytes + "B");
    }
    #endregion

    public const int BASE_HEADER_SIZE = 32;

    public virtual int VersionCode {
        get { return 0x0; }
    }

    public virtual int CalculateSize(World world) {
        return (BASE_HEADER_SIZE + world.Area);
    }

    public void Encode(World world, DataInterface dataInterface) {
        dataInterface.Write((ushort)world.Width);
        dataInterface.Write((ushort)world.Height);
        dataInterface.Write(world.NavyScale);
        dataInterface.Write(world.StartCoordinate.Latitude);
        dataInterface.Write(world.StartCoordinate.Longitude);
        Debug("Finished encoding world base header.");

        EncodeBody(world, dataInterface);
    }

    public abstract void EncodeBody(World world, DataInterface dataInterface);

    public World Decode(DataInterface dataInterface) {
        var width = dataInterface.ReadUShort();
        var height = dataInterface.ReadUShort();
        var scale = dataInterface.ReadFloat();
        var coord = new Coordinate();
        coord.Latitude = dataInterface.ReadDouble();
        coord.Longitude = dataInterface.ReadDouble();
        Debug("Finished decoding world base header.");

        World world = new World(width, height, coord, scale);

        DecodeBody(world, dataInterface);

        return world;
    }

    public abstract void DecodeBody(World world, DataInterface dataInterface);
}