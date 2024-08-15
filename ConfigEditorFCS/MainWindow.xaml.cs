using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConfigEditorFCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<DirectoryPair> directoryPairs;
        private string configFilePath = @"C:\Projects\FileCopyService\FileCopyService\config.json";

        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            try
            {
                if (File.Exists(configFilePath))
                {
                    string json = File.ReadAllText(configFilePath);
                    directoryPairs = JsonConvert.DeserializeObject<List<DirectoryPair>>(json);
                    listBoxPairs.ItemsSource = directoryPairs;
                }
                else
                {
                    directoryPairs = new List<DirectoryPair>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load configuration: {ex.Message}");
            }
        }

        private void AddPair_Click(object sender, RoutedEventArgs e)
        {
            var newPair = new DirectoryPair
            {
                SourceDir = txtSourceDir.Text,
                TargetDir = txtTargetDir.Text
            };
            directoryPairs.Add(newPair);
            listBoxPairs.ItemsSource = null;
            listBoxPairs.ItemsSource = directoryPairs;
        }

        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string json = JsonConvert.SerializeObject(directoryPairs, Formatting.Indented);
                File.WriteAllText(configFilePath, json);
                MessageBox.Show("Configuration saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save configuration: {ex.Message}");
            }
        }
    }

    public class DirectoryPair
    {
        public string SourceDir { get; set; }
        public string TargetDir { get; set; }

        public string Display => $"Source: {SourceDir}, Target: {TargetDir}";
    }

    public static class TextBoxPlaceholder
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(TextBoxPlaceholder), new PropertyMetadata(default(string), OnPlaceholderChanged));

        public static void SetPlaceholder(DependencyObject element, string value)
        {
            element.SetValue(PlaceholderProperty, value);
        }

        public static string GetPlaceholder(DependencyObject element)
        {
            return (string)element.GetValue(PlaceholderProperty);
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if (textBox.Text == string.Empty)
                {
                    textBox.Text = (string)e.NewValue;
                    textBox.Foreground = Brushes.Gray;
                }

                textBox.GotFocus += (sender, args) =>
                {
                    if (textBox.Text == (string)e.NewValue)
                    {
                        textBox.Text = string.Empty;
                        textBox.Foreground = Brushes.Black;
                    }
                };

                textBox.LostFocus += (sender, args) =>
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = (string)e.NewValue;
                        textBox.Foreground = Brushes.Gray;
                    }
                };
            }
        }
    }
}
