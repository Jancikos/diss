using OSPABA;
using  FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariB
{
	//meta! id="12"
	public class AgentStolariB : OSPABA.Agent
	{
		public AgentStolariB(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerStolariB(SimId.ManagerStolariB, MySim, this);
			AddOwnMessage(Mc.RequestResponseDajStolara);
			AddOwnMessage(Mc.NoticeStolarUvolneny);
		}
		//meta! tag="end"
	}
}
