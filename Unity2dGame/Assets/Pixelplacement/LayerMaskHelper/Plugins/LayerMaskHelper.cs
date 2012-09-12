using UnityEngine;

public class LayerMaskHelper {
	
	public static int OnlyIncluding( params int[] layers ){
		return MakeMask( layers );
	}
	
	public static int EverythingBut( params int[] layers ){
		return ~MakeMask( layers );
	}
	
	static int MakeMask( params int[] layers ){
		int mask = 0;
		foreach ( int item in layers ) {
			mask |= 1 << item;
		}
		return mask;	
	}
	
}
