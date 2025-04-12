using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia;
using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentStanica;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Simulation
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
			AgentStanica = new AgentStanica(SimId.AgentStanica, this, AgentModelu);
			AgentPresunZakaznika = new AgentPresunZakaznika(SimId.AgentPresunZakaznika, this, AgentStanica);
			AgentObsluhaZakaznika = new AgentObsluhaZakaznika(SimId.AgentObsluhaZakaznika, this, AgentStanica);
		}
		public AgentModelu AgentModelu
		{ get; set; }
		public AgentOkolia AgentOkolia
		{ get; set; }
		public AgentStanica AgentStanica
		{ get; set; }
		public AgentPresunZakaznika AgentPresunZakaznika
		{ get; set; }
		public AgentObsluhaZakaznika AgentObsluhaZakaznika
		{ get; set; }
		//meta! tag="end"
	}
}