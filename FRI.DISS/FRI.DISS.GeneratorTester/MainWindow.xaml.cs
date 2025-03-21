using FRI.DISS.Libs.Generators;
using System.IO;
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

namespace FRI.DISS.GeneratorTester
{
    internal enum GeneratorType
    {
        UniformDiscrete,
        UniformContinuous,
        EmpiricalDiscrete,
        EmpiricalContinuous,
        Exponential,
        Triangular
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileInfo? _lastTestFile;
        private string _testFilesDirectory = "./generations";


        public MainWindow()
        {
            InitializeComponent();

            _initializeGUI();

            if (!Directory.Exists(_testFilesDirectory))
            {
                Directory.CreateDirectory(_testFilesDirectory);
            }
        }

        private void _initializeGUI()
        {
            // Initialize ComboBox
            foreach (GeneratorType type in Enum.GetValues(typeof(GeneratorType)))
            {
                _cmbx_GeneratorType.Items.Add(type);
            }
            _cmbx_GeneratorType.SelectedIndex = 4; // Exponential
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void _btn_Generate_Click(object sender, RoutedEventArgs e)
        {
            try {
                var generatorType = (GeneratorType)_cmbx_GeneratorType.SelectedItem;
                var seed = int.Parse(_txtbx_GeneratorSeed.Value.ToString());
                var samplesCount = int.Parse(_txtbx_GeneratorSamplesCount.Value.ToString());

                var saveSamples = _chkbx_SaveSamples.IsChecked ?? false;
                var renderSamples = _chkbx_RenderSamples.IsChecked ?? false;

                _clearGUI();

                AbstractGenerator generator = _createGenerator(generatorType, seed);
                Statistics statistics = new();

                const int PeriodicSaveSamplesInterval = 50000; // iba odhanuta hodnota po experimentovani, aby sa nepristupovalo k disku prilis casto
                StringBuilder sb = new();
                _lastTestFile = new FileInfo($"{_testFilesDirectory}/{generatorType}_{seed}_{samplesCount}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
                StreamWriter sw = _lastTestFile.CreateText();

                var timer = new System.Diagnostics.Stopwatch();
                timer.Start();

                for (int i = 0; i < samplesCount; i++)
                {
                    var sample = generator.Mode == GenerationMode.Discrete
                        ? generator.GetSampleInt()
                        : generator.GetSampleDouble();
                    statistics.AddSample(sample);

                    if (saveSamples)
                    {
                        sb.AppendLine(sample.ToString());

                        // Save periodically to avoid memory issues with large sample counts
                        if ((i + 1) % PeriodicSaveSamplesInterval == 0 || i == samplesCount - 1)
                        {
                            sw.Write(sb.ToString());
                            sb.Clear();
                        }
                    }

                    if (renderSamples)
                    {
                        _lstbx_GeneratedSamples.Items.Add(sample);
                    }
                }
                timer.Stop();
                sw.Close();

                // update statistics 
                _txt_GeneratedSamplesTime.Value = $"{timer.Elapsed.Minutes:00}:{timer.Elapsed.Seconds:00}:{timer.Elapsed.Milliseconds:000}";
                _updateStatistics(statistics);

                MessageBox.Show($"Samples generation done.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void _updateStatistics(Statistics statistics)
        {
            _txt_GeneratedSamplesCount.Value = statistics.Count.ToString();

            _txt_GeneratedSamplesMin.Value = statistics.Min.ToString();
            _txt_GeneratedSamplesMax.Value = statistics.Max.ToString();
            
            _txt_GeneratedSamplesMean.Value = statistics.Mean.ToString();
            _txt_GeneratedSamplesVariance.Value = statistics.Variance.ToString();
            _txt_GeneratedSamplesStdDev.Value = statistics.StandardDeviation.ToString();
        }

        private AbstractGenerator _createGenerator(GeneratorType generatorType, int seed)
        {
            var seedGenerator = new SeedGenerator(seed);

            switch (generatorType)
            {
                case GeneratorType.UniformDiscrete:
                    return new UniformGenerator(GenerationMode.Discrete, seedGenerator)
                    {
                        Min=1,
                        Max=10
                    };
                case GeneratorType.UniformContinuous:
                    return new UniformGenerator(GenerationMode.Continuous, seedGenerator);
                case GeneratorType.EmpiricalDiscrete:
                case GeneratorType.EmpiricalContinuous:
                    return new EmpiricalGenerator (
                        generatorType == GeneratorType.EmpiricalDiscrete 
                            ? GenerationMode.Discrete
                            : GenerationMode.Continuous,
                        [5, 10, 50, 70, 80, 95],
                        [0.4, 0.3, 0.2, 0.06, 0.04],
                        seedGenerator
                    );
                case GeneratorType.Exponential:
                    return new ExponentialGenerator((double)1/30, seedGenerator);
                case GeneratorType.Triangular:
                    return new TriangularGenerator(60, 500, 150, seedGenerator);
                default:
                    throw new ArgumentException("Invalid generator type");
            }
        }

        private void _clearGUI()
        {
            _lstbx_GeneratedSamples.Items.Clear();

            _txt_GeneratedSamplesCount.Value = "0";
            _txt_GeneratedSamplesMin.Value = "0";
            _txt_GeneratedSamplesMax.Value = "0";
            _txt_GeneratedSamplesMean.Value = "0";
            _txt_GeneratedSamplesVariance.Value = "0";
            _txt_GeneratedSamplesStdDev.Value = "0";
        }

        private void _btn_SaveSamples_Click(object sender, RoutedEventArgs e)
        {
            if (_lastTestFile is null)
            {
                MessageBox.Show("No samples to save.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = _lastTestFile.Name,
                DefaultExt = ".txt",
                Filter = "Text documents (.txt)|*.txt"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                MessageBox.Show("No file selected.", "Info", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            File.Copy(_lastTestFile.FullName, saveFileDialog.FileName, true);
            MessageBox.Show($"Samples saved to file: {saveFileDialog.FileName}", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}