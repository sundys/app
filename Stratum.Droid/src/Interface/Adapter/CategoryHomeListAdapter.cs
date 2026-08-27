// Copyright (C) 2026
// SPDX-License-Identifier: GPL-3.0-only

using System;
using Android.Content;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Stratum.Core.Entity;
using Stratum.Droid.Interface.ViewHolder;
using Stratum.Droid.Persistence.View;
using Stratum.Droid.Shared;

namespace Stratum.Droid.Interface.Adapter
{
    public class CategoryHomeListAdapter : RecyclerView.Adapter, IReorderableListAdapter, IRestrictedReorderableListAdapter
    {
        private readonly Context _context;
        private readonly ICategoryView _categoryView;
        private readonly ICustomIconView _customIconView;
        private readonly bool _isDark;
        private readonly bool _showUncategorised;

        private int MetaCategoryCount => _showUncategorised ? 2 : 1;
        public override int ItemCount => _categoryView.Count + MetaCategoryCount;

        public CategoryHomeListAdapter(Context context, ICategoryView categoryView, ICustomIconView customIconView,
            bool isDark, bool showUncategorised)
        {
            _context = context;
            _categoryView = categoryView;
            _customIconView = customIconView;
            _isDark = isDark;
            _showUncategorised = showUncategorised;
        }

        public event EventHandler<CategorySelector> CategorySelected;
        public event EventHandler<string> MenuClicked;
        public event EventHandler<bool> MovementFinished;

        public bool CanMove(int position)
        {
            return position >= MetaCategoryCount && position < ItemCount;
        }

        public void MoveItemView(int oldPosition, int newPosition)
        {
            if (!CanMove(oldPosition) || !CanMove(newPosition))
            {
                return;
            }

            _categoryView.Swap(oldPosition - MetaCategoryCount, newPosition - MetaCategoryCount);
            NotifyItemMoved(oldPosition, newPosition);
        }

        public void OnMovementStarted()
        {
        }

        public void OnMovementFinished(bool orderChanged)
        {
            MovementFinished?.Invoke(this, orderChanged);
        }

        public override long GetItemId(int position)
        {
            return position switch
            {
                0 => -1,
                1 when MetaCategoryCount == 2 => -2,
                _ => _categoryView[position - MetaCategoryCount].Id.GetHashCode()
            };
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = (CategoryHomeListHolder) viewHolder;
            var isCustom = CanMove(position);
            Category category = isCustom ? _categoryView[position - MetaCategoryCount] : null;

            holder.Name.Text = position switch
            {
                0 => _context.GetString(Resource.String.categoryAll),
                1 when MetaCategoryCount == 2 => _context.GetString(Resource.String.categoryUncategorised),
                _ => category.Name
            };

            holder.MenuButton.Visibility = isCustom ? ViewStates.Visible : ViewStates.Gone;

            if (category?.Icon != null && category.Icon.StartsWith(CustomIcon.Prefix))
            {
                var bitmap = _customIconView.GetOrDefault(category.Icon[1..]);

                if (bitmap != null)
                {
                    holder.Icon.SetImageBitmap(bitmap);
                    return;
                }
            }

            holder.Icon.SetImageResource(isCustom
                ? IconResolver.GetService(category?.Icon, _isDark)
                : Resource.Drawable.baseline_folder_24);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.listItemCategoryHome, parent, false);
            var holder = new CategoryHomeListHolder(itemView);

            holder.ItemView.Click += delegate
            {
                var position = holder.BindingAdapterPosition;

                if (position == RecyclerView.NoPosition)
                {
                    return;
                }

                var selector = position switch
                {
                    0 => CategorySelector.Of(MetaCategory.All),
                    1 when MetaCategoryCount == 2 => CategorySelector.Of(MetaCategory.Uncategorised),
                    _ => CategorySelector.Of(_categoryView[position - MetaCategoryCount].Id)
                };

                CategorySelected?.Invoke(this, selector);
            };

            holder.MenuButton.Click += delegate
            {
                var position = holder.BindingAdapterPosition;

                if (CanMove(position))
                {
                    MenuClicked?.Invoke(this, _categoryView[position - MetaCategoryCount].Id);
                }
            };

            return holder;
        }
    }
}
