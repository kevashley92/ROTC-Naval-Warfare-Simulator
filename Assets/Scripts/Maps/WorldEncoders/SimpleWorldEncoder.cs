using System;

public class SimpleWorldEncoder : WorldEncoder {
    public override int VersionCode {
        get { return 0x0; }
    }

    public override int CalculateSize(World world) {
        return base.CalculateSize(world);
    }

    public override void DecodeBody(World world, DataInterface dataInterface) {
        Buffer.BlockCopy(dataInterface.Source, dataInterface.IndexRead, world.Tiles, 0, world.Area);
    }

    public override void EncodeBody(World world, DataInterface dataInterface) {
        Buffer.BlockCopy(world.Tiles, 0, dataInterface.Source, dataInterface.IndexWrite, world.Width * world.Height);
    }
}
