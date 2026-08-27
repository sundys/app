// Copyright (C) 2026
// SPDX-License-Identifier: GPL-3.0-only

using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextView;

namespace Stratum.Droid.Interface.ViewHolder
{
    public class CategoryHomeListHolder : RecyclerView.ViewHolder
    {
        public CategoryHomeListHolder(View itemView) : base(itemView)
        {
            Icon = itemView.FindViewById<ImageView>(Resource.Id.imageIcon);
            Name = itemView.FindViewById<MaterialTextView>(Resource.Id.textName);
            MenuButton = itemView.FindViewById<ImageButton>(Resource.Id.buttonMenu);
        }

        public ImageView Icon { get; }
        public MaterialTextView Name { get; }
        public ImageButton MenuButton { get; }
    }
}
