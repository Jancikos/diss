using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia.ContinualAssistants;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentOkolia
{
	//meta! id="2"
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
			new SchedulerPrichody(SimId.SchedulerPrichody, MySim, this);
			AddOwnMessage(Mc.NoticeInicialuzuj);
		}
		//meta! tag="end"
	}
}
