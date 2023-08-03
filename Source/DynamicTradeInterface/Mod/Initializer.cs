﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace DynamicTradeInterface.Mod
{

	[StaticConstructorOnStartup]
	static class Initializer
	{
		static Initializer()
		{
			DynamicTradeInterfaceMod.Harmony.PatchAll();
			DynamicTradeInterfaceMod.Settings.Initialize();
		}
	}
}
