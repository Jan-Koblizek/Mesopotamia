using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection
{
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions
{

	public static HexDirection Opposite(this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}

	public static HexDirection Before(this HexDirection direction)
	{
		return (HexDirection)(((int)direction - 1 + 6) % 6);
	}

	public static HexDirection After(this HexDirection direction)
	{
		return (HexDirection)(((int)direction + 1) % 6);
	}

	public static Vector2 VectorDirection(this HexDirection direction)
	{
		switch (direction)
		{
			case HexDirection.NE: return new Vector2(0.5f, 0.86f);
			case HexDirection.SE: return new Vector2(0.5f, -0.86f);
			case HexDirection.SW: return new Vector2(-0.5f, -0.86f);
			case HexDirection.NW: return new Vector2(-0.5f, 0.86f);
			case HexDirection.E: return new Vector2(1.0f, 0.0f);
			case HexDirection.W: return new Vector2(-1.0f, 0.0f);
		}
		return new Vector2(0.0f, 0.0f);
	}
}