class BitCoordinates {
    public static byte ToBits(int x, int y) {
        // x and y cannot be greater than 15,
        // but we don't check it for the sake of performance
        byte ret = (byte)x;
        ret <<= 4;
        byte yy = (byte)((byte)y & (byte)0b00001111);
        return (byte)(ret | yy);
    }

    public static (int, int)FromBits(byte b) {
        int x = b & (byte)0b11110000;
        int y = b & (byte)0b00001111;
        return (x >> 4, y);
    }
};