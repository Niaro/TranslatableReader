using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Template10.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TranslatableReader.Controls
{
	internal static class Extensions
	{
		public static ChangedEventArgs<T> ToChangedEventArgs<T>(this DependencyPropertyChangedEventArgs e)
			=> new ChangedEventArgs<T>((T)e.OldValue, (T)e.NewValue);
	}

	public sealed partial class RichTextViewer : UserControl
	{
		public List<Paragraph> Source
		{
			get { return (List<Paragraph>)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public static readonly DependencyProperty SourceProperty =
			 DependencyProperty.Register(nameof(Source), typeof(List<Paragraph>),
				 typeof(RichTextViewer), new PropertyMetadata(null, (d, e) =>
				 {
					 Changed(nameof(Source), e);
					 (d as RichTextViewer).SourceChanged?.Invoke(d, e.ToChangedEventArgs<List<Paragraph>>());
				 }));

		public event EventHandler<ChangedEventArgs<List<Paragraph>>> SourceChanged;

		#region Debug

		private static void DebugWrite(string text = null, Template10.Services.LoggingService.Severities severity = Template10.Services.LoggingService.Severities.Template10, [CallerMemberName]string caller = null) =>
			 Template10.Services.LoggingService.LoggingService.WriteLine(text, severity, caller: $"RichTextViewer.{caller}");

		#endregion Debug

		public RichTextViewer()
		{
			this.InitializeComponent();

			SourceChanged += RichTextViewer_SourceChanged;
			TextViewerListContainer.ItemsSource = Source;
		}

		private void RichTextViewer_SourceChanged(object sender, ChangedEventArgs<List<Paragraph>> e)
		{
			TextViewerListContainer.ItemsSource = e.NewValue;
		}

		// debug write change
		private static void Changed(string v, DependencyPropertyChangedEventArgs e)
		{
			DebugWrite($"OldValue: {e.OldValue} NewValue: {e.NewValue}", caller: v);
		}
	}

	public class RichListViewItem
	{
	}

	public class RichTextBlockHelper : DependencyObject
	{
		public static readonly DependencyProperty BlocksProperty =
			DependencyProperty.RegisterAttached("Blocks", typeof(object),
				typeof(RichTextBlockHelper),
				new PropertyMetadata(default(object), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var control = sender as RichTextBlock;
			if (control != null)
			{
				control.Blocks.Clear();
				var value = (Paragraph)e.NewValue;
				control.Blocks.Add(value);
			}
		}

		public static object GetBlocks(UIElement element)
		{
			return (object)element.GetValue(BlocksProperty);
		}

		public static void SetBlocks(UIElement element, object value)
		{
			element.SetValue(BlocksProperty, value);
		}
	}
}