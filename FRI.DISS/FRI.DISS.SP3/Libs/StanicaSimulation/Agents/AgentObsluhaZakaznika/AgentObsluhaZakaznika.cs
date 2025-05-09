using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.InstantAssistants;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika.ContinualAssistants;
using FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentModelu;

namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika
{
	//meta! id="19"
	public class AgentObsluhaZakaznika : OSPABA.Agent
	{
        public bool IsServiceFree { get; set; } = true;
        public Queue<Customer> CustomersQueue { get; set; } = new();

		public AgentObsluhaZakaznika(int id, OSPABA.Simulation mySim, Agent parent) :
			base(id, mySim, parent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

            CustomersQueue.Clear();
            IsServiceFree = true;
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		private void Init()
		{
			new ManagerObsluhaZakaznika(SimId.ManagerObsluhaZakaznika, MySim, this);
			new ProcessObsluzZakaznika(SimId.ProcessObsluzZakaznika, MySim, this);
			new AdviserZaradZakaznikaDoFronty(SimId.AdviserZaradZakaznikaDoFronty, MySim, this);
			AddOwnMessage(Mc.RequestResponseObluzZakaznika);
		}
		//meta! tag="end"
	}
}