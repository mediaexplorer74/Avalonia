// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using OmniXaml;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Data;

namespace Avalonia.Markup.Xaml.MarkupExtensions
{
    public class TemplateBindingExtension : MarkupExtension
    {
        public TemplateBindingExtension()
        {
        }

        public TemplateBindingExtension(string path)
        {
            Path = path;
        }

        public override object ProvideValue(MarkupExtensionContext extensionContext)
        {
            return new Binding
            {
                Converter = Converter,
                ElementName = ElementName,
                Mode = Mode,
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Path = Path,
                Priority = Priority,
            };
        }

        public IValueConverter Converter { get; set; }
        public string ElementName { get; set; }
        public object FallbackValue { get; set; }
        public BindingMode Mode { get; set; }
        public string Path { get; set; }
        public BindingPriority Priority { get; set; } = BindingPriority.TemplatedParent;
    }
}