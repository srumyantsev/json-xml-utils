using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CommonConverter {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();

			ContentContainer.LogAction = WriteLogMessage;
		}

		private void AddProcessStepOnClick(object sender, RoutedEventArgs e) {
			var processStepSelector = new ComboBox();
			processStepSelector.ItemsSource = Enum.GetValues(typeof(ProcessStep));
			processStepSelector.SelectedValue = ProcessStep.DecodeBase64;

			var removeProcessStepSelectorButton = new Button();
			removeProcessStepSelectorButton.Content = "-";
			removeProcessStepSelectorButton.Click += (o, args) => {
				var parentWrapPanel = (o as Button).Parent as WrapPanel;
				ProcessContentPanel.Children.Remove(parentWrapPanel);
			};

			var wrapPanel = new WrapPanel();
			wrapPanel.Children.Add(processStepSelector);
			wrapPanel.Children.Add(removeProcessStepSelectorButton);
			ProcessContentPanel.Children.Add(wrapPanel);
		}

		private void ConvertOnClick(object sender, RoutedEventArgs e) {
			try {
				Execute();
			} catch (Exception exception) {
				string errorMessage = $"Error: {exception.Message}";
				MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				WriteLogMessage(errorMessage);
			}
		}

		private void Execute() {
			ContentContainer contentContainer;
			if (SourceTypeFile.IsChecked.HasValue && SourceTypeFile.IsChecked.Value) {
				contentContainer = new ContentContainer(File.ReadAllBytes(SourceFilePath.Text));
			} else {
				if (string.IsNullOrEmpty(SourceRawContent.Text)) {
					throw new Exception("Source raw content is invalid");
				}
				contentContainer = new ContentContainer(SourceRawContent.Text);
			}

			if (ProcessContentPanel.Children.Count < 1) {
				throw new Exception("No process steps");
			}

			for (int i = 0; i < ProcessContentPanel.Children.Count; i++) {
				var processStepSelector = (ProcessContentPanel.Children[i] as WrapPanel).Children[0] as ComboBox;

				var processStep = (ProcessStep)processStepSelector.SelectedValue;
				contentContainer.ExecuteProcessStep(processStep);
			}

			if (ResultTypeFile.IsChecked.HasValue && ResultTypeFile.IsChecked.Value) {
				if (string.IsNullOrEmpty(ResultFilePath.Text)) {
					throw new Exception("Result file path is invalid");
				}

				if (File.Exists(ResultFilePath.Text)) {
					MessageBoxResult overwriteFile = MessageBox.Show($"File {ResultFilePath.Text} already exists.{Environment.NewLine}Overwrite?{Environment.NewLine}[YES] - overwrite{Environment.NewLine}[NO] - apply unique part to file name", "File exists", MessageBoxButton.YesNo);
					if (overwriteFile == MessageBoxResult.Yes) {
						File.Delete(ResultFilePath.Text);
					} else {
						ResultFilePath.Text = ResultFilePath.Text.Replace(Path.GetExtension(ResultFilePath.Text), $"_{DateTime.Now:yyyy-MM-dd_HH-mm}{Path.GetExtension(ResultFilePath.Text)}");
					}
				}

				File.WriteAllBytes(ResultFilePath.Text, contentContainer.Value);
				MessageBox.Show("File successfuly saved. Opening folder with file.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
				Process.Start("explorer.exe", $"/select, \"{ResultFilePath.Text}\"");
			} else {
				ResultRawContent.Text = contentContainer.ValueString;
				MessageBox.Show("Text successfuly converted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void WriteLogMessage(string message) {
			LogControl.Text = LogControl.Text.Insert(0, $"[{DateTime.Now.ToShortDateString()}] {message}");
		}
	}
}
