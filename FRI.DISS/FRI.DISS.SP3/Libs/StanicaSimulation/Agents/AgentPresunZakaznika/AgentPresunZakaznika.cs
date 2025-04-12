using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika.ContinualAssistants;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentPresunZakaznika
{
	//meta! id="18"
	public class AgentPresunZakaznika : OSPABA.Agent
	{
		public AgentPresunZakaznika(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerPresunZakaznika(SimId.ManagerPresunZakaznika, MySim, this);
			new ProcessPresunZakaznika(SimId.ProcessPresunZakaznika, MySim, this);
			AddOwnMessage(Mc.RequestResponsePresunZakaznika);
		}
		//meta! tag="end"
	}
}
