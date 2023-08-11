﻿using DynamicTradeInterface.InterfaceComponents.TableBox;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace DynamicTradeInterface.Defs
{
	public class TradeColumnDef : Def
	{
		internal delegate void TradeColumnCallback(ref Rect boundingBox, Tradeable item, Transactor transactor, ref bool refresh);
		internal delegate Func<Tradeable, IComparable> TradeColumnOrderValueCallback(Transactor transactor);

		/// <summary>
		/// Colon-based method identifier string for column draw callback.
		/// </summary> 
		public string? callbackHandler = null;
		public string? orderValueCallbackHandler = null;
		public float defaultWidth = 100;

		/// <summary>
		/// Whether or not the column should be automatically added to visible columns.
		/// </summary>
		public bool defaultVisible = true;

		/// <summary>
		/// Whether the label should be shown in the column header.
		/// </summary>
		public bool showCaption = true;

		/// <summary>
		/// If set then this is shown as a tooltip on column header, otherwise label is used.
		/// </summary>
		public string? tooltip;

		/// <summary>
		/// The direction the column will sort by on first click. Descending is default.
		/// </summary>
		public SortDirection initialSort = SortDirection.Descending;

		//TODO way to add additional data to search string.

		internal TradeColumnCallback? _callback;
		internal TradeColumnOrderValueCallback? _orderValueCallback;

		// TODO consider if this override is required.
		public override void ResolveReferences()
		{
			base.ResolveReferences();
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (var error in base.ConfigErrors()) 
			{
				yield return error;
			}

			if (string.IsNullOrEmpty(callbackHandler))
				yield return "TradeColumnDef must have a callbackHandler defined.";
		}

	}
}
