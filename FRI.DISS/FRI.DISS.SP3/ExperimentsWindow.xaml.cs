using System;
using System.Collections.Generic;
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
        private int _confCurrent = 0;
        private string[]? _confings;
        
        public ExperimentsWindow()
        {
            InitializeComponent();

            _simulation = new MySimulation();

            _simulation.OnRefreshUI(_onRefreshUI);
            _simulation.OnReplicationWillStart(_onReplicationWillStart);
            _simulation.OnReplicationDidFinish(_onReplicationDidFinish);
            _simulation.OnSimulationWillStart(_onSimulationWillStart);
            _simulation.OnSimulationDidFinish(_onSimulationDidFinish);
        }

        private void _onSimulationWillStart(Simulation simulation)
        {

        }
        private void _onSimulationDidFinish(Simulation simulation)
        {
            var repsDone = _simulation.CurrentReplication;

            Dispatcher.InvokeAsync(() =>
            {
                _txt_repsEndTime.Value = DateTime.Now.ToString("HH:mm:ss");

                _refreshReplicationsDoneCounters(repsDone);
                _refreshReplicationsStats(_simulation.ReplicationsStatistics);
            });

            // save simulation results to csv file
            if (repsDone > 30)
            {
                NabytokReplicationsStatisticsCsvWriter.Instance.Write(
                    new FileInfo("./experimentsOutput.csv"),
                    _simulation
                );
            }


            // should start another simulation
            var runAnotherSimulation = true;

            if (!runAnotherSimulation)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    _txt_confsEndTime.Value = DateTime.Now.ToString("HH:mm:ss");
                });
                return;
            }

            // start another simulation
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
                _txt_simCurrentReplication.Text = repsDone.ToString();
            });
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btn_simStart_Click(object sender, RoutedEventArgs e)
        {
            var repsCount = _txt_simReplicationsCount.IntValue;
            _txt_expTotalReplications.Text = repsCount.ToString();

            _initializeSimulationFromGUI();
            _simulation.SimulateAsync(repsCount, _simulation.Endtime);
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
                Title = "Select configuration file"
            };

            if (fileDialog.ShowDialog() == true)
            {
                _confings = File.ReadAllLines(fileDialog.FileName);
                _txt_confLoadedCount.Value = _confings?.Length.ToString() ?? "???";
            }

        }
    }
}
