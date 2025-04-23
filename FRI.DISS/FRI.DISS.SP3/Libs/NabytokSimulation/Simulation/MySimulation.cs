using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariC;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariB;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.Helpers;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
	public class MySimulation : OSPABA.Simulation
	{
        public int PracoviskaCount { get; set; } = 20;
        public Dictionary<StolarType, int> StolariCount { get; set; } = new()
        {
            { StolarType.A, 2 },
            { StolarType.B, 2 },
            { StolarType.C, 18 }
        };

        public SeedGenerator SeedGenerator { get; set; } = SeedGenerator.Global;

        // pracovna doba je denne od 6:00 do 14:00
        public string CurrentTimeFormatted => TimeSpan.FromSeconds(TimeHelper.HoursToSeconds(6) + (CurrentTime % TimeHelper.HoursToSeconds(8))).ToString(@"hh\:mm\:ss");
        public string CurrentTimeDayFormatted => ((int)CurrentTime / TimeHelper.HoursToSeconds(8)).ToString();

        public int Endtime => TimeHelper.HoursToSeconds(8) * 249; // 6:00 az 14:00 * 249 dni

		public MySimulation()
		{
			Init();
		}

		override public void PrepareSimulation()
		{
			base.PrepareSimulation();
			// Create global statistcis
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Reset entities, queues, local statistics, etc...
		}

		override public void ReplicationFinished()
		{
			// Collect local statistics into global, update UI, etc...
			base.ReplicationFinished();
		}

		override public void SimulationFinished()
		{
			// Display simulation results
			base.SimulationFinished();
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			AgentModelu = new AgentModelu(SimId.AgentModelu, this, null);
			AgentOkolia = new AgentOkolia(SimId.AgentOkolia, this, AgentModelu);
			AgentStolarov = new AgentStolarov(SimId.AgentStolarov, this, AgentModelu);
			AgentStolariA = new AgentStolariA(SimId.AgentStolariA, this, AgentStolarov);
			AgentStolariB = new AgentStolariB(SimId.AgentStolariB, this, AgentStolarov);
			AgentStolariC = new AgentStolariC(SimId.AgentStolariC, this, AgentStolarov);
			AgentPracovisk = new AgentPracovisk(SimId.AgentPracovisk, this, AgentModelu);
			AgentPresunuStolarov = new AgentPresunuStolarov(SimId.AgentPresunuStolarov, this, AgentStolarov);
		}
		public AgentModelu AgentModelu
		{ get; set; }
		public AgentOkolia AgentOkolia
		{ get; set; }
		public AgentStolarov AgentStolarov
		{ get; set; }
		public AgentStolariA AgentStolariA
		{ get; set; }
		public AgentStolariB AgentStolariB
		{ get; set; }
		public AgentStolariC AgentStolariC
		{ get; set; }
		public AgentPracovisk AgentPracovisk
		{ get; set; }
		public AgentPresunuStolarov AgentPresunuStolarov
		{ get; set; }
		//meta! tag="end"
	}
}
