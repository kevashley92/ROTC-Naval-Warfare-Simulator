/* ****************************************************************
 * 
 * Name: DataInterface
 * 
 * Date Created: 2015-02-11
 * 
 * Original Team: Maps
 * 
 * Description: Used as a utility class to write primitives to a
 *      byte array.
 * 
 * Change Log
 * ================================================================
 * Name             Date        Comment
 * ---------------- ----------- -----------------------------------
 * R. Lodico        2015-02-11  Created, written and tested
 */

using System;

public class DataInterface : MapDebuggable {
    #region Constructor and Fields/Properties
    private byte[] data;
    private int idxRead;
    private int idxWrite;

    public DataInterface(ref byte[] data) {
        this.data = data;
        this.idxRead = 0;
        this.idxWrite = 0;
    }

    public int IndexRead { get { return idxRead; } }
    public int IndexWrite { get { return idxWrite; } }
    public byte[] Source { get { return data; } }

    #endregion

    #region Index Tracking
    public int ShiftRead(int size) {
        idxRead += size;
        return (idxRead - size);
    }

    public int ShiftWrite(int size) {
        idxWrite += size;
        return (idxWrite - size);
    }
    #endregion

    #region Data Reading
    public byte ReadByte() {
        return ReadByte(ShiftRead(sizeof(byte)));
    }

    public byte ReadByte(int position) {
        return data[position];
    }

    public byte[] ReadBytes(int length) {
        return ReadBytes(length, ShiftRead(length * sizeof(byte)));
    }

    public byte[] ReadBytes(int length, int position) {
        var val = new byte[length];

        Array.Copy(data, position, val, 0, length);

        return val;
    }

    public bool ReadBool() {
        return ReadBool(ShiftRead(sizeof(bool)));
    }

    public bool ReadBool(int position) {
        return BitConverter.ToBoolean(data, position);
    }

    public char ReadChar() {
        return ReadChar(ShiftRead(sizeof(char)));
    }

    public char ReadChar(int position) {
        return BitConverter.ToChar(data, position);
    }

    public double ReadDouble() {
        return ReadDouble(ShiftRead(sizeof(double)));
    }

    public double ReadDouble(int position) {
        return BitConverter.ToDouble(data, position);
    }

    public float ReadFloat() {
        return ReadFloat(ShiftRead(sizeof(float)));
    }

    public float ReadFloat(int position) {
        return BitConverter.ToSingle(data, position);
    }

    public int ReadInt() {
        return ReadInt(ShiftRead(sizeof(int)));
    }

    public int ReadInt(int position) {
        return BitConverter.ToInt32(data, position);
    }

    public long ReadLong() {
        return ReadLong(ShiftRead(sizeof(long)));
    }

    public long ReadLong(int position) {
        return BitConverter.ToInt64(data, position);
    }

    public sbyte ReadSByte() {
        return ReadSByte(ShiftRead(sizeof(sbyte)));
    }

    public sbyte ReadSByte(int position) {
        return Convert.ToSByte(data[position]);
    }

    public short ReadShort() {
        return ReadShort(ShiftRead(sizeof(short)));
    }

    public short ReadShort(int position) {
        return BitConverter.ToInt16(data, position);
    }

    public uint ReadUInt() {
        return ReadUInt(ShiftRead(sizeof(uint)));
    }

    public uint ReadUInt(int position) {
        return BitConverter.ToUInt32(data, position);
    }

    public ulong ReadULong() {
        return ReadULong(ShiftRead(sizeof(ulong)));
    }

    public ulong ReadULong(int position) {
        return BitConverter.ToUInt64(data, position);
    }

    public ushort ReadUShort() {
        return ReadUShort(ShiftRead(sizeof(ushort)));
    }

    public ushort ReadUShort(int position) {
        return BitConverter.ToUInt16(data, position);
    }
    #endregion

    #region Data Writing
    public DataInterface Write(byte val) {
        return Write(val, ShiftWrite(1));
    }

    public DataInterface Write(byte val, int position) {
        data[position] = val;

        return this;
    }

    public DataInterface Write(byte[] val) {
        return Write(val, ShiftWrite(val.Length));
    }

    public DataInterface Write(byte[] val, int position) {
        Array.Copy(val, 0, data, position, val.Length);

        return this;
    }

    public DataInterface Write(bool val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(bool val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(char val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(char val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(double val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(double val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(float val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(float val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(int val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(int val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(long val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(long val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(sbyte val) {
        return Write(Convert.ToByte(val));
    }

    public DataInterface Write(sbyte val, int position) {
        return Write(Convert.ToByte(val), position);
    }

    public DataInterface Write(short val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(short val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(uint val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(uint val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(ulong val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(ulong val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }

    public DataInterface Write(ushort val) {
        return Write(BitConverter.GetBytes(val));
    }

    public DataInterface Write(ushort val, int position) {
        return Write(BitConverter.GetBytes(val), position);
    }
    #endregion
}