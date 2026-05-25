using Microsoft.Win32;   // for OpenFileDialog
using System;
using System.Collections.Generic;
using System.Windows;

namespace WindowsToolsLauncher
{
    public partial class ManageButtonsWindow : Window
    {
        public List<CustomTool> CustomTools { get; private set; }

        public ManageButtonsWindow(List<CustomTool> existingTools)
        {
            InitializeComponent();
            CustomTools = new List<CustomTool>(existingTools);
            RefreshListBox();
        }

        private void RefreshListBox()
        {
            ToolsListBox.ItemsSource = null;
            ToolsListBox.ItemsSource = CustomTools;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CommandTextBox.Text))
            {
                MessageBox.Show("Both Name and Command are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CustomTools.Add(new CustomTool
            {
                Name = NameTextBox.Text.Trim(),
                Command = CommandTextBox.Text.Trim()
            });

            NameTextBox.Clear();
            CommandTextBox.Clear();
            RefreshListBox();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToolsListBox.SelectedItem is CustomTool selected)
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text) || string.IsNullOrWhiteSpace(CommandTextBox.Text))
                {
                    MessageBox.Show("Both Name and Command are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.Name = NameTextBox.Text.Trim();
                selected.Command = CommandTextBox.Text.Trim();
                RefreshListBox();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToolsListBox.SelectedItem is CustomTool selected)
            {
                CustomTools.Remove(selected);
                RefreshListBox();
            }
        }

        private void SaveCloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select a program or file",
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)
            };

            if (dialog.ShowDialog() == true)
            {
                CommandTextBox.Text = dialog.FileName;
            }
        }
    }
}