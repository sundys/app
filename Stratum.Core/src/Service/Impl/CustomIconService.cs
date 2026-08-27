// Copyright (C) 2022 jmh
// SPDX-License-Identifier: GPL-3.0-only

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Stratum.Core.Entity;
using Stratum.Core.Persistence;
using Stratum.Core.Persistence.Exception;

namespace Stratum.Core.Service.Impl
{
    public class CustomIconService : ICustomIconService
    {
        private readonly ICustomIconRepository _customIconRepository;
        private readonly IAuthenticatorRepository _authenticatorRepository;
        private readonly ICategoryRepository _categoryRepository;

        public CustomIconService(ICustomIconRepository customIconRepository,
            IAuthenticatorRepository authenticatorRepository, ICategoryRepository categoryRepository)
        {
            _customIconRepository = customIconRepository;
            _authenticatorRepository = authenticatorRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task AddIfNotExistsAsync(CustomIcon icon)
        {
            ArgumentNullException.ThrowIfNull(icon);
            var existing = await _customIconRepository.GetAsync(icon.Id);

            if (existing == null)
            {
                await _customIconRepository.CreateAsync(icon);
            }
        }

        public async Task<int> AddManyAsync(IEnumerable<CustomIcon> icons)
        {
            ArgumentNullException.ThrowIfNull(icons);

            var added = 0;

            foreach (var icon in icons)
            {
                try
                {
                    await _customIconRepository.CreateAsync(icon);
                }
                catch (EntityDuplicateException)
                {
                    continue;
                }

                added++;
            }

            return added;
        }

        public Task<List<CustomIcon>> GetAllAsync()
        {
            return _customIconRepository.GetAllAsync();
        }

        public async Task CullUnusedAsync()
        {
            var authenticators = await _authenticatorRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var icons = await _customIconRepository.GetAllAsync();

            var iconsInUse = authenticators
                .Select(a => a.Icon)
                .Concat(categories.Select(c => c.Icon))
                .Where(icon => icon != null && icon.StartsWith(CustomIcon.Prefix))
                .Select(icon => icon[1..])
                .Distinct();

            var unusedIcons = icons.Where(i => !iconsInUse.Contains(i.Id));

            foreach (var icon in unusedIcons)
            {
                await _customIconRepository.DeleteAsync(icon);
            }
        }
    }
}