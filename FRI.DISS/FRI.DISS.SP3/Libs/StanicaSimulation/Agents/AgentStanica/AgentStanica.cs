using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentStanica
{
	//meta! id="5"
	public class AgentStanica : OSPABA.Agent
	{
        public int CustomersInStation { get; set; } = 0;

		public AgentStanica(int id, OSPABA.Simulation mySim, Agent parent) :
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
			new ManagerStanica(SimId.ManagerStanica, MySim, this);
			AddOwnMessage(Mc.NoticePrichodZakaznika);
			AddOwnMessage(Mc.RequestResponseObluzZakaznika);
			AddOwnMessage(Mc.RequestResponsePresunZakaznika);
		}
		//meta! tag="end"
	}
}