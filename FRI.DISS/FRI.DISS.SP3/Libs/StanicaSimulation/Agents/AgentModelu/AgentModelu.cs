using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu
{
	/*!
	 * Tiez by som ho považoval za agenta celej stanice
	 */
	//meta! id="1"
	public class AgentModelu : OSPABA.Agent
	{
		public AgentModelu(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerModelu(SimId.ManagerModelu, MySim, this);
			AddOwnMessage(Mc.NoticePrichodZakaznika);
			AddOwnMessage(Mc.NoticeOdchodZakaznika);
		}
		//meta! tag="end"
	}
}