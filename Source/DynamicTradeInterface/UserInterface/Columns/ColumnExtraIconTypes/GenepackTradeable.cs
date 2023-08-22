﻿using DynamicTradeInterface.InterfaceComponents;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DynamicTradeInterface.UserInterface.Columns.ColumnExtraIconTypes
{
	internal static class EnumerableExt
	{
		public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
		{
			foreach (IEnumerable<T> outer in source)
				foreach (T inner in outer)
					yield return inner;
		}

		public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult?> selector)
		{
			foreach (TSource elem in source)
			{
				TResult? result = selector(elem);
				if (result != null)
					yield return result;
			}
		}
	}

	internal class GenepackTradeable : ITradeable
	{
		private static Dictionary<GeneDef, GeneAssistant.GeneType> _bankedGenes = new Dictionary<GeneDef, GeneAssistant.GeneType>();

		private GeneAssistant.GeneType _geneType = GeneAssistant.GeneType.None;
		private Texture? _icon = null;
		private string _tooltip = string.Empty;

		public static void PostOpen(IEnumerable<Tradeable> rows, Transactor transactor)
		{
			if (transactor != Transactor.Colony)
				return;

			var genepacks = Find.CurrentMap
				?.listerBuildings
				.allBuildingsColonist
				.SelectNotNull((building) => building.TryGetComp<CompGenepackContainer>()?.ContainedGenepacks)
				.Flatten()
				.SelectNotNull((genepack) => genepack.GeneSet?.GenesListForReading);
			if (genepacks != null)
			{
				foreach (var genes in genepacks)
				{
					if (genes.Count == 1)
						_bankedGenes[genes[0]] = GeneAssistant.GeneType.Isolated;
					else
						foreach (var gene in genes)
							_bankedGenes.TryAdd(gene, GeneAssistant.GeneType.Mixed);
				}
			}
		}

		public static void PostClosed(IEnumerable<Tradeable> rows, Transactor transactor)
		{
			if (transactor == Transactor.Colony)
				_bankedGenes.Clear();
		}

		public GenepackTradeable(Genepack genepack)
		{
			List<GeneAssistant.GeneType>? types = genepack.GeneSet
				?.GenesListForReading
				.Select((gene) => _bankedGenes.TryGetValue(gene, GeneAssistant.GeneType.Missing))
				.Distinct()
				.ToList();

			if (types?.Count > 0)
			{
				if (types.Any((x) => x == GeneAssistant.GeneType.Missing))
				{
					_geneType = GeneAssistant.GeneType.Missing;
					_tooltip = "DynamicTradeWindowGenepackMissing".Translate();
				}
				else if (types.All((x) => x == GeneAssistant.GeneType.Isolated))
				{
					_geneType = GeneAssistant.GeneType.Isolated;
					_tooltip = "DynamicTradeWindowGenepackIsolated".Translate();
				}
				else
				{
					_geneType = GeneAssistant.GeneType.Mixed;
					_tooltip = "DynamicTradeWindowGenepackMixed".Translate();
				}
			}

			_icon = GeneAssistant.IconFor(_geneType);
		}

		public void Draw(ref Rect rect, Tradeable row, Transactor transactor, ref bool refresh)
		{
			if (_icon != null)
			{
				Rect iconRect = new Rect(rect.xMax - rect.height, rect.y, rect.height, rect.height).ContractedBy(1);
				GUI.DrawTexture(iconRect, _icon);

				if (Mouse.IsOver(iconRect))
					TooltipHandler.TipRegion(iconRect, _tooltip);
			}
		}
	}
}
