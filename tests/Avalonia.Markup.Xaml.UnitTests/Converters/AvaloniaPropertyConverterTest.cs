// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive;
using Moq;
using OmniXaml;
using OmniXaml.ObjectAssembler.Commands;
using OmniXaml.TypeConversion;
using OmniXaml.Typing;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Styling;
using Xunit;

namespace Avalonia.Markup.Xaml.UnitTests.Converters
{
    public class AvaloniaPropertyConverterTest
    {
        public AvaloniaPropertyConverterTest()
        {
            // Ensure properties are registered.
            var foo = Class1.FooProperty;
            var attached = AttachedOwner.AttachedProperty;
        }

        [Fact]
        public void ConvertFrom_Finds_Fully_Qualified_Property()
        {
            var target = new AvaloniaPropertyTypeConverter();
            var context = CreateContext();
            var result = target.ConvertFrom(context, null, "Class1.Foo");

            Assert.Equal(Class1.FooProperty, result);
        }

        [Fact]
        public void ConvertFrom_Uses_Selector_TargetType()
        {
            var target = new AvaloniaPropertyTypeConverter();
            var style = new Style(x => x.OfType<Class1>());
            var context = CreateContext(style);
            var result = target.ConvertFrom(context, null, "Foo");

            Assert.Equal(Class1.FooProperty, result);
        }

        [Fact]
        public void ConvertFrom_Finds_Attached_Property()
        {
            var target = new AvaloniaPropertyTypeConverter();
            var context = CreateContext();
            var result = target.ConvertFrom(context, null, "AttachedOwner.Attached");

            Assert.Equal(AttachedOwner.AttachedProperty, result);
        }

        private IValueContext CreateContext(Style style = null)
        {
            var context = new Mock<IValueContext>();
            var topDownValueContext = new Mock<ITopDownValueContext>();
            var typeRepository = new Mock<ITypeRepository>();
            var featureProvider = new Mock<ITypeFeatureProvider>();
            var class1XamlType = new XamlType(typeof(Class1), typeRepository.Object, null, featureProvider.Object);
            var attachedOwnerXamlType = new XamlType(typeof(AttachedOwner), typeRepository.Object, null, featureProvider.Object);
            context.Setup(x => x.TopDownValueContext).Returns(topDownValueContext.Object);
            context.Setup(x => x.TypeRepository).Returns(typeRepository.Object);
            topDownValueContext.Setup(x => x.GetLastInstance(It.IsAny<XamlType>())).Returns(style);
            typeRepository.Setup(x => x.GetByQualifiedName("Class1")).Returns(class1XamlType);
            typeRepository.Setup(x => x.GetByQualifiedName("AttachedOwner")).Returns(attachedOwnerXamlType);
            return context.Object;
        }

        private class Class1 : AvaloniaObject, IStyleable
        {
            public static readonly StyledProperty<string> FooProperty =
                AvaloniaProperty.Register<Class1, string>("Foo");

            public IAvaloniaReadOnlyList<string> Classes
            {
                get { throw new NotImplementedException(); }
            }

            public string Name
            {
                get { throw new NotImplementedException(); }
            }

            public Type StyleKey
            {
                get { throw new NotImplementedException(); }
            }

            public ITemplatedControl TemplatedParent
            {
                get { throw new NotImplementedException(); }
            }

            IObservable<IStyleable> IStyleable.StyleDetach { get; }
        }

        private class AttachedOwner
        {
            public static readonly AttachedProperty<string> AttachedProperty =
                AvaloniaProperty.RegisterAttached<AttachedOwner, Class1, string>("Attached");
        }
    }
}
