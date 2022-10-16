// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Controls;

namespace Avalonia.Layout.UnitTests
{
    internal class TestLayoutRoot : Decorator, ILayoutRoot
    {
        public TestLayoutRoot()
        {
            ClientSize = new Size(500, 500);
        }

        public Size ClientSize
        {
            get;
            set;
        }

        public Size MaxClientSize => Size.Infinity;
        public double LayoutScaling => 1;
    }
}
