namespace Labyrinth
{
    public enum CellKind : byte
    {
        Wall = (byte)'#',
        Pass = (byte)'.',
        StartPosition = (byte)'T',
        ControlRoom = (byte)'C',
        Unknown = (byte)'?',
    }
}