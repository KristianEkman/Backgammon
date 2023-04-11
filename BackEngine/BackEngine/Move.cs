using System;
namespace BackEngine
{
	public struct Move
	{
		public byte From;
		public byte To;
		public sbyte Side;

        public override string ToString()
        {
            if (Side == 0)
                return "empty";
            var side = Side == 1 ? "White" : "Black";
            return $"{From}-{To} {side}";
        }

        public uint GetId()
        {
            uint id = From;
            id <<= 8;
            id |= To;
            id <<= 8;
            return id;
        }
    }

    public struct Undid
    {
        public byte FirstWhite;
        public byte FirstBlack;
        public ushort WhitePip;
        public ushort BlackPip;
        public bool Hit;
    }
}

