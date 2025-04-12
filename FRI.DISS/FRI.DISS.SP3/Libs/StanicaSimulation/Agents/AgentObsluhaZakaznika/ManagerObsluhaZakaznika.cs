using OSPABA;
using  FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;
namespace FRI.DISS.SP3.Libs.StanicaSimulation.Agents.AgentObsluhaZakaznika
{
	//meta! id="19"
	public class ManagerObsluhaZakaznika : OSPABA.Manager
	{
		public ManagerObsluhaZakaznika(int id, OSPABA.Simulation mySim, Agent myAgent) :
			base(id, mySim, myAgent)
		{
			Init();
		}

		override public void PrepareReplication()
		{
			base.PrepareReplication();
			// Setup component for the next replication

			if (PetriNet != null)
			{
				PetriNet.Clear();
			}
		}

		//meta! sender="AgentStanica", id="28", type="Request"
		public void ProcessRequestResponseObluzZakaznika(MessageForm message)
		{
		}

		//meta! sender="ProcessObsluzZakaznika", id="39", type="Finish"
		public void ProcessFinish(MessageForm message)
		{
		}

		//meta! userInfo="Process messages defined in code", id="0"
		public void ProcessDefault(MessageForm message)
		{
			switch (message.Code)
			{
			}
		}

		//meta! userInfo="Generated code: do not modify", tag="begin"
		public void Init()
		{
		}

		override public void ProcessMessage(MessageForm message)
		{
			switch (message.Code)
			{
			case Mc.RequestResponseObluzZakaznika:
				ProcessRequestResponseObluzZakaznika(message);
			break;

			case Mc.Finish:
				ProcessFinish(message);
			break;

			default:
				ProcessDefault(message);
			break;
			}
		}
		//meta! tag="end"
		public new AgentObsluhaZakaznika MyAgent
		{
			get
			{
				return (AgentObsluhaZakaznika)base.MyAgent;
			}
		}
	}
}
