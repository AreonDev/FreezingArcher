using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Pencil.Gaming.Scripting {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
	public delegate int LuaCFunction(LuaStatePtr l);
	[return: MarshalAs(UnmanagedType.LPStr)]
	public delegate string LuaReader(LuaStatePtr l,IntPtr ud,out int sz);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
	public delegate int LuaWriter(LuaStatePtr l,IntPtr p,int sz,IntPtr ud);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
	public delegate IntPtr LuaAlloc(IntPtr ud,IntPtr ptr,int osize,int nsize);
	[UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
	public delegate void LuaHook(LuaStatePtr l,LuaDebugPtr ar);
}

