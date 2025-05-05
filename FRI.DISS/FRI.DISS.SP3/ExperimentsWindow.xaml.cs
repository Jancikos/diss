using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FRI.DISS.Libs.Helpers;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using OSPABA;

namespace FRI.DISS.SP3
{
    /// <summary>
    /// Interaction logic for ExperimentsWindow.xaml
    /// </summary>
    public partial class ExperimentsWindow : Window
    {
        private readonly MySimulation _simulation;
        private int? _confCurrentRunnig;
        private double? _confIntervalWidthPercent; 
        private string[]? _confings;

        private FileInfo _fileConfigs = new FileInfo("./experimentsConfigs.csv");
        private FileInfo _fileOutput = new FileInfo("./experimentsOutput.csv");

        public ExperimentsWindow()
        {
            InitializeComponent();

            _simulation = new MySimulation();

            _simulation.OnReplicationWillStart(_onReplicationWillStart);
            _simulation.OnReplicationDidFinish(_onReplicationDidFinish);
            _simulation.OnSimulationWillStart(_onSimulationWillStart);
            _simulation.OnSimulationDidFinish(_onSimulationDidFinish);

            _loadConfigsFromFile(_fileConfigs.FullName);
        }

        private void _onSimulationWillStart(Simulation simulation)
        {
            Dispatcher.InvokeAsync(() =>
            {
                _txt_simCurrentReplication.Text = "0";
                _txt_simTotalReplications.Text = simulation.ReplicationCount.ToString();
                _txt_simStartTime.Value = DateTime.Now.ToString("HH:mm:ss");
            });
        }
        private void _onSimulationDidFinish(Simulation simulation)
        {
            if (_confCurrentRunnig is null)
                throw new Exception("Current configuration running can not be null.");

            Dispatcher.InvokeAsync(() =>
            {
                _txt_confsCurrent.Text = (_confCurrentRunnig + 1).ToString();
            });

            // save simulation results to csv file
            if (_simulation.CurrentReplication >= 30)
            {
                NabytokReplicationsStatisticsCsvWriter.Instance.Write(
                    _fileOutput,
                    _simulation
                );
            }

            // should start another simulation
            var runAnotherSimulation = _confCurrentRunnig < _confings?.Length - 1;
            if (!runAnotherSimulation)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    _txt_confsEndTime.Value = DateTime.Now.ToString("HH:mm:ss");
                    MessageBox.Show("All configurations have been simulated.", "Simulation finished", MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return;
            }

            // start another simulation
            _startSimulation(_confCurrentRunnig.Value + 1);
        }

        private void _onReplicationWillStart(Simulation simulation)
        {
        }
        private void _onReplicationDidFinish(Simulation simulation)
        {
            var repRefershInterval = _simulation.RepliactionsRefreshStatisticsInterval;
            var repsDone = _simulation.CurrentReplication;

            Dispatcher.InvokeAsync(() =>
            {
                _txt_simCurrentReplication.Text = (repsDone + 1).ToString();
            });

            // check if interval is not short enough
            var orderTotalTime = _simulation.ReplicationsStatistics.ObjednavkaTime;
            if (orderTotalTime.Count > 30)
            {
                var intervalWidth = orderTotalTime.IntervalWidth;
                var intervalWidthPercent = intervalWidth / orderTotalTime.Mean * 100;

                // Debug.WriteLine($"[{_confCurrentRunnig}:{repsDone + 1}] Order total time interval width: {TimeHelper.S2H(intervalWidth):F2} ({intervalWidthPercent:F2}%)");

                if (intervalWidthPercent < _confIntervalWidthPercent)
                {
                    Debug.WriteLine($"[{_confCurrentRunnig}:{repsDone + 1}] Interval width is small enough ({intervalWidthPercent:F2}%). Starting next simulation.");
                    simulation.StopSimulation();
                    return;
                }
            }
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _btn_simStart_Click(object sender, RoutedEventArgs e)
        {
            if (_confings is null || _confings.Length == 0)
            {
                MessageBox.Show("No configuration loaded. Please load a configuration file first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _txt_confsCurrent.Text = 0.ToString();
            _txt_confsTotal.Text = _confings?.Length.ToString() ?? "???";

            _txt_confsStartTime.Value = DateTime.Now.ToString("HH:mm:ss");
            _txt_confsEndTime.Value = "???";

            _startSimulation(0);
        }

        private void _startSimulation(int i)
        {
            var config = _confings![i].Split(';');
            _confIntervalWidthPercent = double.TryParse(config[1], out var intervalWidthPercent) ? intervalWidthPercent : throw new Exception("Interval width percent is not valid.");
            
            _simulation.InitializeFromCsvRow(config);

            _simulation.SimulateAsync(int.Parse(config[0]), _simulation.Endtime);
            _confCurrentRunnig = i;
        }

        private void _btn_simStop_Click(object sender, RoutedEventArgs e)
        {
            _simulation.StopSimulation();
        }

        private void _btn_simResume_Click(object sender, RoutedEventArgs e)
        {
            _simulation.ResumeSimulation();
        }

        private void _btn_simPause_Click(object sender, RoutedEventArgs e)
        {
            _simulation.PauseSimulation();
        }

        private void _btn_confLoadFile_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Configuration files (*.xml)|*.xml|All files (*.*)|*.*",
                Title = "Select configuration file",
                InitialDirectory = Environment.CurrentDirectory,
                FileName = _fileConfigs.FullName,
            };

            if (fileDialog.ShowDialog() == true)
            {
                _loadConfigsFromFile(fileDialog.FileName);
            }
        }

        private void _loadConfigsFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"File {filePath} does not exist.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _confings = File.ReadAllLines(filePath).Skip(1).ToArray();
            _txt_confLoadedCount.Value = _confings?.Length.ToString() ?? "???";
        }
    }
}
