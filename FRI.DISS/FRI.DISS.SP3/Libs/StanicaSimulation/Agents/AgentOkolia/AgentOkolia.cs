using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia.ContinualAssistants;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentOkolia
{
	//meta! id="3"
	public class AgentOkolia : OSPABA.Agent
	{
		public AgentOkolia(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerOkolia(SimId.ManagerOkolia, MySim, this);
			new SchedulerPrichodZakaznika(SimId.SchedulerPrichodZakaznika, MySim, this);
			AddOwnMessage(Mc.NoticeInicializuj);
		}
		//meta! tag="end"
	}
}