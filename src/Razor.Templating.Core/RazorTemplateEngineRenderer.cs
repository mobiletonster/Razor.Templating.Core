﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Razor.Templating.Core
{
    internal sealed class RazorTemplateEngineRenderer : IRazorTemplateEngine
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorTemplateEngine"/> class.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <exception cref="ArgumentNullException"><paramref name="serviceProvider"/> is null.</exception>
        public RazorTemplateEngineRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Renders View(.cshtml) To String
        /// </summary>
        /// <param name="viewName">Relative path of the .cshtml view. Eg:  /Views/YourView.cshtml or ~/Views/YourView.cshtml</param>
        /// <param name="viewModel">Strongly typed object</param>
        /// <param name="viewBagOrViewData">ViewData</param>
        /// <returns></returns>
        public async Task<string> RenderAsync(string viewName, object? viewModel = null, Dictionary<string, object>? viewBagOrViewData = null)
        {
            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            foreach (var keyValuePair in viewBagOrViewData ?? new())
            {
                viewDataDictionary.Add(keyValuePair!);
            }

            using var serviceScope = _serviceProvider.CreateScope();
            var renderer = serviceScope.ServiceProvider.GetRequiredService<RazorViewToStringRenderer>();
            return await renderer.RenderViewToStringAsync(viewName, viewModel, viewDataDictionary).ConfigureAwait(false);
        }
    }
}