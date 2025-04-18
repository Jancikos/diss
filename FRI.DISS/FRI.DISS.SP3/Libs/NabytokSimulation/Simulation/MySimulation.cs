using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPresunuStolarov;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov;
using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariC;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariB;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Simulation
{
	public class MySimulation : OSPABA.Simulation
	{
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
