// Copyright (C) 2022 jmh
// SPDX-License-Identifier: GPL-3.0-only

using System;
using Android.OS;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Stratum.Droid.Interface.Fragment
{
    public class MainMenuBottomSheet : BottomSheet, IAutoDismissFragment
    {
        public MainMenuBottomSheet() : base(Resource.Layout.sheetMainMenu, Resource.String.mainMenu)
        {
        }

        public event EventHandler BackupClicked;
        public event EventHandler CategoriesClicked;
        public event EventHandler IconPacksClicked;
        public event EventHandler SettingsClicked;
        public event EventHandler AboutClicked;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = base.OnCreateView(inflater, container, savedInstanceState);
            var menu = view.FindViewById<RecyclerView>(Resource.Id.listMenu);
            SetupMenu(menu,
            [
                new SheetMenuItem(Resource.Drawable.baseline_save_24, Resource.String.backup, BackupClicked),
                new SheetMenuItem(Resource.Drawable.baseline_category_24, Resource.String.categories, CategoriesClicked),
                new SheetMenuItem(Resource.Drawable.baseline_folder_24, Resource.String.iconPacks, IconPacksClicked),
                new SheetMenuItem(Resource.Drawable.baseline_settings_24, Resource.String.settings, SettingsClicked),
                new SheetMenuItem(Resource.Drawable.outline_info_24, Resource.String.about, AboutClicked)
            ]);

            return view;
        }
    }
}
